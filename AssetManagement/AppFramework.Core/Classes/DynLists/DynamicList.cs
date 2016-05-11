using AppFramework.Core.Classes.Extensions;

namespace AppFramework.Core.Classes.DynLists
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Xml.Serialization;
    using AppFramework.Entities;
    using System.Collections.Generic;

    /// <summary>
    /// Class representing dynamics list of values
    /// </summary>
    public class DynamicList
    {
        public long UpdateUserId
        {
            get { return _base.UpdateUserId; }
            set { _base.UpdateUserId = value; }
        }

        public DateTime UpdateDate
        {
            get { return _base.UpdateDate; }
            set { _base.UpdateDate = value; }
        }

        public long UID
        {
            get { return _base.DynListUid; }
            set { _base.DynListUid = value; }
        }

        public string Name
        {
            get { return _base.Name.Localized(); }
            set { _base.Name = value; }
        }
        
        public long DataTypeUid
        {
            get { return _base.DataTypeId; }
            set { _base.DataTypeId = value; }
        }

        public string Label
        {
            get { return _base.Label; }
            set { _base.Label = value; }
        }

        public string Comment
        {
            get { return _base.Comment; }
            set { _base.Comment = value; }
        }

        public bool Active
        {
            get { return _base.Active; }
            set { _base.Active = value; }
        }

        public IList<DynamicListItem> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new ObservableCollection<DynamicListItem>(
                        _base.DynListItems.OrderBy(i => i.DisplayOrder)
                            .Select(i => new DynamicListItem(i)));
                    _items.CollectionChanged += _items_CollectionChanged;
                }
                return _items;
            }
            set 
            { 
                _items = new ObservableCollection<DynamicListItem>(value);
                _items.CollectionChanged += _items_CollectionChanged;
            } // for unit-test purposes (yes, code smell)
        }
        private ObservableCollection<DynamicListItem> _items;

        [XmlIgnore]
        public DynList Base
        {
            get { return _base; }
        }

        private readonly DynList _base;

        public DynamicList()
            : this(new DynList())
        {

        }

        public DynamicList(Entities.DynList data)
        {
            if (data == null)
                throw new System.ArgumentNullException("DynList");
            _base = data;
            _base.StartTracking();
        }
        
        public DynamicListItem Parse(string value)
        {
            DynamicListItem result;
            if (!TryParse(value, out result))
            {
                throw new Exception(
                    string.Format("Cannot obtain item of list '{1}' by value '{0}'",
                        value, Name));
            }

            return result;
        }

        public bool TryParse(string value, out DynamicListItem listItem)
        {
            long id;
            bool isPossibleId = long.TryParse(value, out id);
            foreach (var dynamicListItem in Items)
            {
                //value can be Id or value can be value
                if ((isPossibleId && dynamicListItem.ID == id) || value == dynamicListItem.Value)
                {
                    listItem = dynamicListItem;
                    return true;
                }

                //search inner lists
                if (dynamicListItem.AssociatedDynList != null)
                {
                    return dynamicListItem.AssociatedDynList.TryParse(value, out listItem);
                }
            }

            listItem = null;
            return false;
        }

        void _items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (DynamicListItem item in e.NewItems)
                {
                    if (Base.DynListItems.All(a => !a.Equals(item.Base)))
                        Base.DynListItems.Add(item.Base);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (DynamicListItem item in e.OldItems)
                {
                    Base.DynListItems.Remove(item.Base);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                Base.DynListItems.Clear();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
