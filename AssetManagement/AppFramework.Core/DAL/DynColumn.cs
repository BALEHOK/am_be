using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using System;
using AppFramework.Core.DataTypes;

namespace AppFramework.Core.DAL
{
    public class DynColumn
    {
        /// <summary>
        /// Gets and sets column name
        /// </summary>
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        /// <summary>
        /// Gets and sets column data type
        /// </summary>
        public DataTypeBase DataType
        {
            get { return this._dataType; }
            set { this._dataType = value; }
        }

        /// <summary>
        /// Gets and sets if column can be null
        /// </summary>
        public bool IsNull
        {
            get { return this._isNull; }
            set { this._isNull = value; }
        }

        /// <summary>
        /// Gets and sets data value in result rowset
        /// </summary>
        public object Value
        {
            get { return this._value; }
            set { this._value = value; }
        }

        /// <summary>
        /// Gets or sets if this field is autoincrement
        /// </summary>
        public bool IsIdentity
        {
            get { return this._isIdentity; }
            set { this._isIdentity = value; }
        }

        /// <summary>
        /// Indicates if this column must contain unique values within different revisions
        /// </summary>
        public bool IsUnique { get; set; }


        private string _name = string.Empty;
        private object _value = string.Empty;
        private DataTypeBase _dataType;
        private bool _isNull;
        private bool _isIdentity;

        public DynColumn() { }

        /// <param name="name">name of column to create</param>
        /// <param name="dataType">data type object which describes the column </param>
        /// <param name="size">size of column's data type </param>
        /// <param name="isNull">Can column be null</param>
        /// <param name="isPrimary">Column as primary key</param>
        public DynColumn(string name, DataTypeBase dataType, bool isNull, bool isIdentity)
        {
            _name = name;
            _dataType = dataType;
            _isNull = isNull;
            _isIdentity = isIdentity;
        }

        /// <summary>
        /// Returns the default value for curent CustomDataType
        /// </summary>
        /// <returns></returns>
        public string GetDefaultValue()
        {
            string returnValue = String.Empty;
            switch (DataType.Code)
            {
                case Enumerators.DataType.Int:
                case Enumerators.DataType.Long:
                case Enumerators.DataType.Bool:
                case Enumerators.DataType.Asset:
                case Enumerators.DataType.Assets:
                case Enumerators.DataType.DynList:
                case Enumerators.DataType.DynLists:
                case Enumerators.DataType.Money:
                case Enumerators.DataType.USD:
                case Enumerators.DataType.Euro:
                case Enumerators.DataType.Float:
                    returnValue = default(int).ToString();
                    break;
                default:
                    returnValue = "''";
                    break;
            }
            return returnValue;
        }

        public override string ToString()
        {
            return Name;
        }
    }    
}
