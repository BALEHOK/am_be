using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Entities;
using AssetManager.Infrastructure.Models.TypeModels;
using AssetManager.Infrastructure.Services;

namespace AssetManager.WebApi.Models.Search
{
    public class AdvanceSearchModelSearchQueryMapper : IAdvanceSearchModelSearchQueryMapper
    {
        private readonly IAssetTypeService _assetTypeService;

        public AdvanceSearchModelSearchQueryMapper(IAssetTypeService assetTypeService)
        {
            if (assetTypeService == null)
                throw new ArgumentNullException("assetTypeService");
            _assetTypeService = assetTypeService;
        }

        public SearchQuery GetSearchQuery(AdvanceSearchModel model)
        {
            Debug.Assert(model.SearchId != null, "model.SearchId == null");

            var searchQuery = new SearchQuery
            {
                AssetTypeId = model.AssetType.Id,
                Context = (byte) model.AssetTypeContext,
                SearchId = model.SearchId.Value,
                Created = DateTime.UtcNow
            };

            searchQuery.SearchQueryAttributes = AttributeFiltersToSearchQueryAttributes(model.Attributes);

            return searchQuery;
        }

        public AdvanceSearchModel GetAdvanceSearchModel(SearchQuery searchQuery)
        {
            var assetType = _assetTypeService.GetAssetType(searchQuery.AssetTypeId, true);

            return new AdvanceSearchModel
            {
                AssetType = new IdNamePair<long, string>(searchQuery.AssetTypeId, assetType.DisplayName),
                AssetTypeContext = (TimePeriodForSearch) searchQuery.Context,
                SearchId = searchQuery.SearchId,
                Attributes =
                    SearchQueryAttributesToAttributeFilters(searchQuery.SearchQueryAttributes, assetType.Attributes)
            };
        }

        private static TrackableCollection<SearchQueryAttribute> AttributeFiltersToSearchQueryAttributes(AttributeFilter[] attributeFilters)
        {
            var searchQueryAttributes = new TrackableCollection<SearchQueryAttribute>();
            var searchQueryAttributesCollection = (ICollection<SearchQueryAttribute>)searchQueryAttributes;

            foreach (var filter in attributeFilters)
            {
                var attribute = new SearchQueryAttribute
                {
                    Index = filter.Index,
                    LogicalOperator = (byte)filter.LogicalOperator
                };

                if (filter.Parenthesis != AttributeFilter.ParenthesisType.None)
                {
                    attribute.Parenthesis = (byte)filter.Parenthesis;
                }
                else
                {
                    attribute.OperatorId = filter.OperatorId;
                    attribute.ReferencedAttributeId = filter.ReferenceAttrib.Id;

                    if (filter.UseComplexValue)
                    {
                        attribute.ComplexValue = AttributeFiltersToSearchQueryAttributes(filter.ComplexValue);
                    }
                    else if (filter.Value != null)
                    {
                        attribute.ValueLabel = filter.Value.Name;
                        attribute.Value = filter.Value.Id;
                    }
                }

                searchQueryAttributesCollection.Add(attribute);
            }

            return searchQueryAttributes;
        }

        private AttributeFilter[] SearchQueryAttributesToAttributeFilters(ICollection<SearchQueryAttribute> searchQueryAttributes, List<AttributeTypeModel> assetTypeAtributes)
        {
            var attributeFilters = new AttributeFilter[searchQueryAttributes.Count];
            
            foreach (var attribute in searchQueryAttributes)
            {
                var filter = new AttributeFilter
                {
                    Index = attribute.Index,
                    LogicalOperator = (AttributeFilter.LogicalOperators)attribute.LogicalOperator
                };

                if (attribute.Parenthesis.GetValueOrDefault() > 0)
                {
                    filter.Parenthesis = (AttributeFilter.ParenthesisType) attribute.Parenthesis.GetValueOrDefault();
                }
                else
                {
                    filter.ReferenceAttrib = assetTypeAtributes.Single(a => a.Id == attribute.ReferencedAttributeId);

                    Debug.Assert(attribute.OperatorId != null, "attribute.OperatorId == null");
                    filter.OperatorId = attribute.OperatorId.Value;

                    if (attribute.ComplexValue != null && attribute.ComplexValue.Count > 0)
                    {
                        filter.UseComplexValue = true;

                        var referenceAssetTypeAttributes =
                            _assetTypeService.GetAssetType(filter.ReferenceAttrib.RelationId, true).Attributes;

                        filter.ComplexValue = SearchQueryAttributesToAttributeFilters(attribute.ComplexValue, referenceAssetTypeAttributes);
                    }
                    else
                    {
                        filter.Value = new IdNamePair<string, string>(attribute.Value, attribute.ValueLabel);
                    }
                }

                attributeFilters[filter.Index] = filter;
            }

            return attributeFilters;
        }
    }
}