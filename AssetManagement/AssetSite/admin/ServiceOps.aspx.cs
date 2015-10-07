using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Batch;
using System;
using AppFramework.DataProxy;
using Microsoft.Practices.Unity;

namespace AssetSite.admin
{
    public partial class ServiceOps : BasePage
    {
        [Dependency]
        public IAuthenticationService AuthenticationService { get; set; }
        [Dependency]
        public IBatchJobFactory BatchJobFactory { get; set; }

        protected void RebuildActive_Click(object sender, EventArgs e)
        {
            var job = BatchJobFactory.CreateRebuildIndexJob(AuthenticationService.CurrentUserId);
            Response.Redirect(job.NavigateUrl);
        }

        protected void RebuildHistory_Click(object sender, EventArgs e)
        {
            var job = BatchJobFactory.CreateRebuildIndexJob(AuthenticationService.CurrentUserId, true);
            Response.Redirect(job.NavigateUrl);
        }
    }
}
