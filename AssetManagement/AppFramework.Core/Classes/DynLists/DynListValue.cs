namespace AppFramework.Core.Classes.DynLists
{
    using System;
    using System.Xml.Serialization;
    using AppFramework.Entities;

    [Serializable]
    [Obsolete("To be removed")]
    public class DynamicListValue
    {
        public long DynamicListItemId { get; set; }

        private string _dynItemValue;
        /// <summary>
        /// Gets or sets the dynamic list id.
        /// </summary>
        /// <value>The dynamic list id.</value>  
        public long DynamicListUid
        {
            get { return _base.DynListUid; }
            set { _base.DynListUid = value; }
        }

        /// <summary>
        /// Gets or sets the dynamic list item id. Actually, it's not used - instead of it using Value field
        /// </summary>
        /// <value>The dynamic list item id.</value>  
        public long DynamicListItemUid
        {
            get { return _base.DynListItemUid; }
            set { _base.DynListItemUid = value; }
        }

        /// <summary>
        /// Gets or sets the parent list id.
        /// </summary>
        /// <value>The parent list id.</value>
        [XmlElement]
        public long ParentListId
        {
            get { return _base.ParentListId; }
            set { _base.ParentListId = value; }
        }

        /// <summary>
        /// Gets or sets the asset attribute.
        /// </summary>
        /// <value>The asset attribute.</value>
        [XmlIgnore]
        public AssetAttribute AssetAttribute
        {
            get
            {
                return _assetAttribute;
            }
            set
            {
                _assetAttribute = value;
            }
        }

        /// <summary>
        /// Gets or sets the display order.
        /// </summary>
        /// <value>The display order.</value>
        [XmlIgnore]
        public long DisplayOrder
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the UID.
        /// </summary>
        /// <value>The UID.</value>
        [XmlIgnore]
        public long DynListValueUid
        {
            get { return _base.DynListValueUid; }
            set { _base.DynListValueUid = value; }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [XmlElement]
        public string Value
        {
            get { return _base.Value; }
            set { _base.Value = value; }
        }

        [XmlIgnore]
        public Entities.DynListValue Base
        {
            get { return _base; }
        }

        private Entities.DynListValue _base;
        private DynLists.DynamicList _dynamicList;
        private Classes.AssetAttribute _assetAttribute;

        public DynamicListValue()
            : this(new Entities.DynListValue())
        {

        }

        public DynamicListValue(Entities.DynListValue data)
        {
            if (data == null)
                throw new ArgumentNullException("DynListValue");
            _base = data;
            _base.StartTracking();
        }
    }
}
