using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Batch;
using AssetSite.Helpers;
using System;
using System.Web.UI;
using Microsoft.Practices.Unity;

namespace AssetSite.admin.Batch
{
    public partial class Edit : BasePage
    {
        [Dependency]
        public IBatchJobManager BatchJobManager { get; set; }

        protected BatchJob BatchJob { get; set; }

        protected string Locale
        {
            get
            {
                return CookieWrapper.Language.Split(new char[] { '-' })[0].ToLower();
            }
        }

        protected string DatePattern
        {
            get
            {
                if (ApplicationSettings.DisplayCultureInfo.TwoLetterISOLanguageName == "en")
                {
                    return "mm/dd/yy";
                }
                else if (ApplicationSettings.DisplayCultureInfo.TwoLetterISOLanguageName == "fr")
                {
                    return "dd/mm/yy";
                }
                else
                {
                    return "dd-mm-yy";
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.RegisterClientScriptInclude(this, typeof(Edit), "timepicker", "/javascript/jquery.ui.timepicker.js");
            if (Locale != "en")
            {
                Page.ClientScript.RegisterClientScriptInclude("datepicker_localization",
                  string.Format("/javascript/jquery.ui.datepicker-{0}.js", Locale));
                Page.ClientScript.RegisterClientScriptInclude("timepicker_localization",
                  string.Format("/javascript/jquery.ui.timepicker-{0}.js", Locale));
            }

            long uid = 0;
            if (Request["BatchUid"] != null && long.TryParse(Request["BatchUid"], out uid))
            {
                BatchJob = BatchJobManager.GetByUid(uid);
                if (BatchJob.BatchSchedule == null)
                    BatchJob.Schedule(0, DateTime.Now.AddHours(1), "");

                if (!IsPostBack)
                {
                    if (BatchJob.BatchSchedule.RepeatInHours > 0)
                        dlRepeat.SelectedIndex = dlRepeat.Items.IndexOf(
                            dlRepeat.Items.FindByValue(BatchJob.BatchSchedule.RepeatInHours.Value.ToString()));
                    DataBind();
                }
            }
            else
            {
                Response.Redirect("/admin/Batch/");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                BatchJob.BatchSchedule.IsEnabled = chkEnabled.Checked;
                BatchJob.BatchSchedule.RepeatInHours = null;
                if (!string.IsNullOrEmpty(dlRepeat.SelectedValue))
                {
                    double repeat = double.Parse(dlRepeat.SelectedValue);
                    if (repeat > 0)
                        BatchJob.BatchSchedule.RepeatInHours = repeat;
                }
                BatchJob.BatchSchedule.ExecuteAt = DateTime.Parse(string.Format("{0} {1}", txtDatePicker.Text, txtTimepicker.Text));
                BatchJob.BatchSchedule.Notes = txtNotes.Text;
                BatchJobManager.SaveJob(BatchJob);
                Response.Redirect("/admin/Batch/Actions.aspx?BatchUid=" + BatchJob.UID);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("/admin/Batch/Actions.aspx?BatchUid=" + BatchJob.UID);
        }
    }
}