using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AssetSite.Controls;
using System.Web.UI.HtmlControls;

namespace AssetSite.Wizard
{
    public partial class MasterPageWizard : System.Web.UI.MasterPage
    {
        public delegate void ButtonHandler(object sender, EventArgs e);

        public event ButtonHandler ButtonNextClicked;
        public event ButtonHandler ButtonPreviousClicked;
        public event ButtonHandler ButtonCancelClicked;
        
        public Button NextButton { get { return this.btnNext; } }
        public Button PreviousButton { get { return this.btnPrevious; } }
        public Button CancelButton { get { return this.btnClose; } }
        public WizardMenu WizardMenu { get { return this.WizardMenu1; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            btnNext.Click += btnNext_Click;
            btnPrevious.Click += btnPrevious_Click;
            btnClose.Click += btnClose_Click;

            if (!IsPostBack)
            {
                if (this.Page.GetType().ToString().ToLower().Contains("step4"))
                {
                    var controller = (WizardController)Page;
                    if (controller.AssetType != null)
                    {
                        if ((controller.AssetType.AutoGenerateNameType == AppFramework.ConstantsEnumerators.Enumerators.TypeAutoGenerateName.InsertOnly ||
                            controller.AssetType.AutoGenerateNameType == AppFramework.ConstantsEnumerators.Enumerators.TypeAutoGenerateName.InsertUpdate))// &&
                            //controller.AssetType.Attributes.Where(a => a.IsUsedForNames).Count() > 0)
                        {                            
                            btnNameGenSort.Visible = true;
                        }
                    }
                }
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            ButtonNextClicked(sender, e);
        }

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            ButtonPreviousClicked(sender, e);
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            ButtonCancelClicked(sender, e);
        }
    }
}
