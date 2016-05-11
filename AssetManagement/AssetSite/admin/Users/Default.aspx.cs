using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AssetSite.admin.Users
{
    public partial class Default : BasePage
    {
        protected long AssetTypeId
        {
            get
            {
                return AssetTypeRepository.GetPredefinedAssetType(PredefinedEntity.User).ID;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void gvUsers_DataBound(object sender, GridViewRowEventArgs e)
        {
             if (e.Row.RowType == DataControlRowType.DataRow)
             {
                var deleteButton = e.Row.FindControl("lbtnDelete") as LinkButton;
                string postbackEvent = Page.ClientScript.GetPostBackEventReference(deleteButton, "");
                deleteButton.OnClientClick = "return ShowConfirmationDialog(function(){ " + postbackEvent + " });";
             }
        }

        protected void usersDataSource_Selecting(Object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (!e.InputParameters.Contains("assetTypeId"))
            {
                e.InputParameters.Add("assetTypeId", AssetTypeId);
            }
        }

        protected void lbtnDelete_Click(object sender, EventArgs e)
        {
            long id = long.Parse(((LinkButton)sender).CommandArgument.ToString());
            var at = AssetTypeRepository.GetById(AssetTypeId);
            var asset = AssetsService.GetAssetById(id, at);
            var permission = AuthenticationService.GetPermission(asset);
            AssetsService.DeleteAsset(asset);
            gvUsers.DataBind();
        }
    }
}
