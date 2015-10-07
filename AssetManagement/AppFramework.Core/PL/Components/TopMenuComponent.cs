using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Permissions;

namespace AppFramework.Core.PL.Components
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:TopMenuComponent runat=server></{0}:TopMenuComponent>")]
    [ParseChildren(true)]
    /*[Designer("WebControls.DialogDesigner, WebControls", typeof(IDesigner))]*/
    /*[AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]*/
    public class TopMenuComponent : CompositeControl
    {
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the component class.
        /// </summary>
        public TopMenuComponent():base()
        { }
        
        #endregion

        #region Private members

        // All private members (variables)
        private bool errorInComponent = false;
        private ITemplate containerTemplate;

        #endregion

        #region Enumerators
        #endregion

        #region Component Properties

/*
        [Browsable(false),PersistenceMode(PersistenceMode.InnerProperty)]
        [Localizable(true)]
        [Category("")]
        public virtual ITemplate ItemTemplate
        {
            get
            {
                return containerTemplate;
            }
            set
            {
                containerTemplate = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Text
        {
            get
            {
                String s = (String)ViewState["Text"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["Text"] = value;
            }
        }
*/
        #endregion

        #region Component Events

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"></see> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                AppFramework.CustomException.Handler handler = new AppFramework.CustomException.Handler(false);
                handler.HandleException(ex);
                errorInComponent = true;
            }

            base.OnLoad(e);
        }

        #endregion

        #region Component Methods (overriden)

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation 
        /// to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            try
            {
                this.Controls.Clear();
                base.CreateChildControls();

                CreateControls();

                this.ChildControlsCreated = true;
            }
            catch (Exception ex)
            {
                AppFramework.CustomException.Handler handler = new AppFramework.CustomException.Handler(false);
                handler.HandleException(ex);
                errorInComponent = true;
            }
        }

        /// <summary>
        /// Create the controls inside the component (buttons, panels, ...
        /// </summary>
        private void CreateControls()
        {
            // Create the controls for the left menu
            Panel pnlLeft = new Panel();
            pnlLeft.CssClass = "toleft headmenu";

            HyperLink hl = new HyperLink();
            hl.NavigateUrl = "~/Search/Search.aspx";
            hl.Text = "Search";
            pnlLeft.Controls.Add(hl);
            hl = new HyperLink();
            hl.NavigateUrl = "#";
            hl.Text = "Stock";
            //pnlLeft.Controls.Add(hl);
            hl = new HyperLink();
            hl.NavigateUrl = "#";
            //hl.Text = HttpContext.GetGlobalResourceObject("MenuItem", "Mobile").ToString();
            pnlLeft.Controls.Add(hl);
            hl = new HyperLink();
            hl.NavigateUrl = "#";
            //hl.Text = HttpContext.GetGlobalResourceObject("MenuItem", "Financial").ToString();
            pnlLeft.Controls.Add(hl);
            hl = new HyperLink();
            hl.NavigateUrl = "~/Reports/";
            hl.Text = "Reporting";
            pnlLeft.Controls.Add(hl);

            this.Controls.Add(pnlLeft);

            // Create the controls for the right menu
            Panel pnlRight = new Panel();
            pnlRight.CssClass = "toright headmenu";

            hl = new HyperLink();
            hl.NavigateUrl = "~/ConfigOverview.aspx";
            hl.Text = "Admin";
            pnlRight.Controls.Add(hl);
            hl = new HyperLink();
            hl.NavigateUrl = "#";
            hl.Text = "MyAccount";
            //pnlRight.Controls.Add(hl);
            hl = new HyperLink();
            hl.NavigateUrl = "#";
            hl.Text = "Help";
            //pnlRight.Controls.Add(hl);

            this.Controls.Add(pnlRight);
            
        }

        /// <summary>
        /// This method is present just to be able to force correct rendering during design-time
        /// </summary>
        internal void GetDesignTimeHtml()
        {
            this.EnsureChildControls();
        }

        /// <summary>
        /// This method is overriden to ensure that the outer control is rendered as a DIV-tag.
        /// </summary>
        protected override HtmlTextWriterTag TagKey
        {
            get { return HtmlTextWriterTag.Div; }
        }

        #endregion
    }
}
