using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AppFramework.Core.Classes;
using AssetSite.Helpers;

namespace AssetSite.Wizard
{
    /// <summary>
    /// Parent class for all Wizard pages
    /// Put common logic for Wizard here
    /// </summary>
    public class WizardController : BasePage
    {
        protected string path = string.Empty;
        protected string fileName = string.Empty;
        protected bool savedStateExist = false;

        private int CurrentStepIndex
        {
            get
            {
                Regex re = new Regex(@"Step(\d+)");
                Match m = re.Match(Request.Url.PathAndQuery);
                int currentStep = 0;

                if (m.Groups[1].Success)
                {
                    int.TryParse(m.Groups[1].Value, out currentStep);
                }
                return currentStep;
            }
        }

        public AssetType AssetType
        {
            get
            {
                return Session["AssetTypeWizard"] as AssetType;
            }
            set
            {
                Session["AssetTypeWizard"] = value;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            (Page.Master as MasterPageWizard).ButtonNextClicked
                += new MasterPageWizard.ButtonHandler(btnNext_Click);

            (Page.Master as MasterPageWizard).ButtonPreviousClicked
                += new MasterPageWizard.ButtonHandler(btnPrevious_Click);

            (Page.Master as MasterPageWizard).ButtonCancelClicked
                += new MasterPageWizard.ButtonHandler(btnClose_Click);

            if (AssetType == null) 
                return;

            AssetTypeAttribute cStock
                = AssetType.Attributes.FirstOrDefault(a => a.DBTableFieldName == "StockCount");
            AssetTypeAttribute pStock
                = AssetType.Attributes.FirstOrDefault(a => a.DBTableFieldName == "StockPrice");

            // if not in stock, hide special stock attributes   
            if (cStock != null && pStock != null)
            {
                cStock.IsShownOnPanel = AssetType.IsInStock;
                pStock.IsShownOnPanel = AssetType.IsInStock;
            }

            // prevent step-over navigation for new assettypes
            if (AssetType.UID == 0)
            {
                CheckVisitingOrder(CurrentStepIndex);
            }

            (Master as MasterPageWizard).WizardMenu.CurrentStepIndex = CurrentStepIndex;
        }

        protected virtual void btnNext_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected virtual void btnPrevious_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Prevents step over navigation on new asset's creation
        /// </summary>
        private void CheckVisitingOrder(int currentStep)
        {
            if (Session["MaxReachedStepWizard"] == null)
            {
                Session["MaxReachedStepWizard"] = 1;
            }

            int maxReachedStep = 1;
            int.TryParse(Session["MaxReachedStepWizard"].ToString(), out maxReachedStep);
            
            // skip removed steps
            if (currentStep == 9)
                maxReachedStep = 8;

            // if it is new asset type creation and step over, then redirect
            if (currentStep > maxReachedStep + 1)
            {
                Response.Redirect(string.Format("~/Wizard/Step{0}.aspx", maxReachedStep));
            }
            else if (currentStep > maxReachedStep)
            {
                Session["MaxReachedStepWizard"] = currentStep;
            }

        }

        /// <summary>
        /// Inits the state - checks file name, create directories if needed
        /// </summary>
        /// <param name="createDirectories">if set to <c>true</c> [create directories].</param>
        /// <returns>Return <see cref="Boolean"/> value</returns>
        public bool InitState(bool createDirectories)
        {
            if (this.AssetType != null)
            {
                string dirName = AuthenticationService.CurrentUserId.ToString();
                this.path = Path.Combine(ApplicationSettings.TempFullPath, dirName) + "\\";
                if (createDirectories) Directory.CreateDirectory(path);
                this.fileName = path + string.Format("{0}{1}_{2}.xml", "AT_", this.AssetType.UID, DateTime.Now.ToString("ddMMyyyy"));
                this.savedStateExist = File.Exists(this.fileName);
            }

            return this.savedStateExist;
        }

        protected virtual void btnClose_Click(object sender, EventArgs e)
        {
            SessionWrapper.CleanWizardSession();
            Response.Redirect("~/admin/");
        }
    }
}
