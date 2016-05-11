using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Core.Classes.SearchEngine.SearchOperators;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.UnitTests.Fixtures;
using AppFramework.Core.UnitTests.Fixtures.AssetTypes;
using AppFramework.DataProxy;
using Ploeh.AutoFixture.Xunit;
using Xunit;
using Xunit.Extensions;

namespace AppFramework.Core.UnitTests.Classes.SearchEngine
{
    public class SearchQueryBuilderTest
    {
        [Theory]
        [InlineAutoDomainData(TimePeriodForSearch.CurrentTime)]
        [InlineAutoDomainData(TimePeriodForSearch.History)]
        public void GenerateTypeSearchQuery_ByType_OneSimpleCondition(TimePeriodForSearch period, [Frozen]IUnitOfWork unitOfWork)
        {
            var searchId = Guid.NewGuid();

            var assetType = CreateTestAssetType(unitOfWork);

            var elements = new List<AttributeElement>
            {
                CreateNameCondition()
            };

            List<SqlParameter> parameters;
            var actual = SearchQueryBuilder.GenerateTypeSearchQuery(searchId, assetType, elements, period,
                out parameters);

            var expected = string.Format(
                "SELECT ''{0}'', [ADynEntity_test_table_name].DynEntityUid, [ADynEntity_test_table_name].DynEntityConfigUid " +
                "FROM [ADynEntity_test_table_name] " +
                "WHERE ([ADynEntity_test_table_name].[Name] LIKE {1}) " +
                "AND [ADynEntity_test_table_name].[ActiveVersion] = {2}",
                searchId,
                parameters[0].ParameterName,
                (int)period);

            Assert.Equal(IgnoreWhitespace(expected), IgnoreWhitespace(actual));
        }

        [Theory]
        [InlineAutoDomainData(TimePeriodForSearch.CurrentTime)]
        [InlineAutoDomainData(TimePeriodForSearch.History)]
        public void GenerateTypeSearchQuery_ByType_InSimpleAssetCondition(TimePeriodForSearch period, [Frozen]IUnitOfWork unitOfWork)
        {
            var searchId = Guid.NewGuid();

            var assetType = CreateTestAssetType(unitOfWork);

            var elements = new List<AttributeElement>
            {
                CreateUpdateUserSimpleCondition()
            };

            List<SqlParameter> parameters;
            var actual = SearchQueryBuilder.GenerateTypeSearchQuery(searchId, assetType, elements, period,
                out parameters);

            var expected = string.Format(
                "SELECT ''{0}'', [ADynEntity_test_table_name].DynEntityUid, [ADynEntity_test_table_name].DynEntityConfigUid " +
                "FROM [ADynEntity_test_table_name] " +
                "WHERE ([ADynEntity_test_table_name].[UpdateUserId] = {1}) AND [ADynEntity_test_table_name].[ActiveVersion] = {2}",
                searchId,
                parameters[0].ParameterName,
                (int)period);

            Assert.Equal(IgnoreWhitespace(expected), IgnoreWhitespace(actual));
        }

        [Theory]
        [InlineAutoDomainData(TimePeriodForSearch.CurrentTime)]
        [InlineAutoDomainData(TimePeriodForSearch.History)]
        public void GenerateTypeSearchQuery_ByType_AndOrConditionsWithParentheses(TimePeriodForSearch period, [Frozen]IUnitOfWork unitOfWork)
        {
            var searchId = Guid.NewGuid();

            var assetType = CreateTestAssetType(unitOfWork);

            var elements = new List<AttributeElement>
            {
                CreateNameCondition("(", operation: ConcatenationOperation.Or),
                CreateRevisionCondition(operation: ConcatenationOperation.And, endBrackets:")"),
                CreateUpdateUserSimpleCondition()
            };

            List<SqlParameter> parameters;
            var actual = SearchQueryBuilder.GenerateTypeSearchQuery(searchId, assetType, elements, period,
                out parameters);

            var expected = string.Format(
                "SELECT ''{0}'', [ADynEntity_test_table_name].DynEntityUid, [ADynEntity_test_table_name].DynEntityConfigUid " +
                "FROM [ADynEntity_test_table_name] " +
                "WHERE (" +
                        "([ADynEntity_test_table_name].[Name] LIKE {1} " +
                            "OR [ADynEntity_test_table_name].[Revision] >= {2}) " +
                        "AND [ADynEntity_test_table_name].[UpdateUserId] = {3}" +
                    ") AND [ADynEntity_test_table_name].[ActiveVersion] = {4}",
                searchId,
                parameters[0].ParameterName,
                parameters[1].ParameterName,
                parameters[2].ParameterName,
                (int)period);

            Assert.Equal(IgnoreWhitespace(expected), IgnoreWhitespace(actual));
        }

