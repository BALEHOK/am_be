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
        public List<DynColumn> Fields
        {
            get { return this._fields; }
        }

        private string _tableName;
        private List<DynColumn> _fields = new List<DynColumn>();

        /// <summary>
        /// Default constructor is providing default columns
        /// </summary>
        public DynRow() { }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="tableName">Name of table from which to retrieve the row</param>
        /// <param name="fields">List of fields to retrieve</param>
        public DynRow(string tableName, List<DynColumn> fields)
            : this()
        {
            this._tableName = tableName;
            this._fields.AddRange(fields);
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
