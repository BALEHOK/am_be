using AppFramework.Entities;

namespace AssetManager.WebApi.Models.Search
{
    public interface IAdvanceSearchModelSearchQueryMapper
    {
        SearchQuery GetSearchQuery(AdvanceSearchModel model);
        AdvanceSearchModel GetAdvanceSearchModel(SearchQuery searchQuery);
    }
}