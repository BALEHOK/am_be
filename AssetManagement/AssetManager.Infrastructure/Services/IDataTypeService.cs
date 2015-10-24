using System.Collections.Generic;
using AppFramework.Entities;

namespace AssetManager.Infrastructure.Services
{
    public interface IDataTypeService
    {
        IEnumerable<SearchOperators>  GetDataTypeOperators(string typeName);
    }
}