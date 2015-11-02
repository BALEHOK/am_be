using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.SearchEngine.Interface;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;

namespace AppFramework.Core.Classes.SearchEngine.SearchOperators
{
    public abstract class BaseOperator : ISearchOperator
    {
        public abstract string GetOperator();

        public virtual SearchTerm Generate(string value, string fieldName)
        {
            var term = new SearchTerm();

            var paramName = string.Format("@parameter{0}", GetHashCode());
            term.CommandText = string.Format("{0} {1} {2}",
                fieldName.Contains("[") ? fieldName : string.Format("[{0}]", fieldName),
                GetOperator(),
                paramName);
            if (string.IsNullOrEmpty(value))
            {
                // set size = 4000, beacause later empty string value could be overidden by default sql date for example
                // and its length is more than 1. bad design
                term.Parameter = new SqlParameter(paramName, SqlDbType.NVarChar, 4000);
                term.Parameter.Value = string.Empty;
            }
            else
            {
                term.Parameter = new SqlParameter(paramName, value);
            }

            return term;
        }

        public virtual SearchTerm GenerateForContext(AttributeElement chain)
        {
            var paramName = string.Format("{0}", string.Format("@parameter{0}", GetHashCode()));
            var term = new SearchTerm();
            term.Parameter = new SqlParameter(paramName, chain.Value);
            var leftPart = " StringValue ";

            switch (chain.ElementType)
            {
                case Enumerators.DataType.DynList:
                    leftPart = " DynamicListItemUid ";
                    term.Parameter = new SqlParameter(paramName, chain.DynListItemId);
                    break;
                case Enumerators.DataType.Long:
                case Enumerators.DataType.Int:
                case Enumerators.DataType.Asset:
                case Enumerators.DataType.Revision:
                case Enumerators.DataType.Assets:
                case Enumerators.DataType.Document:
                case Enumerators.DataType.Role:
                case Enumerators.DataType.Zipcode:
                case Enumerators.DataType.Permission:
                    leftPart = " NumericValue ";
                    term.Parameter = new SqlParameter(paramName, long.Parse(chain.Value));
                    break;
                case Enumerators.DataType.Euro:
                case Enumerators.DataType.Money:
                case Enumerators.DataType.USD:
                    leftPart = "cast(StringValue as money)";
                    break;

                case Enumerators.DataType.Float:
                    leftPart = " NumericValue ";
                    double result;
                    if (double.TryParse(chain.Value, NumberStyles.Float, ApplicationSettings.DisplayCultureInfo,
                        out result))
                        term.Parameter = new SqlParameter(paramName, result);
                    break;
                case Enumerators.DataType.DateTime:
                case Enumerators.DataType.CurrentDate:
                    if (!string.IsNullOrEmpty(chain.Value))
                    {
                        DateTime dt;

                        if (DateTime.TryParse(chain.Value, ApplicationSettings.DisplayCultureInfo.DateTimeFormat,
                            DateTimeStyles.None, out dt))
                        {
                            if (dt.TimeOfDay.TotalSeconds == 0)
                            {
                                leftPart = " CAST( DateTimeValue AS DATE ) ";
                                term.Parameter = new SqlParameter(paramName, dt.ToShortDateString());
                            }
                            else
                            {
                                leftPart = " DateTimeValue ";
                                term.Parameter = new SqlParameter(paramName, dt);
                            }
                        }
                    }
                    break;
            }
            term.CommandText = string.Format(" {0} {1} {2} ", leftPart, GetOperator(), paramName);
            return term;
        }

        public static ISearchOperator GetOperatorByClassName(string className)
        {
            var opType = Type.GetType(
                string.Format("AppFramework.Core.Classes.SearchEngine.SearchOperators.{0}", className)
                );

            if (opType == null)
            {
                throw new Exception(string.Format("Search operator {0} not implemented", className));
            }

            var op = Activator.CreateInstance(opType) as ISearchOperator;

            if (op == null)
            {
                throw new Exception(string.Format("Search operator {0} can not be created", className));
            }

            return op;
        }
    }
}