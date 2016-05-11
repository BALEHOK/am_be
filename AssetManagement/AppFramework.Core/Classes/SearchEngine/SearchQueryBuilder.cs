using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Core.Classes.SearchEngine.SearchOperators;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.Core.ConstantsEnumerators;

namespace AppFramework.Core.Classes.SearchEngine
{
    public class SearchQueryBuilder
    {
        /// <summary>
        /// Complex search performs when there are complex attributes dynlists, multipleassets) in search chains
        /// </summary>
        /// <returns></returns>
        public static string GenerateTypeSearchQuery(Guid searchId, AssetType at, List<AttributeElement> elements,
            TimePeriodForSearch period, out List<SqlParameter> parameters)
        {
            var joins = new HashSet<string>();
            var whereSb = new StringBuilder();
            parameters = new List<SqlParameter>();

            foreach (var complexElement in elements.Where(el => el.IsComplex))
            {
                var joinString = GetJoinTableForComplexValue(complexElement, period, at.DBTableName);
                if (!joins.Contains(joinString))
                {
                    joins.Add(joinString);
                }
            }
            
            if (elements.Count > 0)
            {
                whereSb.Append("(");
            }

            AppendWhereConditionsForElements(elements, period, at.DBTableName, whereSb, parameters);

            if (elements.Count > 0)
            {    
                whereSb.Append(") AND ");
            }

            whereSb.AppendFormat("[{0}].[{1}] = {2}",
                    at.DBTableName,
                    AttributeNames.ActiveVersion,
                    (int)period);

            var query = string.Format(@"SELECT ''{0}'', [{1}].DynEntityUid, [{1}].DynEntityConfigUid 
                                           FROM [{1}] {2} 
                                           WHERE {3}",
                searchId,
                at.DBTableName,
                string.Join(" ", joins),
                whereSb);

            return query;
        }

        private static string GetJoinTableForComplexValue(AttributeElement element, TimePeriodForSearch period, string dbTableName)
        {
            // join related asset table to be able to apply complex WHERE condition
            if (element.ElementType == Enumerators.DataType.Asset)
            {
                var joinFormat = " LEFT JOIN [{0}] ON [{1}].[{2}] = [{0}].[DynEntityId]";
                if (period == TimePeriodForSearch.CurrentTime)
                {
                    joinFormat += " AND [{0}].[" + AttributeNames.ActiveVersion + "] = 1 ";
                }

                return string.Format(
                        joinFormat,
                        element.ReferencedAssetType.DBTableName,
                        dbTableName,
                        element.FieldSql);
            }

            if (IsDynamicList(element))
            {
                return string.Format(
                        @" LEFT JOIN DynListValue ON DynListValue.DynEntityConfigUid = [{0}].DynEntityConfigUid AND DynListValue.AssetUid = [{0}].DynEntityUid ",
                        dbTableName);
            }

            throw new NotImplementedException(element.ElementType + " ElementType is not supported in search");
        }

        private static void AppendWhereConditionsForElements(List<AttributeElement> elements, TimePeriodForSearch period, string dbTableName, StringBuilder whereSb, List<SqlParameter> parameters)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                var element = elements[i];
                if (element.UseComplexValue)
                {
                    AppendWhereForComlexCondition(element, period, dbTableName, whereSb, parameters);
                }
                else
                {
                    AppendWhereForSimpleCondition(element, period, dbTableName, whereSb, parameters);
                }

