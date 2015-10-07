using System.Web.UI;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.DataProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.Practices.Unity;

namespace AppFramework.Core.PL.Components.CatTax
{
    public class TaxonomiesDropDown : DropDownList
    {
        [Dependency]
        public ITaxonomyService TaxonomyService { get; set; }

        public delegate void SelectedTaxonomyChangedHandler(Taxonomy tax);
        public event SelectedTaxonomyChangedHandler SelectedTaxonomyChanged;

        public Taxonomy DefaultTaxonomy
        {
            get { return _defaultTaxonomy ?? (_defaultTaxonomy = TaxonomyService.GetCategory()); }
        }
        private Taxonomy _defaultTaxonomy;

        public override bool AutoPostBack
        {
            get { return true; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                DataSource = TaxonomyService.GetAll().ToList(); 
                DataTextField = "Name";
                DataValueField = "UID";
                DataBind();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            Page.RegisterRequiresControlState(this);
            base.OnInit(e);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);

            long uid = 0;
            long.TryParse(this.SelectedValue, out uid);
            if (uid != 0)
            {
                if (SelectedTaxonomyChanged != null)
                {
                    var t = TaxonomyService.GetByUid(uid);
                    SelectedTaxonomyChanged(t);
                }
            }
        }
    }
}
