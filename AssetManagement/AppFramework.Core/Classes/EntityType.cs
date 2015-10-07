namespace AppFramework.Core.Classes
{
    using System;
    using System.Collections.Generic;
    using AppFramework.Entities;

    [Serializable]
    public class EntityType
    {
        /// <summary>
        /// Basic DAL object. Implementation of proxy.
        /// </summary>
        private DynEntityType _baseType;

        /// <summary>
        /// Gets the base DAL object
        /// </summary>
        public DynEntityType Base
        {
            get
            {
                return _baseType;
            }
        }

        /// <summary>
        /// DB ID of entity type
        /// </summary>
        public int ID
        {
            get
            {
                return _baseType.DynEntityTypeId;
            }
            set
            {
                _baseType.DynEntityTypeId = value;
            }
        }

        /// <summary>
        /// Name of entity type
        /// </summary>
        public string Name
        {
            get
            {
                return _baseType.DynEntityTypeName;
            }
            set
            {
                _baseType.DynEntityTypeName = value;
            }
        }

        /// <summary>
        /// Description of entity type
        /// </summary>
        public string Description
        {
            get
            {
                return _baseType.DynEntityTypeDescription;
            }
            set
            {
                _baseType.DynEntityTypeDescription = value;
            }
        }

        /// <summary>
        /// Class constructor.
        /// Describes entity type: concrete, abstract, etc...
        /// </summary>
        public EntityType()
            : this(new DynEntityType()) { }

        /// <summary>
        /// Class constructor with properties initialization by provided data.
        /// </summary>
        /// <param name="data">DynEntityType table data</param>
        public EntityType(DynEntityType data)
        {
            this._baseType = data;
        }

        /// <summary>
        /// Returns DynEntityType by its DB ID
        /// </summary>
        /// <param name="id">ID of DynEntityType table record</param>
        /// <returns>EntityType</returns>
        public static EntityType GetByID(int id)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            return new EntityType(unitOfWork.DynEntityTypeRepository.Single(d => d.DynEntityTypeId == id));
        }

        /// <summary>
        /// Returns list of all Entity Types
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<EntityType> GetAll()
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            foreach (DynEntityType record in unitOfWork.DynEntityTypeRepository.Get())
            {
                yield return new EntityType(record);
            }
        }
    }
}
