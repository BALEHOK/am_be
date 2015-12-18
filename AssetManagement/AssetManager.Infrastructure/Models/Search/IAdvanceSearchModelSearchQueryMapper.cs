using AppFramework.Entities;

namespace AssetManager.Infrastructure.Models.Search
{
    public interface IAdvanceSearchModelSearchQueryMapper
    {
        SearchQuery GetSearchQuery(AdvanceSearchModel model);
        AdvanceSearchModel GetAdvanceSearchModel(SearchQuery searchQuery);
    }
}