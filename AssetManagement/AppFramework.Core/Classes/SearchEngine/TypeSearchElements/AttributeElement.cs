using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
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

        public ConcatenationOperation ConcatenationOperation
        {
            get { return AndOr == 0 ? ConcatenationOperation.And : ConcatenationOperation.Or; }

            set { AndOr = (int) value; }
        }

        /// <summary>
        /// Returns if current search chain is of complext type (multipleassets, dynlist(s))
        /// </summary>
        public bool IsComplex
        {
            get
            {
                return (ElementType == Enumerators.DataType.Asset && UseComplexValue)
                       || ElementType == Enumerators.DataType.Assets
                       || ElementType == Enumerators.DataType.DynList
                       || ElementType == Enumerators.DataType.DynLists;
            }
        }

        /// <summary>
        /// Which value of element to use: simple or complex
        /// </summary>
        public bool UseComplexValue { get; set; }

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

        public AssetType ReferencedAssetType { get; set; }
        public List<AttributeElement> ComplexValue { get; set; }

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
    }
}