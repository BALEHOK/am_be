using Microsoft.Practices.EnterpriseLibrary.Common.Utility;

namespace AppFramework.Core.Classes
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using AppFramework.Core.AC.Authentication;
    using AppFramework.Core.Interfaces;
    using AppFramework.Entities;

    /// <summary>
    /// Taxonomies and categories management. Actually, categories and taxonomies are same entities in database, type determine flag IsCategory
    /// Every Get* method have not-required parameter - IsCategory. By default, it equal False - so by default all methods work with taxonomies, not categories
    /// </summary>
    public class Taxonomy : IRevision, IModification
    {
        /// <summary>
        /// Gets taxonomy unique ID
        /// </summary>
        public long UID
        {
            get { return this.Base.TaxonomyUid; }
        }

        /// <summary>
        /// Gets taxonomy ID
        /// </summary>
        public long ID
        {
            get { return this.Base.TaxonomyId; }
        }

        /// <summary>
        /// Gets and sets taxonomy name
        /// </summary>
        public string Name
        {
            get { return new TranslatableString(this.Base.Name).GetTranslation(); }
            set { this.Base.Name = value; }
        }

        /// <summary>
        /// Gets and sets taxonomy description
        /// </summary>
        public string Description
        {
            get { return this.Base.Description; }
            set { this.Base.Description = value; }
        }

        /// <summary>
        /// Gets and sets active flag
        /// </summary>
        public bool IsActive
        {
            get { return this.Base.IsActive; }
            set { this.Base.IsActive = value; }
        }

        /// <summary>
        /// Gets and sets the flag if this taxonomy is category
        /// </summary>
        public bool IsCategory
        {
            get { return this.Base.IsCategory; }
            set { this.Base.IsCategory = value; }
        }

        /// <summary>
        /// Gets and sets the flag if this taxonomy is draft
        /// </summary>
        public bool IsDraft
        {
            get { return this.Base.IsDraft; }
            set { this.Base.IsDraft = value; }
        }

        /// <summary>
        /// Gets and sets the flag if this version is active revision
        /// </summary>
        public bool IsActiveVersion
        {
            get { return this.Base.ActiveVersion; }
            set { this.Base.ActiveVersion = value; }
        }

        /// <summary>
        /// Gets and sets the revision number
        /// </summary>
        public int Revision
        {
            get { return this.Base.Revision; }
            set { this.Base.Revision = value; }
        }

        /// <summary>
        /// Gets the list of of taxonomy items
        /// </summary>
        public ObservableCollection<TaxonomyItem> Items { get; private set; }

        /// <summary>
        /// Gets the root nodes.
        /// </summary>
        /// <value>The root nodes.</value>
        public IEnumerable<TaxonomyItem> RootItems
        {
            get
            {
                return this.Items.Where(i => !i.ParentUid.HasValue).OrderBy(ti => ti.DisplayOrder);
            }
        }

        /// <summary>
        /// Gets the base DAL object
        /// </summary>
        public Entities.Taxonomy Base
        {
            get{return _base;}
        }

        private Entities.Taxonomy _base;

        ///// <summary>
        ///// Default class constructor
        ///// </summary>
        //public Taxonomy()
        //    : this(new Entities.Taxonomy())
        //{
        //    Revision = 1;
        //    IsActiveVersion = true;
        //    IsActive = true;
        //}

        /// <summary>
        /// Private constructor
        /// </summary>
        /// <param name="data"></param>
        public Taxonomy(Entities.Taxonomy data, IEnumerable<TaxonomyItem> items)
        {
            if (data == null)
                throw new ArgumentNullException("Taxonomy");
            _base = data;
            _base.StartTracking();
            if (items != null)
            {
                Items = new ObservableCollection<TaxonomyItem>(items);
                Items.CollectionChanged += _items_CollectionChanged;
            }
        }

        void _items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (TaxonomyItem item in e.NewItems)
                {
                    if (_base.TaxonomyItem.All(a => !a.Equals(item.Base)))
                    {
                        _base.TaxonomyItem.Add(item.Base);
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (TaxonomyItem item in e.OldItems)
                {
                    _base.TaxonomyItem.Remove(item.Base);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _base.TaxonomyItem.Clear();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
