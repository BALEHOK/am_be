using System;
using System.Web.UI;
using AppFramework.Core.Classes;

namespace AssetSite.admin
{
    public partial class ZipsAndPlaces : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["SelectedPlace"] = 0;
            }

            long placeId = Convert.ToInt64(Session["SelectedPlace"]);

            if (!IsPostBack)
            {
                this.BindZips();
            }

            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "_DlgInitalize_" + this.ClientID,
                "$(function () {$(\"#" + this.DialogContainer.ClientID + "\").dialog({ autoOpen: false, width: 420, height: 180});});", true);
            lbtnOK.Attributes.Add("onclick", "return modifyPOZ(" + placeId + ",\"" + this.DialogContainer.ClientID + "\");");
            lbtnAddPlace.Attributes.Add("onclick", "return OnPOZEditDialog(0,0,\"" + this.DialogContainer.ClientID + "\");");
            lbtnAddZip.Attributes.Add("onclick", "return  OnPOZEditDialog(0,1,\"" + this.DialogContainer.ClientID + "\");");
        }

        protected void gvPlaces_SelectedIndexChanged(object sender, EventArgs e)
        {
            RebindZips();
        }

        #region Binding helpers
        private void BindZips()
        {
            long placeId = Convert.ToInt64(Session["SelectedPlace"]);
            if (placeId != 0)
            {
                gvZips.DataSource = Place.GetZipCodes(placeId);
                gvZips.DataBind();

                lbtnAddZip.Visible = true;
            }
        }

        public string GetPlaceEditScript(object placeId)
        {
            string editScript = "return OnPOZEditDialog(" + placeId + ",0,\"" + DialogContainer.ClientID + "\");";
            return editScript;
        }
        public string GetPlaceDeleteScript(object palceId)
        {
            return "return DeletePlace(" + palceId + ");";
        }

        public string GetZipEditScript(object zipId)
        {
            string editScript = "return OnPOZEditDialog(" + zipId + ",1,\"" + DialogContainer.ClientID + "\");";
            return editScript;
        }
        public string GetZipDeleteScript(object zipId)
        {
            return "return DeleteZip(" + zipId + ");";
        }

        protected void OnRebindZipCodes(object sender, EventArgs e)
        {
            this.BindZips();
        }

        protected void OnRebindPlaces(object sender, EventArgs e)
        {
            gvPlaces.DataSourceID = placesDataSource.ID;
            gvPlaces.DataBind();
            RebindZips();
            
        }
        #endregion

        private void RebindZips()
        {
            if (gvPlaces.SelectedDataKey != null)
            {
                Session["SelectedPlace"] = gvPlaces.SelectedDataKey.Value;
                lbtnOK.Attributes.Add("onclick", "return modifyPOZ(" + gvPlaces.SelectedDataKey.Value + ",\"" + this.DialogContainer.ClientID + "\");");
                this.BindZips();    
            }
        }

        protected void gvPlaces_PageIndexChanged(object sender, EventArgs e)
        {
            Session["SelectedPlace"] = 0;
            gvZips.DataSource = null;
            gvZips.DataBind();

            gvPlaces.SelectedIndex = -1;
        }

        protected void btnRebind_Click(object sender, EventArgs e)
        {
            gvPlaces.DataSourceID = "placesDataSource";
            gvPlaces.DataBind();
        }
    }
}