using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.Core.Interfaces;
using AppFramework.Entities;

namespace AppFramework.Core.Classes
{
    public class TaxonomyItem : IRevision, IModification
    {
        /// <summary>
        /// Gets the unique item ID
        /// </summary>
        public long Uid
        {
            get { return _base.TaxonomyItemUid; }
        }

        /// <summary>
        /// Gets item ID
        /// </summary>
        public long Id
        {
            get { return _base.TaxonomyItemId; }
            set { _base.TaxonomyItemId = value; }
        }

        /// <summary>
        /// Gets unique ID of taxonomy which item belongs to
        /// </summary>
        public long TaxonomyUid
        {
            get { return _base.TaxonomyUid; }
        }

        /// <summary>
        /// Gets and sets name
        /// </summary>
        public string Name
        {
            get { return new TranslatableString(_base.Name).GetTranslation(); }
            set { _base.Name = value; }
        }

        /// <summary>
        /// Gets and sets display order
        /// </summary>
        public short DisplayOrder
        {
            get { return _base.DisplayOrder; }
            set { _base.DisplayOrder = value; }
        }

        /// <summary>
        /// Gets and sets ID of parent item
        /// </summary>
        public long? ParentUid
        {
            get { return _base.ParentTaxonomyItemUid; }
            set { _base.ParentTaxonomyItemUid = value; }
        }

        public ObservableCollection<TaxonomyItem> ChildItems
        {
            get
            {
                if (_childItems == null)
                {
                    _childItems = new ObservableCollection<TaxonomyItem>((from dataitem in _base.ChildItems
                        select new TaxonomyItem(dataitem))
                        .OrderBy(ti => ti.DisplayOrder));
                    _childItems.CollectionChanged += _childItems_CollectionChanged;
                }
                return _childItems;
            }
        }

        private ObservableCollection<TaxonomyItem> _childItems;

        /// <summary>
        /// Gets and sets number to display in hierarchy
        /// </summary>
        public string Number
        {
            get { return _base.Number; }
            set { _base.Number = value; }
        }

        public bool ActiveVersion
        {
            get { return _base.ActiveVersion; }
            set { _base.ActiveVersion = value; }
        }

        public Entities.Taxonomy Taxonomy
        {
            get { return _base.Taxonomy; }
        }

        public Entities.TaxonomyItem Base
        {
            get { return _base; }
        }

        private readonly Entities.TaxonomyItem _base;

        public TaxonomyItem(Entities.TaxonomyItem data)

        {
            if (data == null)
                throw new ArgumentNullException("TaxonomyItem");
            if (data.Taxonomy == null)
                throw new ArgumentNullException("TaxonomyItem.Taxonomy");
            _base = data;
            _base.StartTracking();
        }

        /// <summary>
        /// Convert TaxonomyItem to TreeNode
        /// By default, not display Manage link for leafs when converting
        /// </summary>
        /// <returns></returns>
        public TreeNode ToTreeNode()
        {
            return ToTreeNode(false, false);
        }

        /// <summary>
        /// Convert TaxonomyItem to TreeNode
        /// </summary>
        /// <returns></returns>
        public TreeNode ToTreeNode(bool ShowAddNodeLink, bool ShowManageLink)
        {
            var node = new TreeNode
            {
                Text = Name,
                Value = Id.ToString()
            };
            if (ShowAddNodeLink)
            {
                node.Text += string.Format("&nbsp;<a href='#' onclick=\"return AddNode({0},'{1}')\">Add node</a>", Uid,
                    Name);
            }
            if (ShowManageLink)
            {
                node.Text += string.Format(" / <a href='{1}?Uid={0}'>Manage types</a>", Uid,
                    (new Control()).ResolveUrl("~/admin/Taxonomies/ManageTypes.aspx"));
            }

            foreach (var ch in ChildItems)
            {
                node.ChildNodes.Add(ch.ToTreeNode(ShowAddNodeLink, ShowManageLink));
            }

            return node;
        }

        void _childItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (TaxonomyItem item in e.NewItems)
                {
                    if (_base.ChildItems.All(a => !a.Equals(item.Base)))
                    {
                        _base.ChildItems.Add(item.Base);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (TaxonomyItem item in e.OldItems)
                {
                    _base.ChildItems.Remove(item.Base);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _base.ChildItems.Clear();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}