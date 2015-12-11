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
            #region Build select statement with joins

            // retrieve complex chains
            var complexChains = elements.Where(el => el.IsComplex);

            // join appropriate assets tables for multiple assets elements
            var joins = new HashSet<string>();
            foreach (var element in complexChains)
            {
                string joinLine;

                // join related asset table to be able to apply complex WHERE condition
                if (element.ElementType == Enumerators.DataType.Asset)
                {
                    var joinFormat = " LEFT JOIN [{0}] ON [{1}].[{2}] = [{0}].[DynEntityId]";
                    if (period == TimePeriodForSearch.CurrentTime)
                    {
                        joinFormat += " AND [{0}].[" + AttributeNames.ActiveVersion + "] = 1 ";
                    }
                    joinLine =
                        string.Format(
                            joinFormat,
                            element.ReferencedAssetType.DBTableName,
                            at.DBTableName,
                            element.FieldSql);
                }
                else if (IsDynamicList(element))
                {
                    joinLine =
                        string.Format(
                            @" LEFT JOIN DynListValue ON DynListValue.DynEntityConfigUid = [{0}].DynEntityConfigUid AND DynListValue.AssetUid = [{0}].DynEntityUid ",
                            at.DBTableName);
                }
                else
                {
                    throw new NotImplementedException(element.ElementType + " ElementType is not supported in search");
                }

                if (!joins.Contains(joinLine))
                    joins.Add(joinLine);
            }

            #endregion

            var query = string.Format(@"SELECT ''{3}'', [{0}].DynEntityUid, [{0}].DynEntityConfigUid 
                                           FROM [{0}] 
                                           {1} 
                                           {2}",
                at.DBTableName,
                string.Join(" ", joins),
                _getWhereStatement(elements, period, at.DBTableName, out parameters),
                searchId);
            return query;
        }

        /// <summary>
        /// Returns the SQL where statement
        /// </summary>
        private static string _getWhereStatement(List<AttributeElement> elements, TimePeriodForSearch period,
            string tableName, out List<SqlParameter> parameters, bool skipBrackets = false)
        {
            var query = new StringBuilder();
            parameters = new List<SqlParameter>();

            query.Append(" WHERE ");
            if (elements.Count > 0)
            {
                query.Append("(");

                BuildQuery(elements, period, tableName, parameters, skipBrackets, query);

                query.Append(") AND ");
            }

            query.AppendFormat("[{0}].[{1}] = {2}",
                tableName,
                AttributeNames.ActiveVersion,
                (int)period);

            return query.ToString();
        }

        private static void BuildQuery(List<AttributeElement> elements, TimePeriodForSearch period, string tableName, List<SqlParameter> parameters, bool skipBrackets, StringBuilder query)
        {
            for (var i = 0; i < elements.Count; i++)
            {
                var element = elements[i];

                if (element.UseComplexValue)
                {
                    if (!skipBrackets)
                    {
                        query.Append(element.StartBrackets);
                    }

                    var oper = BaseOperator.GetOperatorByClassName(element.ServiceMethod);
                    if (element.ElementType == Enumerators.DataType.ChildAssets)
                    {
                        query.Append("[" + tableName + "].[DynEntityId]");
                        if (oper is AssetInOperator)
                        {
                            query.Append(" IN (");
                        }
                        else if (oper is AssetNotInOperator)
                        {
                            query.Append(" NOT IN (");
                        }
                        else
                        {
                            throw new Exception("operator " + oper + " not supported with ChildAssets attribute condition");
                        }

                        query.AppendFormat("SELECT [{0}] FROM [{1}] WHERE {2}",
                            element.FieldSql,
                            element.ReferencedAssetType.DBTableName,
                            period == TimePeriodForSearch.CurrentTime ? "(" : String.Empty);

                        BuildQuery(element.ComplexValue, period, element.ReferencedAssetType.DBTableName, parameters, skipBrackets, query);

                        query.Append(") ");

                        if (period == TimePeriodForSearch.CurrentTime)
                        {
                            if (period == TimePeriodForSearch.CurrentTime)
                            {
                                query.AppendFormat(" AND [{0}].[{1}] = 1)",
                                    element.ReferencedAssetType.DBTableName,
                                    AttributeNames.ActiveVersion);
                            }
                        }
                    }
                    else
                    {
                        if (oper is AssetNotInOperator)
                        {
                            query.Append(" NOT");
                        }
                        query.Append(" (");

                        BuildQuery(element.ComplexValue, period, element.ReferencedAssetType.DBTableName, parameters, skipBrackets, query);
                        query.Append(") ");
                    }
                    if (!skipBrackets)
                    {
                        query.Append(element.EndBrackets);
                    }

                    if (i < elements.Count - 1)
                    {
                        query.Append(element.AndOr == 0 ? " AND " : " OR ");
                    }
                    continue;
                }

                var t = GetSearchTerm(element, tableName);

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
                        sb.Append("[" + tableName + "].[DynEntityId]");
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

                if (skipBrackets)
                {
                    query.Append(t.CommandText);
                }
                else
                {
                    query.Append(element.StartBrackets);
                    query.Append(t.CommandText);
                    query.Append(element.EndBrackets);
                }

                if (i < elements.Count - 1)
                {
                    query.Append(element.AndOr == 0 ? " AND " : " OR ");
                }
            }
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

            if (element.ElementType == Enumerators.DataType.ChildAssets && !element.UseComplexValue)
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