using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Reports;
using AppFramework.Core.Classes.Tasks;
using AppFramework.Core.Classes.Tasks.Runners;
using AssetManager.Infrastructure.Services;
using Microsoft.Practices.Unity;
using System;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace AssetSite.admin.Tasks
{
    public partial class EditTask : BasePage
    {
        [Dependency]
        public ITasksService TasksService { get; set; }
        [Dependency]
        public ITaskRunnerFactory TaskRunnerFactory { get; set; }

        const string SessionFlagKey = "TaskConfig";
        const string SessionFunctionData = "TaskData";

        public string ExecutableType
        {
            get;
            set;
        }
        public string ExecutablePath
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Request.QueryString["AssetTypeId"]))
                Response.Redirect("~/Wizard/EditAssetType.aspx");

            long assetTypeId = Convert.ToInt64(Request.QueryString["AssetTypeId"]);

            if (!IsPostBack &&
                !String.IsNullOrEmpty(Request.QueryString["Edit"]) &&
                !String.IsNullOrEmpty(Request.QueryString["TaskId"]))
            {
                var task = TasksService.GetTaskById(long.Parse(Request.QueryString["TaskId"]));

                txtName.Text = task.Name;
                txtDescription.Text = task.Description;
                chkShowAtSidebar.Checked = task.DisplayInSidebar;

                int fType = (int)task.FunctionType;

                ddlFunctionType.SelectedValue = fType.ToString();
                mvFunctions.ActiveViewIndex = fType;

                InitTask(assetTypeId, task);
                ddlFunctionType_SelectedIndexChanged(ddlFunctionType, null);

                switch (task.FunctionType)
                {
                    case (int)TaskFunctionType.LaunchBatch:
                        var batchData = BatchTaskParametersDescriptor.Deserialize(task.FunctionData);
                        BatchCommonParams.ExecutablePath = task.ExecutablePath;
                        BatchCommonParams.ExecutableType = (TaskExecutableType)task.ExecutableType;
                        BatchCommonParams.FunctionData = batchData.Data;
                        btnSave.OnClientClick = "return CollectParams('" + BatchCommonParams.HiddenFieldId + "');";
                        break;
                    case (int)TaskFunctionType.ImportFile:
                        ImportTaskParametersDescriptor importData = ImportTaskParametersDescriptor.Deserialize(task.FunctionData);
                        importParams.ExecutablePath = task.ExecutablePath;
                        importParams.ExecutableType = (TaskExecutableType)task.ExecutableType;
                        importParams.FunctionData = importData.Data;
                        btnSave.OnClientClick = "return CollectParams('" + importParams.HiddenFieldId + "');";
                        ddlOutputFileType.SelectedValue = ((int)importData.FileType).ToString();
                        break;
                    case (int)TaskFunctionType.ExportFileSSIS:
                        var exportData = ExportTaskParametersDescriptor.Deserialize(task.FunctionData);
                        exportParams.ExecutablePath = task.ExecutablePath;
                        exportParams.FunctionData = exportData.Data;
                        break;
                    case (int)TaskFunctionType.CreateAsset:
                        SetScreensDataSource();
                        ddlView.DataBound += (s, args) =>
                        {
                            var newAssetData = NewAssetTaskParametrsDescriptor.Deserialize(task.FunctionData);
                            if (newAssetData.ScreenId.HasValue)
                                ddlView.SelectedValue = newAssetData.ScreenId.ToString();
                        };
                        break;
                    case (int)TaskFunctionType.PrintReport:
                        SetCurrentReport(assetTypeId, task.FunctionData);
                        break;
                    case (int)TaskFunctionType.ExecuteSearch:
                    case (int)TaskFunctionType.ExportFileSearch:
                        var runner = TaskRunnerFactory.GetRunner(task, AuthenticationService.CurrentUserId, null);
                        var result = runner.Run(task);
                        if (result.Status == TaskStatus.Sussess && result.ActionOnComplete == TaskActionOnComplete.Navigate)
                        {
                            ifSearch.Attributes.Add("src", result.NavigationResult);
                        }
                        break;
                }
            }
        }

        private void InitTask(long assetTypeId, AppFramework.Entities.Task task)
        {
            if (!string.IsNullOrEmpty(ddlFunctionType.SelectedValue))
            {
                switch (int.Parse(ddlFunctionType.SelectedValue))
                {
                    case (int)TaskFunctionType.PrintReport:
                        BindReportsList(assetTypeId, task.FunctionData);
                        break;
                    default:
                        break;
                }
            }
        }

        private void SetScreensDataSource()
        {
            edsScreens.WhereParameters.Clear();
            long assetTypeId = Convert.ToInt64(Request.QueryString["AssetTypeId"]);
            AssetType current = AssetType.GetByID(assetTypeId);
            edsScreens.WhereParameters.Add(new Parameter() { Name = "dynEntityConfigUid", DefaultValue = current.UID.ToString(), Type = TypeCode.Int64 });
        }

        protected void ddlFunctionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedType = Convert.ToInt32((sender as DropDownList).SelectedValue);
            mvFunctions.ActiveViewIndex = selectedType;

            switch ((TaskFunctionType) selectedType)
            {
                case TaskFunctionType.ExecuteSearch:
                case TaskFunctionType.ExportFileSearch:
                    Session[SessionFlagKey] = "1";
                    btnSave.OnClientClick = "return SearchDataCheck();";
                    break;
                case TaskFunctionType.LaunchBatch:
                    btnSave.OnClientClick = "return CollectParams('" + BatchCommonParams.HiddenFieldId + "');";
                    break;
                case TaskFunctionType.ImportFile:
                    btnSave.OnClientClick = "return CollectParams('" + importParams.HiddenFieldId + "');";
                    break;
                case TaskFunctionType.ExportFileSSIS:
                    btnSave.OnClientClick = "return CollectParams('" + exportParams.HiddenFieldId + "');";
                    break;
                case TaskFunctionType.CreateAsset:
                    SetScreensDataSource();
                    break;
                case TaskFunctionType.PrintReport:
                    long assetTypeId = Convert.ToInt64(Request.QueryString["AssetTypeId"]);
                    BindReportsList(assetTypeId);
                    break;
                case TaskFunctionType.ExecuteSqlServerAgentJob:
                    BindAgentJobsList();
                    break;
            }
        }

        private void BindAgentJobsList()
        {
            var jobs = UnitOfWork.GetSqlServerAgentJobs();
            dlAgentJobs.DataSource = jobs;
            dlAgentJobs.DataTextField = "name";
            dlAgentJobs.DataValueField = "job_id";
            dlAgentJobs.DataBind();
        }

        private void BindReportsList(long assetTypeId, string reportId = null)
        {
            var reports = Report.GetAll().Where(r => r.AssetTypeId == assetTypeId);
            ReportsList.DataSource = reports;
            ReportsList.DataBind();
        }

        private void SetCurrentReport(long assetTypeId, string reportId = null)
        {
            if (!string.IsNullOrEmpty(reportId))
            {
                var item = ReportsList.Items.FindByValue(reportId);
                if (item != null)
                {
                    item.Selected = true;
                }
            }
        }

        [WebMethod]
        public static bool CheckForFunctionData()
        {
            return HttpContext.Current.Session[SessionFunctionData] != null;
        }

        protected void btnSaveTaskConfig_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            long assetTypeId = Convert.ToInt64(Request.QueryString["AssetTypeId"]);
            TaskFunctionType selectedType = (TaskFunctionType)Convert.ToInt32(ddlFunctionType.SelectedValue);

            var currentTask = !string.IsNullOrEmpty(Request.QueryString["TaskId"]) 
                ? TasksService.GetTaskById(Convert.ToInt64(Request.QueryString["TaskId"])) 
                : new AppFramework.Entities.Task();

            currentTask.DynEntityConfigId = assetTypeId;
            currentTask.Name = txtName.Text;
            currentTask.Description = txtDescription.Text;
            currentTask.FunctionType = (int)selectedType;
            currentTask.IsActive = true;
            currentTask.DisplayInSidebar = chkShowAtSidebar.Checked;

            switch (selectedType)
            {
                case TaskFunctionType.ExecuteSearch:
                case TaskFunctionType.ExportFileSearch:
                    (viewSearch.FindControl("searchSessionEmpty") as Label).Visible = Session[SessionFunctionData] ==
                                                                                      null;
                    if (Session[SessionFunctionData] == null)
                    {
                        return;
                    }
                    else
                    {
                        currentTask.FunctionData = Session[SessionFunctionData].ToString();
                        currentTask.ExecutableType = (int)TaskExecutableType.Internal;
                        currentTask.ExecutablePath = null;
                        Session[SessionFunctionData] = null;
                    }
                    break;

                case TaskFunctionType.LaunchBatch:
                    currentTask.ExecutableType = (int)BatchCommonParams.GetExecutableType();
                    currentTask.ExecutablePath = BatchCommonParams.GetExecutablePath();
                    BatchTaskParametersDescriptor batchData = new BatchTaskParametersDescriptor();
                    batchData.Data = BatchCommonParams.GetFunctionData();
                    currentTask.FunctionData = batchData.Serialize();
                    break;

                case TaskFunctionType.ImportFile:
                    currentTask.ExecutableType = (int)importParams.GetExecutableType();
                    currentTask.ExecutablePath = importParams.GetExecutablePath();
                    ImportTaskParametersDescriptor importData = new ImportTaskParametersDescriptor();
                    importData.Data = importParams.GetFunctionData();
                    importData.FileType = (TaskImportFileType) Convert.ToInt32(ddlOutputFileType.SelectedValue);
                    currentTask.FunctionData = importData.Serialize();
                    break;

                case TaskFunctionType.ExportFileSSIS:
                    currentTask.ExecutableType = (int)exportParams.GetExecutableType();
                    currentTask.ExecutablePath = exportParams.GetExecutablePath();
                    var exportData = new ExportTaskParametersDescriptor();
                    exportData.Data = exportParams.GetFunctionData();
                    currentTask.FunctionData = exportData.Serialize();
                    break;

                case TaskFunctionType.CreateAsset:
                    currentTask.ExecutableType = (int)TaskExecutableType.Internal;
                    currentTask.ExecutablePath = null;
                    NewAssetTaskParametrsDescriptor newAssetData = new NewAssetTaskParametrsDescriptor();
                    if (!string.IsNullOrEmpty(ddlView.SelectedValue) && long.Parse(ddlView.SelectedValue) > 0)
                        newAssetData.ScreenId = long.Parse(ddlView.SelectedValue);
                    currentTask.FunctionData = newAssetData.Serialize();
                    break;

                case TaskFunctionType.PrintReport:
                    currentTask.ExecutableType = (int)TaskExecutableType.Internal;
                    currentTask.ExecutablePath = null;
                    currentTask.FunctionData = ReportsList.SelectedValue;
                    break;

                case TaskFunctionType.ExecuteSqlServerAgentJob:
                    currentTask.ExecutableType = (int)TaskExecutableType.Internal;
                    currentTask.ExecutablePath = null;
                    currentTask.FunctionData = dlAgentJobs.SelectedValue;
                    break;
            }

            TasksService.SaveTask(currentTask, AuthenticationService.CurrentUserId);
            Response.Redirect("TasksList.aspx?AssetTypeId=" + Request.QueryString["AssetTypeId"]);
        }

        protected void ReportsList_DataBound(object sender, EventArgs e)
        {
            (sender as DropDownList).Items.Insert(0, new ListItem() { Value = string.Empty, Text = "Select..." });
        }
    }
}