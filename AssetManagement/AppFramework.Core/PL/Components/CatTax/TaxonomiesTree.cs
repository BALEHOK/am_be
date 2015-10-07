using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes;

namespace AppFramework.Core.PL.Components.CatTax
{
    public partial class TaxonomiesTree : System.Web.UI.WebControls.TreeView
    {
        public delegate void TaxonomyItemSelectedHandler(TaxonomyItem item);

        public Taxonomy Taxonomy;

        /// <summary>
        /// Gets the selected item uid.
        /// </summary>
        /// <value>The selected item uid.</value>
        public long SelectedItemUid
        {
            get
            {
                long uid = 0;
                long.TryParse(this.SelectedValue, out uid);
                return uid;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether need to show Manage Asset types link
        /// </summary>
        /// <value><c>true</c> if show link; otherwise, <c>false</c>.</value>
        public bool ShowManageLink
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets if 'Add node' link must be shown behind the node
        /// </summary>
        public bool ShowAddNodeLink
        {
            get;
            set;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);           
            this.ExpandAll();
            this.SelectedNodeStyle.Font.Bold = true;
        }

        /// <summary>
        /// Builds the tree.
        /// </summary>
        public void BuildTree()
        {
            this.Nodes.Clear();
            if (Taxonomy != null)
            {
                foreach (TaxonomyItem item in Taxonomy.RootItems)
                {
                    this.Nodes.Add(item.ToTreeNode(ShowAddNodeLink, ShowManageLink));
                }
            }
            this.ExpandAll();
        } 
    }
}