using System;
using System.Collections.Generic;
using AppFramework.DataProxy;
using AppFramework.Entities;

namespace AssetManager.Infrastructure.Services
{
    public class DataTypeService : IDataTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DataTypeService(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<SearchOperators> GetDataTypeOperators(string typeName)
        {
            var datatype = _unitOfWork.DataTypeRepository.Single(d => d.Name == typeName, dt => dt.SearchOperators);

            return datatype.SearchOperators;
        }
    }
}