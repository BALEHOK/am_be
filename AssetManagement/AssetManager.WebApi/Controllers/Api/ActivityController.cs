using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.SearchEngine;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Core.Classes.SearchEngine.SearchOperators;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Entities;
using AssetManager.Infrastructure.Extensions;
using AssetManager.Infrastructure.Models.Search;
using WebApi.OutputCache.V2;

namespace AssetManager.WebApi.Controllers.Api
{
    [RoutePrefix("api/activity")]
    public class ActivityController : ApiController
    {
        private readonly ISearchService _searchService;
        private readonly ISearchResultMapper _searchResultMapper;
        private readonly IAssetTypeRepository _assetTypeRepository;

        public ActivityController(
            ISearchService searchService,
            ISearchResultMapper searchResultMapper,
            IAssetTypeRepository assetTypeRepository)
        {
            _searchService = searchService;
            _searchResultMapper = searchResultMapper;
            _assetTypeRepository = assetTypeRepository;
        }

        /// <summary>
        /// Performs search for open activities
        /// </summary>
        [Route("open/my"), HttpPost]
        [CacheOutput(ClientTimeSpan = 120, ServerTimeSpan = 120)]
        public SearchResultModel MyOpenActivities([FromBody]string dueDateStr)
        {
            var userId = User.GetId();

            var activityAssetType = _assetTypeRepository.GetPredefinedAssetType(PredefinedEntity.Activity);
            var attributeElements = CreateActivitiesFilter(dueDateStr, activityAssetType, userId);

            var searchId = Guid.NewGuid();
            var result = _searchService.FindByType(
                searchId,
                userId,
                activityAssetType.ID,
                attributeElements,
                time: TimePeriodForSearch.CurrentTime,
                order: Enumerations.SearchOrder.Date,
                pageSize: 50);

            return _searchResultMapper.GetSearchResultModel(searchId, result);
        }

        private static string _assetInOperator = typeof(AssetInOperator).Name;
        private static string _notEqualOperator = typeof(NotEqualOperator).Name;
        private static string _lessEqualOperator = typeof(LessEqualOperator).Name;
        private static List<AttributeElement> CreateActivitiesFilter(string dueDateStr, AssetType activityAssetType, long userId)
        {
            var attributeElements = new List<AttributeElement>(3);

            // assignee
            var attribute = activityAssetType[AttributeNames.Assignee];
            attributeElements.Add(
                CreateAttributeFilterElement(attribute, _assetInOperator, userId.ToString()));

            // status
            attribute = activityAssetType[AttributeNames.Status];
            var completedStatus =
                attribute.DynamicList.Items
                    .Single(i => string.Equals(i.Value, ApplicationSettings.ActivityCompletedStatus));
            attributeElements.Add(
                CreateAttributeFilterElement(attribute, _notEqualOperator, completedStatus.ID.ToString()));

            // due date
            if (!string.IsNullOrEmpty(dueDateStr))
            {
                var dueDate = DateTime.Parse(dueDateStr);

                // need all the activities of the day
                dueDate = new DateTime(dueDate.Year, dueDate.Month, dueDate.Day, 23, 59, 59);

                attribute = activityAssetType[AttributeNames.DueDate];

                attributeElements.Add(
                    CreateAttributeFilterElement(attribute, _lessEqualOperator, dueDate.ToString()));
            }
            return attributeElements;
        }

        private static AttributeElement CreateAttributeFilterElement(AssetTypeAttribute attribute, string operatorType, string value)
        {
            return new AttributeElement
            {
                FieldName = attribute.Name,
                FieldSql = attribute.DBTableFieldName,
                ServiceMethod = operatorType,
                ElementType = attribute.DataTypeEnum,
                DateType = attribute.DataTypeEnum,
                UseComplexValue = false,
                Value = value,
                ConcatenationOperation = ConcatenationOperation.And
            };
        }
    }
}