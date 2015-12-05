using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AssetManager.Infrastructure.Services;
using AssetSite.Helpers;
using Microsoft.Practices.Unity;
using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AssetSite.admin
{
    public partial class FAQItems : BasePage
    {
        [Dependency]
        public IFaqService FaqService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!CookieWrapper.IsSet)
                    Session["curCult"] = ApplicationSettings.DisplayCultureInfo.Name;
                else
                    Session["curCult"] = CookieWrapper.Language;

                repLangs.DataBind();
                this.BindFaqItems();
            }

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "_DlgInitalize_" + this.ClientID, "$(function () {$('#" + this.DialogContainer.ClientID + "').dialog({ autoOpen: false, width: 790, height: 700 });});", true);
            lbtnShowDlg.Attributes.Add("onclick", "return ShowFaqDlg('" + this.DialogContainer.ClientID + "',0);");
            lbtnOK.Attributes.Add("onclick", "return SaveItem('" + this.DialogContainer.ClientID + "');");

            litCultureName.Text = "<script> var cultName='" + Session["curCult"] + "'; </script>";

            //string handlerScript = "<script type='text/javascript'>function onButtonClick() { alert('qwe'); SetWaitImage('" + this.DialogContainer.ClientID + "');}</script>";
            //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "imagehandler", handlerScript);
        }

        protected void OnChangeCulture(object sender, CommandEventArgs e)
        {
            Session["curCult"] = e.CommandArgument.ToString();
            litCultureName.Text = "<script> var cultName='" + Session["curCult"] + "'; </script>";
            repLangs.DataBind();
            this.BindFaqItems();
        }

        #region Binding helpers
        private void BindFaqItems()
        {
            string cultureName = Session["curCult"].ToString();
            gvFaqItems.DataSource = FaqService.GetFaqItems(CultureInfo.GetCultureInfo(cultureName));
            gvFaqItems.DataBind();
        }

        public string GetCssClass(object culture)
        {
            if (culture.ToString() == Session["curCult"].ToString())
                return "linkSelected";

            return string.Empty;
        }

        public string GetEditScript(object id)
        {
            return "return ShowFaqDlg('" + this.DialogContainer.ClientID + "'," + id + ");";
        }
        #endregion

        protected void lbDelete_Command(object sender, CommandEventArgs e)
        {
            var currentAT = AssetTypeRepository.GetPredefinedAssetType(PredefinedEntity.Faq);
            long aId = long.Parse(e.CommandArgument.ToString());
            var asset = AssetsService.GetAssetById(aId, currentAT);
            var permission = AuthenticationService.GetPermission(asset);
            AssetsService.DeleteAsset(asset, permission);
            this.BindFaqItems();
        }

        protected void lbtnRebind_Click(object sender, EventArgs e)
        {
            this.BindFaqItems();
        }
    }
}