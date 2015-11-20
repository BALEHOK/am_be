using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.SearchEngine.SearchOperators;
using Newtonsoft.Json;
using Formatting = AppFramework.Core.Helpers.Formatting;

namespace AppFramework.Core.Classes.SearchEngine.TypeSearchElements
{
    [DataContract]
    [Serializable]
    public class AttributeElement
    {
        [DataMember]
        public int AscDesc { get; set; }

        [DataMember]
        public int AndOr { get; set; }

        [DataMember]
        public int AttributeId { get; set; }

        [DataMember]
        public int OperatorId { get; set; }

        [DataMember]
        public int ItemId { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string StartBrackets { get; set; }

        [DataMember]
        public string EndBrackets { get; set; }

        [DataMember]
        public Enumerators.DataType DateType { get; set; }

        [DataMember]
        public long DynListId { get; set; }

        [DataMember]
        public long DynListItemId { get; set; }

        [DataMember]
        [JsonIgnore]
        public AssetAttribute AssetAttribute { get; set; }

        public Enumerations.ConcatenationOperation ConcatenationOperation
        {
            get
            {
                return AndOr == 0 ? Enumerations.ConcatenationOperation.And : Enumerations.ConcatenationOperation.Or;
            }

            set
            {
                AndOr = (int) value;
            }
        }

        /// <summary>
        /// Returns if current search chain is of complext type (multipleassets, dynlist(s))
        /// </summary>
        public bool IsComplex
        {
            get
            {
                return (ElementType == Enumerators.DataType.Asset && ComplexValue != null)
                    || ElementType == Enumerators.DataType.Assets
                    || ElementType == Enumerators.DataType.DynList
                    || ElementType == Enumerators.DataType.DynLists;
            }
        }

        [DataMember]
        public string ServiceMethod { get; set; }

        [DataMember]
        public string FieldName { get; set; }

        [DataMember]
        public string FieldSql { get; set; }

        [DataMember]
        public string Value
        {
            get { return _value; }
            set { _value = Formatting.Escape(value); }
        }
        private string _value;

        public AttributeElementCoplexValue ComplexValue { get; set; }

        [DataMember]
        public long ContextUID { get; set; }

        [DataMember]
        public Enumerators.DataType ElementType { get; set; }

        public AttributeElement()
        {
            AttributeId = 0;
            OperatorId = 0;
            ItemId = -1;
            Text = "";
            AscDesc = 0;
            AndOr = 0;
        }

        /// <summary>
        /// Returns the SQL statement for current chain
        /// </summary>
        /// <param name="tableName">Table which contains searching value</param>
        /// <returns></returns>
        public SearchTerm GetSearchTerm(string tableName)
        {
            if (string.IsNullOrEmpty(FieldSql) && !string.IsNullOrEmpty(FieldName))
                FieldSql = FieldName;
            else if (string.IsNullOrEmpty(FieldSql) && string.IsNullOrEmpty(FieldName))
                throw new ArgumentException("Chain properties not set");

            string fieldName = string.Empty;

            //if field sql contains additional operations TODO:Refactor
            if (this.FieldSql.Contains("("))
            {
                fieldName = this.FieldSql.Replace(this.FieldName, "[" + tableName + "].[" + this.FieldName + "]");
            }
            else
            {
                // convert field name to the full name within table context
                fieldName = this.FieldSql.Contains("].[") ? this.FieldSql :
                    string.Format("[{0}].[{1}]", tableName, this.FieldSql.Trim(new char[] { '[', ']' }));
            }

            return BaseOperator.GetOperatorByClassName(this.ServiceMethod).
                    Generate(this.Value, fieldName);
        }
    }
}
