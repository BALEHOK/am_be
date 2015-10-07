using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using DayPilot.Web.Ui;
using DayPilot.Web.Ui.Events;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommandEventArgs = System.Web.UI.WebControls.CommandEventArgs;

namespace AssetSite.Asset.Reservations
{
    public partial class Overview : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            repAssetTypes.DataSource = AssetType.GetAllReservable();
            repAssetTypes.DataBind();

            scheduler.ClientObjectName = scheduler.ClientID + "_Scheduler";
            scheduler.StartDate = DateTime.Today.Date.AddMonths(-1);
            scheduler.Days = (DateTime.Today.Date.AddMonths(3) - scheduler.StartDate).Days;
            scheduler.TimeRangeSelectedJavaScript =
                string.Format("resDashboardTimeRangeSelected('{0}', start, end, resource)",
                    scheduler.ClientID);
            scheduler.EventClickJavaScript =
                string.Format("resDashboardEventClick(e)");
          
            var initDialogs = string.Format("initReservationDialog('{0}', '{1}', '{2}');",
                ReleaseDialog.ClientID, cbIsDamaged.ClientID, tbRemark.ClientID);
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "_DlgInitalize_" + this.ClientID, 
                "$(function () { " + initDialogs + " })",
                true);

            if (!IsPostBack)
                this.BindData();
        }

        protected void gvHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvHistory.PageIndex = e.NewPageIndex;
            this.BindData();
        }

        protected void lbtnRebind_Click(object sender, EventArgs e)
        {
            this.BindData();
        }

        private void BindData()
        {
            if (Request["CurrentAssetType"] == null)
                return;

            scheduler.Visible = true;
            var assetTypeUid = long.Parse(Request["CurrentAssetType"]);

            var reservationsService = new ReservationService(UnitOfWork, AuthenticationService);
            var assetType = AssetTypeRepository.GetByUid(assetTypeUid);
            var usersType = AssetTypeRepository.GetPredefinedAssetType(PredefinedEntity.User);

            var reservations = reservationsService.GetReservations(assetTypeUid, true).ToList();
            gvHistory.DataSource = reservations.Any()
                ? reservations.Select(res => ReservationDescriptor.ReservationToDescriptor(res, assetType, usersType, AssetsService)).ToList()
                : null;
            gvHistory.DataBind();

            var assets = AssetsService.GetAssetsByAssetTypeAndUser(assetType, AuthenticationService.CurrentUserId);
            scheduler.Resources.Clear();
            foreach (var asset in assets)
            {
                scheduler.Resources.Add(new Resource(asset.Name, asset.UID.ToString()));
            }
            var schedulerData = reservations
                .Select(r => ReservationDescriptor.ReservationToDescriptor(r, assetType, usersType, AssetsService))
                .ToList();
            foreach (var sd in schedulerData)
            {
                sd.EndDate = DateTime.Parse(sd.EndDate).AddDays(1).ToShortDateString();
            }
            scheduler.DataSource = schedulerData;
            scheduler.DataBind();
            scheduler.Visible = scheduler.Resources.Count > 0;
            scheduler.SetScrollX(DateTime.Today.AddDays(-1));

            ddlNewResource.DataSource = assets;
            ddlNewResource.DataBind();

            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "_" + this.ClientID,
                ResolveClientUrl("~/javascript/ReservationDashboard.js"));
            Page.ClientScript.RegisterStartupScript(this.GetType(), "_NewDlgInit_" + this.ClientID,
                string.Format(
                    "resDashboardInitDlg({0}, '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}');",
                    assetTypeUid,
                    scheduler.ClientID,
                    ReservationsDialog.ClientID,
                    ReservationUid.ClientID,
                    ddlNewResource.ClientID,
                    UsersList1.SelectedTextInputBoxClientId,
                    UsersList1.SelectedValueInputBoxClientId,
                    tbNewStartDate.ClientID,
                    tbNewEndDate.ClientID,
                    tbNewReason.ClientID,
                    lblNewBorrowError.ClientID),
                true);
        }
     
        public string GetImgUrl(bool isBorrowed)
        {
            var basePath = Request.Url.GetLeftPart(UriPartial.Authority);
            if (isBorrowed) // this item is currently borrowed. We can return it.
                basePath += "/images/bringBack.png";
            else // it's reserved. we may borrow it right away
                basePath += "/images/buttons/borrow.png";
            return basePath;
        }

        protected void ibtnCancel_Command(object sender, CommandEventArgs e)
        {
            var service = new ReservationService(UnitOfWork, AuthenticationService);
            service.CancelReservation(long.Parse(e.CommandArgument.ToString()));

            Response.Redirect("~/Reservations/Overview.aspx?CurrentAssetType="
                + Request["CurrentAssetType"]);
        }

        protected void lbtnAssetType_Command(object sender, CommandEventArgs e)
        {
            Response.Redirect("~/Reservations/Overview.aspx?CurrentAssetType=" 
                + e.CommandArgument);
        }

        protected bool CanBorrowOrRelease(string startDate)
        {
            return DateTime.Parse(startDate) <= DateTime.Now;
        }

        protected void gvHistory_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                
            }
        }

        protected void scheduler_OnBeforeCellRender(object sender, BeforeCellRenderEventArgs e)
        {
        }
    }
}