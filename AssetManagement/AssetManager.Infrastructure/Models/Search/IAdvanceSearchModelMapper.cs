using System.Collections.Generic;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;

namespace AssetManager.Infrastructure.Models.Search
{
    public interface IAdvanceSearchModelMapper
    {
        /// <summary>
        /// Maps AdvanceSearchModel to business acceptable collection of AttributeElements
        /// </summary>
        List<AttributeElement> GetAttributeElements(AdvanceSearchModel searchModel);
    }
}