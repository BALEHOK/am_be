using AppFramework.DataProxy;

namespace AppFramework.Core.Classes
{
    using AppFramework.Core.Helpers;
    using AppFramework.Entities;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Xml.Serialization;

    [Serializable()]
    [Obsolete("use AttributePanel instead")]
    public class Panel
    {
        /// <summary>
        /// Gets the unique ID of panel
        /// </summary>
        public long UID
        {
            get
            {
                if (_panelIdentifier == 0)
                {
                    if (_base.AttributePanelUid > 0)
                    {
                        _panelIdentifier = _base.AttributePanelUid;
                    }
                    else
                    {
                        _panelIdentifier = Randomization.GetIdentifier();
                    }
                }
                return _panelIdentifier;
            }
        }

        public long ID
        {
            get { return _base.AttributePanelId; }
        }

        /// <summary>
        /// Gets and sets the name of panel
        /// </summary>
        public string Name
        {
            get { return _base.Name; }
            set { _base.Name = value; }
        }

        /// <summary>
        /// Gets and sets the order in layout
        /// </summary>
        public byte DisplayOrder
        {
            get { return _base.DisplayOrder; }
            set { _base.DisplayOrder = value; }
        }

        public bool IsChildPanel
        {
            get { return _base.IsChildAssets; }
            set { _base.IsChildAssets = value; }
        }

        public long ChildAssetAttrId
        {
            get { return _base.ChildAssetAttrId.GetValueOrDefault(); }
            set { _base.ChildAssetAttrId = value; }
        }

        /// <summary>
        /// Gets and sets panel description
        /// </summary>
        public string Description
        {
            get { return _base.Description; }
            set
            {
                _base.Description = value.Length > 250 ? value.Substring(0, 250).Trim() : value;
            }
        }

        /// <summary>
        /// Gets and sets panel header label
        /// </summary>
        public string HeaderLabel
        {
            get { return _base.HeaderLabel; }
            set { _base.HeaderLabel = value; }
        }

        public long AssetTypeUID
        {
            get { return _base.DynEntityConfigUId; }
            set { _base.DynEntityConfigUId = value; }
        }

        public long? ScreenId
        {
            get { return _base.ScreenId; }
            set { _base.ScreenId = value; }
        }

        /// <summary>
        /// Gets and sets list of AssetType attributes assigned to this panel
        /// </summary>
        [XmlArray]
        [XmlArrayItem("AssignedAttributes", typeof(AssetTypeAttribute))]
        public List<AssetTypeAttribute> AssignedAttributes { get; set; }

        /// <summary>
        /// Gets the base DAL object
        /// </summary>
        [XmlIgnore]
        public AttributePanel Base
        {
            get
            {
                return _base;
            }
        }

        private readonly AttributePanel _base;
        private long _panelIdentifier;

        /// <summary>
        /// Constructor for (de)serialization
        /// </summary>
        public Panel()
            :this(new AttributePanel(), null)
        {
            
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        /// <param name="data">AttributePanel table record</param>
        /// <param name="attributes">Collection of assigned attributes</param>
        public Panel(AttributePanel data, IEnumerable<AssetTypeAttribute> attributes)
        {
            if (data == null)
                throw new ArgumentNullException();
            AssignedAttributes = attributes != null
                ? attributes.ToList()
                : new List<AssetTypeAttribute>();
            _base = data;
            _base.StartTracking();
        }

        public void SetAttributeDisplayOrder(AssetTypeAttribute attribute, int order)
        {
            var apa = _base.AttributePanelAttribute
                .SingleOrDefault(item => item.DynEntityAttribConfig.DBTableFieldname 
                    == attribute.DBTableFieldName);
            if (apa != null)
                apa.DisplayOrder = order;
        }

        public int GetAttributeDisplayOrder(AssetTypeAttribute attribute)
        {
            var apa = GetAttributePanelAttribute(attribute);
            return apa != null ? apa.DisplayOrder : default(int);
        }

        public string GetAttributeScreenFormula(AssetTypeAttribute attribute)
        {
            var apa = GetAttributePanelAttribute(attribute);
            return apa != null ? apa.ScreenFormula : null;
        }

        private AttributePanelAttribute GetAttributePanelAttribute(AssetTypeAttribute attribute)
        {
            return _base.AttributePanelAttribute.SingleOrDefault(
                    item => item.DynEntityAttribConfig.DBTableFieldname 
                        == attribute.DBTableFieldName 
                        && item.DynEntityAttribConfigUId == attribute.UID);
        }

        public void RemoveAssetTypeAttribute(AssetTypeAttribute attribute)
        {
            // remove element
            Base.AttributePanelAttribute.Remove(
                Base.AttributePanelAttribute.Single(apa => apa.DynEntityAttribConfigUId == attribute.UID));
            // reindex display orders
            Base.AttributePanelAttribute.ForEachWithIndex((a, index) => { a.DisplayOrder = index; });
            AssignedAttributes.Remove(attribute);
        }

        public void RemoveAssignedAttributes()
        {
            AssignedAttributes.Clear();
            Base.AttributePanelAttribute.Clear();
        }

        public void AssignAssetTypeAttribute(AssetTypeAttribute attribute)
        {
            int order = Base.AttributePanelAttribute.Count > 0
                ? Base.AttributePanelAttribute.Last().DisplayOrder
                : 0;
            if (Base.AttributePanelAttribute.All(apa => !apa.DynEntityAttribConfig.Equals(attribute.Base)))
            {
                AssignedAttributes.Add(attribute);
                Base.AttributePanelAttribute.Add(new AttributePanelAttribute()
                {
                    UpdateDate = DateTime.Now,
                    DynEntityAttribConfig = attribute.Base,
                    DisplayOrder = ++order
                });
            }
        }
    }
}
