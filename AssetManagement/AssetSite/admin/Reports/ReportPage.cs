namespace AssetSite.admin.Reports
{
    using System;
    using AppFramework.Core.Classes.Reports;

    public class ReportPage : BasePage
    {
        public Report report;

        public event Action RequestUidEmpty;

        protected override void OnLoad(EventArgs e)
        {
            #region Init
            this.RequestUidEmpty += new Action(OnRequestUidEmpty);

            long uid = 0;

            if (long.TryParse(Request.QueryString["Uid"], out uid))
            {
                report = Report.GetByUid(uid);
            }
            else
            {
                if (RequestUidEmpty != null) RequestUidEmpty.Invoke();
            }
            #endregion
            base.OnLoad(e);
        }

        protected virtual void OnRequestUidEmpty()
        {
            Response.Redirect("Default.aspx");
        }
    }
}
