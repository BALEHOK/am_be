using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Batch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.Practices.Unity;

namespace AssetSite.admin
{
    public partial class LocationMove : BasePage
    {
        [Dependency]
        public IBatchJobManager BatchJobManager { get; set; }
        [Dependency]
        public IBatchJobFactory BatchJobFactory { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lstAssetTypes.DataSource = AssetTypeRepository.GetAllPublished();
                lstAssetTypes.DataBind();
            }
        }

        protected void btnMove_Click(object sender, EventArgs e)
        {
            if (!locationsList.GetAssetIds().Any())
            {
                lblValidation.Text = (string)GetLocalResourceObject("LocationNotSelectedMessage");
                lblValidation.Visible = true;
                return;
            }

            List<long> atList = new List<long>();
            foreach (ListItem item in lstAssetTypes.Items)
            {
                if (item.Selected)
                {
                    atList.Add(long.Parse(item.Value));
                }
            }

            if (!atList.Any())
            {
                lblValidation.Text = (string)GetLocalResourceObject("TypeNotSelectedMessage");
                lblValidation.Visible = true;
                return;
            }

            var job = BatchJobFactory.CreateLocationMoveJob(
                AuthenticationService.CurrentUserId, 
                BatchActionType.MoveToLocation, 
                new []{ locationsList.GetAssetIds(), atList });
            BatchJobManager.SaveJob(job);
            Response.Redirect(job.NavigateUrl);
        }
    }
}