        [Theory]
        [InlineAutoDomainData(TimePeriodForSearch.CurrentTime, typeof(AssetInOperator))]
        [InlineAutoDomainData(TimePeriodForSearch.History, typeof(AssetInOperator))]
        [InlineAutoDomainData(TimePeriodForSearch.CurrentTime, typeof(AssetNotInOperator))]
        [InlineAutoDomainData(TimePeriodForSearch.History, typeof(AssetNotInOperator))]
        public void GenerateTypeSearchQuery_ByType_ComplexAssetCondition(TimePeriodForSearch period, Type operatorType, [Frozen]IUnitOfWork unitOfWork)
        {
            var searchId = Guid.NewGuid();

            var assetType = CreateTestAssetType(unitOfWork);

            var elements = new List<AttributeElement>
            {
                CreateUpdateUserComplexCondition(unitOfWork, operatorType)
            };

            List<SqlParameter> parameters;
            var actual = SearchQueryBuilder.GenerateTypeSearchQuery(searchId, assetType, elements, period,
                out parameters);

            var expected = string.Format(
                "SELECT ''{0}'', [ADynEntity_test_table_name].DynEntityUid, [ADynEntity_test_table_name].DynEntityConfigUid " +
                "FROM [ADynEntity_test_table_name] " +
                "LEFT JOIN [ADynEntity_users_table_name] ON [ADynEntity_test_table_name].[UpdateUserId] = [ADynEntity_users_table_name].[DynEntityId] {3} " +
                "WHERE ( {4} ([ADynEntity_users_table_name].[Name] LIKE {1}) ) AND [ADynEntity_test_table_name].[ActiveVersion] = {2}",
                searchId,
                parameters[0].ParameterName,
                (int)period,
                period == TimePeriodForSearch.CurrentTime
                ? "AND [ADynEntity_users_table_name].[ActiveVersion] = 1"
                : string.Empty,
                operatorType == typeof(AssetNotInOperator) ? "NOT" : string.Empty);

            Assert.Equal(IgnoreWhitespace(expected), IgnoreWhitespace(actual));
        }

        [Theory]
        [InlineAutoDomainData(TimePeriodForSearch.CurrentTime, typeof(AssetInOperator))]
        [InlineAutoDomainData(TimePeriodForSearch.History, typeof(AssetInOperator))]
        [InlineAutoDomainData(TimePeriodForSearch.CurrentTime, typeof(AssetNotInOperator))]
        [InlineAutoDomainData(TimePeriodForSearch.History, typeof(AssetNotInOperator))]
        public void GenerateTypeSearchQuery_ByType_SimpleChildAssetsCondition(TimePeriodForSearch period, Type operatorType, [Frozen]IUnitOfWork unitOfWork)
        {
            var searchId = Guid.NewGuid();

            var assetType = CreateTestAssetType(unitOfWork, "User", "_users_test_table");

            var elements = new List<AttributeElement>
            {
                CreateNameCondition(),
                CreateUserChildAssetsSimpleCondition(unitOfWork, operatorType)
            };

            List<SqlParameter> parameters;
            var actual = SearchQueryBuilder.GenerateTypeSearchQuery(searchId, assetType, elements, period,
                out parameters);

            var expected = string.Format(
                "SELECT ''{0}'', [ADynEntity_users_test_table].DynEntityUid, [ADynEntity_users_test_table].DynEntityConfigUid " +
                "FROM [ADynEntity_users_test_table] " +
                "WHERE ([ADynEntity_users_test_table].[Name] LIKE {1} " +
                    "AND [ADynEntity_users_test_table].[DynEntityId] {5} IN " +
                        "(SELECT [UpdateUserId] FROM [ADynEntity_test_table_name] WHERE [ADynEntity_test_table_name].[DynEntityId] = {2} {4}) ) AND [ADynEntity_users_test_table].[ActiveVersion] = {3}",
                searchId,
                parameters[0].ParameterName,
                parameters[1].ParameterName,
                (int)period,
                period == TimePeriodForSearch.CurrentTime
                    ? "AND [ADynEntity_test_table_name].[ActiveVersion] = 1"
                    : string.Empty,
                operatorType == typeof(AssetNotInOperator) ? "NOT" : string.Empty);

            Assert.Equal(IgnoreWhitespace(expected), IgnoreWhitespace(actual));
        }
        
