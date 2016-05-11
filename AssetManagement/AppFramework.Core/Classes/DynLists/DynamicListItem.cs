using AppFramework.Core.Classes.Extensions;

namespace AppFramework.Core.Classes.DynLists
{
    using System.Xml.Serialization;
    using AppFramework.Entities;

    public class DynamicListItem
    {
        public long? AssociatedDynListUID { private set; get; }

        public long ID
        {
            get { return _base.DynListItemId; }
        }

        public long UID
        {
            get { return _base.DynListItemUid; }
        }

        public int Revision
        {
            get { return _base.Revision; }
        }

        public bool ActiveVersion
        {
            get { return _base.ActiveVersion; }
            set { _base.ActiveVersion = value; }
        }

        public int DisplayOrder
        {
            get { return _base.DisplayOrder; }
            set { _base.DisplayOrder = value; }
        }

        public string OriginalValue
        {
            get
            {
                return _base.Value;
            }
        }

        public string Value
        {
            get { return _base.Value.Localized(); }
            set { _base.Value = value; }
        }

        public DynamicList ParentDynList
        {
            get
            {
                if (_parentDynList == null)
                {
                    _parentDynList = new DynamicList(_base.DynList);
                }
                return _parentDynList;
            }
            set
            {
                _base.DynListUid = value.UID;
                _parentDynList = value;
            }
        }

        public DynamicList AssociatedDynList
        {
            get
            {
                if (_associatedList == null && _base.AssociatedDynListUid.HasValue)
                {
                    _associatedList = new DynamicList(_base.AssociatedDynList);
                }
                return _associatedList;
            }
            set
            {
                _base.AssociatedDynListUid = null;
                if (value != null)
                {
                    _base.AssociatedDynListUid = value.Base.DynListUid;
                }
            }
        }

        [XmlIgnore]
        public Entities.DynListItem Base
        {
            get { return _base; }
        }

        private DynamicList _parentDynList;
        private DynListItem _base;
        private DynamicList _associatedList;

        public DynamicListItem()
            : this(new DynListItem())
        {
            _base.Revision = 1;
            _base.ActiveVersion = true;
        }

        public DynamicListItem(DynListItem data)
        {
            if (data == null)
                throw new System.ArgumentNullException("DynListItem");
            _base = data;
            _base.StartTracking();
        }
    }
}