                if (i < elements.Count - 1)
                {
                    whereSb.Append(element.AndOr == 0 ? " AND " : " OR ");
                }
            }
        }

        private static void AppendWhereForComlexCondition(AttributeElement element, TimePeriodForSearch period, string dbTableName, StringBuilder whereSb, List<SqlParameter> parameters)
        {
            whereSb.Append(element.StartBrackets);

            var oper = BaseOperator.GetOperatorByClassName(element.ServiceMethod);
            if (element.ElementType != Enumerators.DataType.ChildAssets)
            {
                if (oper is AssetNotInOperator)
                {
                    whereSb.Append(" NOT");
                }
                whereSb.Append(" (");

                AppendWhereConditionsForElements(element.ComplexValue, period, element.ReferencedAssetType.DBTableName,
                    whereSb, parameters);
                whereSb.Append(") ");
            }
            else
            {
                whereSb.Append("[" + dbTableName + "].[DynEntityId]");
                if (oper is AssetInOperator)
                {
                    whereSb.Append(" IN (");
                }
                else if (oper is AssetNotInOperator)
                {
                    whereSb.Append(" NOT IN (");
                }
                else
                {
                    throw new Exception("operator " + oper + " not supported with ChildAssets attribute condition");
                }

                whereSb.AppendFormat("SELECT [{0}] FROM [{1}] WHERE {2}",
                    element.FieldSql,
                    element.ReferencedAssetType.DBTableName,
                    period == TimePeriodForSearch.CurrentTime ? "(" : String.Empty);

                AppendWhereConditionsForElements(element.ComplexValue, period, element.ReferencedAssetType.DBTableName,
                    whereSb, parameters);

                whereSb.Append(") ");

                if (period == TimePeriodForSearch.CurrentTime)
                {
                     whereSb.AppendFormat(" AND [{0}].[{1}] = 1)",
                            element.ReferencedAssetType.DBTableName,
                            AttributeNames.ActiveVersion);
                }
            }

            whereSb.Append(element.EndBrackets);
        }

        private static void AppendWhereForSimpleCondition(AttributeElement element, TimePeriodForSearch period, string dbTableName, StringBuilder whereSb, List<SqlParameter> parameters)
        {
            var t = GetSearchTerm(element, dbTableName);

            switch (element.ElementType)
            {
                case Enumerators.DataType.CurrentDate:
                case Enumerators.DataType.DateTime:
                    DateTime time;
                    t.Parameter.Value = DateTime.TryParse(element.Value, out time) ? time : SqlDateTime.MinValue.Value;

                    break;
                case Enumerators.DataType.Int:
                    int intvalue;
                    int.TryParse(element.Value, out intvalue);
                    t.Parameter.Value = intvalue;
                    break;
                case Enumerators.DataType.Long:
                    long longvalue;
                    long.TryParse(element.Value, out longvalue);
                    t.Parameter.Value = longvalue;
                    break;
                case Enumerators.DataType.Float:
                    float fvalue;
                    float.TryParse(element.Value, out fvalue);
                    t.Parameter.Value = fvalue;
                    break;
                case Enumerators.DataType.Bool:
                    bool bValue;
                    if (element.Value.ToLower() == "true" || element.Value == "1")
                    {
                        bValue = true;
                    }
                    else
                    {
                        bool.TryParse(element.Value, out bValue);
                    }
                    t.Parameter.Value = bValue;
                    break;
                case Enumerators.DataType.Money:
                case Enumerators.DataType.Euro:
                    decimal dvalue;
                    decimal.TryParse(element.Value, out dvalue);
                    t.Parameter.Value = dvalue;
                    break;
                case Enumerators.DataType.ChildAssets:
                    var sb = new StringBuilder();
                    sb.Append("[" + dbTableName + "].[DynEntityId]");
                    var oper = BaseOperator.GetOperatorByClassName(element.ServiceMethod);
                    if (oper is AssetInOperator)
                    {
                        sb.Append(" IN (");
                    }
                    else if (oper is AssetNotInOperator)
                    {
                        sb.Append(" NOT IN (");
                    }
                    else
                    {
                        throw new Exception("operator " + oper + " not supported with ChildAssets attribute condition");
                    }

                    sb.AppendFormat("SELECT [{0}] FROM [{1}] WHERE [{1}].[DynEntityId] = {2}",
                        element.FieldSql,
                        element.ReferencedAssetType.DBTableName,
                        t.Parameter.ParameterName);

                    if (period == TimePeriodForSearch.CurrentTime)
                    {
                        sb.AppendFormat(" AND [{0}].[{1}] = 1",
                            element.ReferencedAssetType.DBTableName,
                            AttributeNames.ActiveVersion);
                    }

                    sb.Append(") ");

                    t.CommandText = sb.ToString();
                    break;
            }

            if (t.Parameter != null)
            {
                parameters.Add(t.Parameter);
            }

            whereSb.Append(element.StartBrackets);
            whereSb.Append(t.CommandText);
            whereSb.Append(element.EndBrackets);
        }

        /// <summary>
        /// Returns the SQL statement for current element
        /// </summary>
        private static SearchTerm GetSearchTerm(AttributeElement element, string tableName)
        {
            if (string.IsNullOrEmpty(element.FieldSql))
            {
                if (string.IsNullOrEmpty(element.FieldName))
                {
                    throw new ArgumentException("Chain properties not set");
                }

                element.FieldSql = element.FieldName;
            }

            string fieldName;

            if (element.ElementType == Enumerators.DataType.DynList)
            {
                fieldName = "[DynListValue].[DynlistItemUid]";
            } else if (element.ElementType == Enumerators.DataType.ChildAssets && !element.UseComplexValue)
            {
                fieldName = "[DynEntityId]";
            }
            //if field sql contains additional operations TODO:Refactor
            else if (element.FieldSql.Contains("("))
            {
                fieldName = element.FieldSql.Replace(element.FieldName, "[" + tableName + "].[" + element.FieldName + "]");
            }
            else
            {
                // convert field name to the full name within table context
                fieldName = element.FieldSql.Contains("].[") ? element.FieldSql :
                    string.Format("[{0}].[{1}]", tableName, element.FieldSql.Trim('[', ']'));
            }

            return BaseOperator.GetOperatorByClassName(element.ServiceMethod).
                    Generate(element.Value, fieldName);
        }

        /// <summary>
        /// Returns if chain is Dymanic List(s) or not
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static bool IsDynamicList(AttributeElement element)
        {
            return element.ElementType == Enumerators.DataType.DynList ||
                   element.ElementType == Enumerators.DataType.DynLists;
        }
    }
}