        [Theory]
        [InlineAutoDomainData(TimePeriodForSearch.CurrentTime, typeof(AssetInOperator))]
        [InlineAutoDomainData(TimePeriodForSearch.History, typeof(AssetInOperator))]
        [InlineAutoDomainData(TimePeriodForSearch.CurrentTime, typeof(AssetNotInOperator))]
        [InlineAutoDomainData(TimePeriodForSearch.History, typeof(AssetNotInOperator))]
        public void GenerateTypeSearchQuery_ByType_ComplexChildAssetsCondition(TimePeriodForSearch period, Type operatorType, [Frozen]IUnitOfWork unitOfWork)
        {
            var searchId = Guid.NewGuid();

            var assetType = CreateTestAssetType(unitOfWork, "User", "_users_test_table");

            var elements = new List<AttributeElement>
            {
                CreateNameCondition(),
                CreateUserChildAssetsComplexCondition(unitOfWork, operatorType)
            };

            List<SqlParameter> parameters;
            var actual = SearchQueryBuilder.GenerateTypeSearchQuery(searchId, assetType, elements, period,
                out parameters);

            var expected = string.Format(
                "SELECT ''{0}'', [ADynEntity_users_test_table].DynEntityUid, [ADynEntity_users_test_table].DynEntityConfigUid " +
                "FROM [ADynEntity_users_test_table] " +
                "WHERE ([ADynEntity_users_test_table].[Name] LIKE {1} " +
                    "AND [ADynEntity_users_test_table].[DynEntityId] {7} IN " +
                        "(SELECT [UpdateUserId] FROM [ADynEntity_test_table_name] WHERE {5}[ADynEntity_test_table_name].[Name] LIKE {2} AND [ADynEntity_test_table_name].[UpdateUserId] = {3}) {6}) AND [ADynEntity_users_test_table].[ActiveVersion] = {4}",
                searchId,
                parameters[0].ParameterName,
                parameters[1].ParameterName,
                parameters[2].ParameterName,
                (int)period,
                period == TimePeriodForSearch.CurrentTime ? "(" : string.Empty,
                period == TimePeriodForSearch.CurrentTime
                    ? "AND [ADynEntity_test_table_name].[ActiveVersion] = 1)"
                    : string.Empty,
                operatorType == typeof(AssetNotInOperator) ? "NOT" : string.Empty);

            Assert.Equal(IgnoreWhitespace(expected), IgnoreWhitespace(actual));
        }

        [Theory]
        [InlineAutoDomainData(TimePeriodForSearch.CurrentTime)]
        [InlineAutoDomainData(TimePeriodForSearch.History)]
        public void GenerateTypeSearchQuery_ByType_DynListCondition(TimePeriodForSearch period, [Frozen]IUnitOfWork unitOfWork)
        {
            var searchId = Guid.NewGuid();

            var assetType = CreateTestAssetType(unitOfWork);

            var elements = new List<AttributeElement>
            {
                CreateRoleCondition()
            };

            List<SqlParameter> parameters;
            var actual = SearchQueryBuilder.GenerateTypeSearchQuery(searchId, assetType, elements, period,
                out parameters);

            var expected = string.Format(
                "SELECT ''{0}'', [ADynEntity_test_table_name].DynEntityUid, [ADynEntity_test_table_name].DynEntityConfigUid " +
                "FROM [ADynEntity_test_table_name] " +
                "LEFT JOIN DynListValue ON DynListValue.DynEntityConfigUid = [ADynEntity_test_table_name].DynEntityConfigUid AND DynListValue.AssetUid = [ADynEntity_test_table_name].DynEntityUid" +
                "WHERE ([DynListValue].[DynlistItemUid] = {1}) " +
                "AND [ADynEntity_test_table_name].[ActiveVersion] = {2}",
                searchId,
                parameters[0].ParameterName,
                (int)period);

            Assert.Equal(IgnoreWhitespace(expected), IgnoreWhitespace(actual));
        }

        private static AssetTypeFixture CreateTestAssetType(IUnitOfWork unitOfWork, string name = "Test asset type", string tableName = "_test_table_name")
        {
            return new AssetTypeFixture(unitOfWork)
            {
                Name = name,
                DBTableName = tableName
            };
        }

        private static AttributeElement CreateNameCondition(string startBrackets = "", string endBrackets = "",
            ConcatenationOperation operation = ConcatenationOperation.And)
        {
            return new AttributeElement
            {
                FieldName = AttributeNames.Name,
                FieldSql = AttributeNames.Name,
                ConcatenationOperation = operation,
                ServiceMethod = "LikeOperator",
                ElementType = Enumerators.DataType.String,
                UseComplexValue = false,
                Value = "test",
                StartBrackets = startBrackets,
                EndBrackets = endBrackets
            };
        }

