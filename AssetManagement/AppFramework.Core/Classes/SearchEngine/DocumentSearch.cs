namespace AppFramework.Core.Classes.SearchEngine
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.OleDb;
    using AppFramework.ConstantsEnumerators;
    using AppFramework.Core.Classes.SearchEngine.Interface;
    using Common.Logging;

    internal class DocumentSearch
    {
        private static ILog _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// search by keywords
        /// </summary>
        /// <param name="allKeywords">all keywords in one string</param>
        /// <returns></returns>
        public static IEnumerable<Asset> FindByKeywords(string allKeywords)
        {
            List<Asset> result = new List<Asset>();
            string connectionString = string.Format("Provider= \"MSIDXS\";Data Source=\"{0}\";", ConfigurationManager.AppSettings["DocumentDirectory"]);
            string query = string.Format(@"SELECT FileName FROM scope() " +
               @"WHERE FREETEXT(Contents, '{0}')", allKeywords);

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    connection.Open();
                    try
                    {
                        OleDbDataReader reader = command.ExecuteReader();

                        List<ISearchCondition> conditions = new List<ISearchCondition>();
                        conditions.Add(new SearchCondition()
                        {
                            FieldName = "ActiveVersion",
                            Value = "True",
                            AndOr = Enumerations.ConcatenationOperation.And,
                            SearchOperator = Enumerations.SearchOperator.Equal
                        });

                        while (reader.Read())
                        {
                            AssetType assetType = AssetType.GetByID(PredefinedAttribute.Get(PredefinedEntity.Document).DynEntityConfigID);
                            if (assetType != null)
                            {
                                int totalCount;
                                conditions.Add(new SearchCondition()
                                {
                                    FieldName = "DocumentFile",
                                    Value = string.Format("%{0}%", reader.GetString(0)),
                                    AndOr = Enumerations.ConcatenationOperation.And,
                                    SearchOperator = Enumerations.SearchOperator.Like
                                });

                                // TODO
                                throw new System.NotImplementedException();
                                //result.AddRange(TypeSearch.GetAssetsByType(
                                //    assetType, conditions.ToSearchChains().ToList(), Enumerations.TimePeriodForSearch.CurrentTime, out totalCount));

                                conditions.RemoveAt(1);
                            }
                        }
                    }
                    catch (OleDbException dbEx)
                    {
                        _logger.Error(dbEx);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return result;
        }
    }
}