
using AppFramework.ConstantsEnumerators;
using AppFramework.Entities;
using System;
using System.Xml.Serialization;

namespace AppFramework.Core.DataTypes
{
    [Serializable]
    public class CustomDataType : DataTypeBase
    {
        /// <summary>
        /// DB ID of this datatype
        /// </summary>
        public long UID
        {
            get { return _base.DataTypeUid; }
        }

        /// <summary>
        /// Datatype name
        /// </summary>
        public override string Name
        {
            get { return _base.Name; }
        }

        /// <summary>
        /// C# DataType
        /// </summary>
        public override Type FrameworkDataType
        {
            get 
            { 
                return _base.FrameworkDataType != null 
                    ? Type.GetType(_base.FrameworkDataType, false, true)
                    : null;
            }
        }

        /// <summary>
        /// Gets dataType comment
        /// </summary>
        public string Comment
        {
            get { return _base.Comment; }
            set { _base.Comment = value; }
        }

        /// <summary>
        /// Gets and sets the size of varchar datatype
        /// </summary>
        public override int? StringSize
        {
            get { return _base.StringSize; }
        }

        /// <summary>
        /// Gets the code of datatype
        /// Setter only for unit testing purposes (code smell indeed)
        /// </summary>
        public override Enumerators.DataType Code
        {
            get { return this._dataTypeCode; }
            set { this._dataTypeCode = value; }
        }

        /// <summary>
        /// Gets if this datatype can be editable by user
        /// </summary>
        public override bool Editable
        {
            get { return _base.IsEditable; }
            set { _base.IsEditable = value; }
        }

        /// <summary>
        /// Gets if this datatype is for internal use or it can be displayed in UI
        /// </summary>
        public override bool IsInternal
        {
            get { return _base.IsInternal; }
            set { _base.IsInternal = value; }
        }

        public string ValidationExpr
        {
            get { return _base.ValidationExpr; }
            set { _base.ValidationExpr = value; }
        }

        public string ValidationMessage
        {
            get { return _base.ValidationMessage; }
            set { _base.ValidationMessage = value; }
        }

        /// <summary>
        /// Gets the base DAL object
        /// </summary>
        [XmlIgnore]
        public override DataType Base
        {
            get { return _base; }
        }

        private readonly DataType _base;

        public CustomDataType()
        {
            _base = new DataType();
        }
        
        /// <summary>
        /// Class constructor with properties initialization by data
        /// </summary>
        /// <param name="data"></param>
        public CustomDataType(DataType data)
        {
            if (data == null)
                throw new ArgumentNullException("DataType");
            _base = data;
            _base.StartTracking();
            _dataTypeCode = (Enumerators.DataType)Enum.Parse(typeof(Enumerators.DataType), data.Name, true);
        }
    }
}
