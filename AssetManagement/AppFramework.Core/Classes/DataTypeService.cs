using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.Caching;
using AppFramework.Core.Classes.Validation;
using AppFramework.Core.Classes.Validation.Expression;
using AppFramework.Core.DataTypes;
using AppFramework.Core.Interceptors;
using AppFramework.DataProxy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppFramework.Core.Classes
{
    public class DataTypeService : IDataTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DataTypeService(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new NullReferenceException("IUnitOfWork");
            _unitOfWork = unitOfWork;
        }

        public CustomDataType GetByType(Enumerators.DataType t)
        {
            var name = t.ToString().ToLower();
            var data = _unitOfWork.DataTypeRepository.Single(dt => dt.Name == name);
            return new CustomDataType(data);
        }

        public CustomDataType GetByUid(long uid)
        {
            var data = _unitOfWork.DataTypeRepository.SingleOrDefault(dt => dt.DataTypeUid == uid);
            if (data == null)
                throw new ArgumentException();
            return new CustomDataType(data);
        }

        public CustomDataType GetByName(string dataTypeName)
        {
            var data = _unitOfWork.DataTypeRepository.Single(dt => dt.Name == dataTypeName);
            return new CustomDataType(data);
        }

        public IEnumerable<CustomDataType> GetAll()
        {
            var dataitems = _unitOfWork.DataTypeRepository.Get(orderBy: items => items.OrderBy(i => i.Name));
            foreach (var dataType in dataitems)
            {
                yield return new CustomDataType(dataType);
            }
        }
        
        [Transaction]
        public void AssignSearchOperators(DataTypeBase dataType, List<long> assignedOperators)
        {
            _unitOfWork.SqlProvider.ExecuteNonQuery(
                string.Format("DELETE FROM DataTypeSearchOperators WHERE DataTypeUid={0}", 
                dataType.Base.DataTypeUid));
            foreach (var so in assignedOperators)
                _unitOfWork.SqlProvider.ExecuteNonQuery(
                    string.Format("INSERT INTO DataTypeSearchOperators (DataTypeUid, SearchOperatorUid) " +
                                  "VALUES({0},{1});", 
                    dataType.Base.DataTypeUid, so));
            Cache<CustomDataType>.Flush();
        }

        /// <summary>
        /// Description of data type for SQL statements
        /// </summary>
        public string ConvertToDbDataType(DataTypeBase customDataType)
        {
            if (customDataType.Code == Enumerators.DataType.String)
                return String.Format("{0} (max) ", customDataType.Base.DBDataType);
            if (customDataType.StringSize > 0)
                return customDataType.Base.DBDataType + "(" + Convert.ToString(customDataType.StringSize) + ")";
            return customDataType.Base.DBDataType;
        }
    }
}