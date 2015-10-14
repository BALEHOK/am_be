using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.Validation;
using System.Collections.Generic;
using AppFramework.Core.DataTypes;

namespace AppFramework.Core.Classes
{
    public interface IDataTypeService
    {
        /// <summary>
        /// Returns the instance of CustomDataType by type - from database
        /// </summary>
        /// <param name="t">Datatype</param>
        /// <returns>CustomDataType object</returns>        
        CustomDataType GetByType(Enumerators.DataType t);

        /// <summary>
        /// Returns DataType by its DB unique ID - from database
        /// </summary>
        /// <param name="uid">unique ID of datatype</param>
        /// <returns>CustomDataType object</returns>
        CustomDataType GetByUid(long uid);

        CustomDataType GetByName(string dataTypeName);

        /// <summary>
        /// Returns all custom data types
        /// </summary>
        /// <returns>List of DataTypeBase objects</returns>
        IEnumerable<CustomDataType> GetAll();
        
        void AssignSearchOperators(DataTypeBase dataType, List<long> assignedOperators);
    }
}