using AppFramework.ConstantsEnumerators;
using AppFramework.Entities;
using System;
using System.Xml.Serialization;

namespace AppFramework.Core.DataTypes
{
    public abstract class DataTypeBase
    {
        protected Enumerators.DataType _dataTypeCode;

        /// <summary>
        /// Datatype name
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// C# DataType
        /// </summary>
        public abstract Type FrameworkDataType { get; }

        /// <summary>
        /// Gets dataType comment
        /// </summary>
        public virtual string Comment { get; set; }

        /// <summary>
        /// Gets and sets the size of varchar datatype
        /// </summary>
        public abstract int? StringSize { get; }

        /// <summary>
        /// Gets the code of datatype
        /// Setter only for unit testing purposes (code smell)
        /// </summary>
        public abstract Enumerators.DataType Code { get; set; }

        /// <summary>
        /// Gets if this datatype can be editable by user
        /// </summary>
        public virtual bool Editable { get; set; }

        /// <summary>
        /// Gets if this datatype is for internal use or it can be displayed in UI
        /// </summary>
        public virtual bool IsInternal { get; set; }

        public virtual string ValidationExpr { get; set; }

        public virtual string ValidationMessage { get; set; }

        [XmlIgnore]
        public virtual DataType Base { get; private set; }

        public DataTypeBase()
        {
            Base = new DataType();
        }
    }
}