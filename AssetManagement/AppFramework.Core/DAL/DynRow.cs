using AppFramework.ConstantsEnumerators;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.DataTypes;
using System.Collections.Generic;
using System.Linq;

namespace AppFramework.Core.DAL
{
    public class DynRow
    {
        /// <summary>
        /// Gets and sets table name
        /// </summary>
        public string TableName
        {
            get { return this._tableName; }
            set { this._tableName = value; }
        }

        /// <summary>
        /// Gets list of table columns
        /// </summary>
        public IReadOnlyCollection<DynColumn> Fields
        {
            get { return this._fields; }
        }

        private string _tableName;
        private List<DynColumn> _fields = new List<DynColumn>();

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="tableName">Name of table from which to retrieve the row</param>
        /// <param name="fields">List of fields to retrieve</param>
        public DynRow(string tableName, IEnumerable<DynColumn> fields)
        {
            _tableName = tableName;
            _fields.AddRange(fields);

            // hack to add fields which are not in the DynEntityAttribConfig table
            if (!_fields.Any(f => f.Name == AttributeNames.IsDeleted))
            {
                _fields.Add(new DynColumn(
                    AttributeNames.IsDeleted,
                    new BoolDataType(),
                    false,
                    false));
            }
        }

        /// <summary>
        /// Gets the <see cref="AppFramework.Core.DAL.DynColumn"/> at the specified index.
        /// </summary>
        /// <value></value>
        public DynColumn this[string index]
        {
            get
            {
                return this._fields.FirstOrDefault(f => f.Name == index);
            }
        }
    }
}