        private static AttributeElement CreateRevisionCondition(string startBrackets = "", string endBrackets = "",
            ConcatenationOperation operation = ConcatenationOperation.And)
        {
            return new AttributeElement
            {
                FieldName = AttributeNames.Revision,
                FieldSql = AttributeNames.Revision,
                ConcatenationOperation = operation,
                ServiceMethod = "MoreEqualOperator",
                ElementType = Enumerators.DataType.Long,
                UseComplexValue = false,
                Value = "2",
                StartBrackets = startBrackets,
                EndBrackets = endBrackets
            };
        }

        private static AttributeElement CreateUpdateUserSimpleCondition(string startBrackets = "", string endBrackets = "",
            ConcatenationOperation operation = ConcatenationOperation.And)
        {
            return new AttributeElement
            {
                FieldName = "UpdateUser",
                FieldSql = AttributeNames.UpdateUserId,
                ConcatenationOperation = operation,
                ServiceMethod = "AssetInOperator",
                ElementType = Enumerators.DataType.Asset,
                UseComplexValue = false,
                Value = "1",
                StartBrackets = startBrackets,
                EndBrackets = endBrackets
            };
        }

        private static AttributeElement CreateUpdateUserComplexCondition(IUnitOfWork unitOfWork, Type operatorType, string startBrackets = "", string endBrackets = "", ConcatenationOperation operation = ConcatenationOperation.And)
        {
            var complexValue = new List<AttributeElement>
            {
                CreateNameCondition()
            };

            return new AttributeElement
            {
                FieldName = "UpdateUser",
                FieldSql = AttributeNames.UpdateUserId,
                ConcatenationOperation = operation,
                ServiceMethod = operatorType.Name,
                ElementType = Enumerators.DataType.Asset,
                UseComplexValue = true,
                ReferencedAssetType = CreateTestAssetType(unitOfWork, "User", "_users_table_name"),
                ComplexValue = complexValue,
                StartBrackets = startBrackets,
                EndBrackets = endBrackets
            };
        }

        private AttributeElement CreateUserChildAssetsSimpleCondition(IUnitOfWork unitOfWork, Type operatorType, string startBrackets = "", string endBrackets = "", ConcatenationOperation operation = ConcatenationOperation.And)
        {
            return new AttributeElement
            {
                FieldName = "UpdateUser",
                FieldSql = AttributeNames.UpdateUserId,
                ConcatenationOperation = operation,
                ServiceMethod = operatorType.Name,
                ElementType = Enumerators.DataType.ChildAssets,
                UseComplexValue = false,
                ReferencedAssetType = CreateTestAssetType(unitOfWork),
                Value = "1",
                StartBrackets = startBrackets,
                EndBrackets = endBrackets
            };
        }

        private AttributeElement CreateUserChildAssetsComplexCondition(IUnitOfWork unitOfWork, Type operatorType, string startBrackets = "", string endBrackets = "", ConcatenationOperation operation = ConcatenationOperation.And)
        {
            var complexValue = new List<AttributeElement>
            {
                CreateNameCondition(),
                CreateUpdateUserSimpleCondition(operation: ConcatenationOperation.Or)
            };

            return new AttributeElement
            {
                FieldName = "UpdateUser",
                FieldSql = AttributeNames.UpdateUserId,
                ConcatenationOperation = operation,
                ServiceMethod = operatorType.Name,
                ElementType = Enumerators.DataType.ChildAssets,
                UseComplexValue = true,
                ReferencedAssetType = CreateTestAssetType(unitOfWork),
                ComplexValue = complexValue,
                StartBrackets = startBrackets,
                EndBrackets = endBrackets
            };
        }

        // DynList condition
        private static AttributeElement CreateRoleCondition(string startBrackets = "", string endBrackets = "",
            ConcatenationOperation operation = ConcatenationOperation.And)
        {
            return new AttributeElement
            {
                FieldName = AttributeNames.Role,
                FieldSql = AttributeNames.Role,
                ConcatenationOperation = operation,
                ServiceMethod = "EqualOperator",
                ElementType = Enumerators.DataType.DynList,
                UseComplexValue = false,
                Value = "864",
                StartBrackets = startBrackets,
                EndBrackets = endBrackets
            };
        }

        private static string IgnoreWhitespace(string input)
        {
            return new string(input.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }
    }
}