using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Tasks;
using AppFramework.Core.Classes.Tasks.Runners;
using AppFramework.Tasks;
using AssetManager.Infrastructure.Services;
using Microsoft.Practices.Unity;
using System;
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

        public string ExecutableType { get; set; }

        public string ExecutablePath { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Request.QueryString["AssetTypeId"]))
                Response.Redirect("~/Wizard/EditAssetType.aspx");

            long assetTypeId = Convert.ToInt64(Request.QueryString["AssetTypeId"]);

            if (!IsPostBack &&
                !String.IsNullOrEmpty(Request.QueryString["Edit"]) &&
                !String.IsNullOrEmpty(Request.QueryString["TaskId"]))
            {
                var task = TasksService.GetTaskById(long.Parse(Request.QueryString["TaskId"]), AuthenticationService.CurrentUserId);

                txtName.Text = task.Name;
                txtDescription.Text = task.Description;
                chkShowAtSidebar.Checked = task.DisplayInSidebar;

                int fType = task.FunctionType;
                ddlFunctionType.SelectedValue = fType.ToString();
                ddlFunctionType_SelectedIndexChanged(ddlFunctionType, null);

                switch (task.FunctionType)
                {
                    case (int)Enumerations.TaskFunctionType.LaunchBatch:
                        mvFunctions.SetActiveView(viewBatch);
                        var batchData = BatchTaskParametersDescriptor.Deserialize(task.FunctionData);
                        BatchCommonParams.ExecutablePath = task.ExecutablePath;
                        BatchCommonParams.ExecutableType = (Enumerations.TaskExecutableType)task.ExecutableType;
                        BatchCommonParams.FunctionData = batchData.Data;
                        btnSave.OnClientClick = "return CollectParams('" + BatchCommonParams.HiddenFieldId + "');";
                        break;
                    case (int)Enumerations.TaskFunctionType.ImportFile:
                        mvFunctions.SetActiveView(viewImport);
                        ImportTaskParametersDescriptor importData = ImportTaskParametersDescriptor.Deserialize(task.FunctionData);
                        importParams.ExecutablePath = task.ExecutablePath;
                        importParams.ExecutableType = (Enumerations.TaskExecutableType)task.ExecutableType;
                        importParams.FunctionData = importData.Data;
                        btnSave.OnClientClick = "return CollectParams('" + importParams.HiddenFieldId + "');";
                        ddlOutputFileType.SelectedValue = ((int)importData.FileType).ToString();
                        break;
                    case (int)Enumerations.TaskFunctionType.ExportFileSSIS:
                        mvFunctions.SetActiveView(viewExportSSIS);
                        var exportData = ExportTaskParametersDescriptor.Deserialize(task.FunctionData);
                        exportParams.ExecutablePath = task.ExecutablePath;
                        exportParams.FunctionData = exportData.Data;
                        break;
                    case (int)Enumerations.TaskFunctionType.CreateAsset:
                        mvFunctions.SetActiveView(viewNewAsset);
                        SetScreensDataSource();
                        ddlView.DataBound += (s, args) =>
                        {
                            var newAssetData = NewAssetTaskParametrsDescriptor.Deserialize(task.FunctionData);
                            if (newAssetData.ScreenId.HasValue)
                                ddlView.SelectedValue = newAssetData.ScreenId.ToString();
                        };
                        break;
                    case (int)Enumerations.TaskFunctionType.ExecuteSearch:
                        mvFunctions.SetActiveView(viewSearch);
                        txtSearchUrl.Text = task.FunctionData;
                        break;
                }
            }
        }

        private void SetScreensDataSource()
        {
            edsScreens.WhereParameters.Clear();
            long assetTypeId = Convert.ToInt64(Request.QueryString["AssetTypeId"]);
            var current = AssetTypeRepository.GetById(assetTypeId);
            edsScreens.WhereParameters.Add(new Parameter() { Name = "dynEntityConfigUid", DefaultValue = current.UID.ToString(), Type = TypeCode.Int64 });
        }

        protected void ddlFunctionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedType = Convert.ToInt32((sender as DropDownList).SelectedValue);
            vldSearchUrl.Enabled = false;

            switch ((Enumerations.TaskFunctionType) selectedType)
            {
                case Enumerations.TaskFunctionType.ExecuteSearch:
                    mvFunctions.SetActiveView(viewSearch);
                    vldSearchUrl.Enabled = true;
                    break;
                case Enumerations.TaskFunctionType.LaunchBatch:
                    mvFunctions.SetActiveView(viewBatch);
                    btnSave.OnClientClick = "return CollectParams('" + BatchCommonParams.HiddenFieldId + "');";
                    break;
                case Enumerations.TaskFunctionType.ImportFile:
                    mvFunctions.SetActiveView(viewImport);
                    btnSave.OnClientClick = "return CollectParams('" + importParams.HiddenFieldId + "');";
                    break;
                case Enumerations.TaskFunctionType.ExportFileSSIS:
                    mvFunctions.SetActiveView(viewExportSSIS);
                    btnSave.OnClientClick = "return CollectParams('" + exportParams.HiddenFieldId + "');";
                    break;
                case Enumerations.TaskFunctionType.CreateAsset:
                    mvFunctions.SetActiveView(viewNewAsset);
                    SetScreensDataSource();
                    break;
                case Enumerations.TaskFunctionType.ExecuteSqlServerAgentJob:
                    mvFunctions.SetActiveView(viewExecuteSqlServerAgentJob);
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
            Enumerations.TaskFunctionType selectedType = (Enumerations.TaskFunctionType)Convert.ToInt32(ddlFunctionType.SelectedValue);

            var currentTask = !string.IsNullOrEmpty(Request.QueryString["TaskId"])
                ? TasksService.GetTaskById(Convert.ToInt64(Request.QueryString["TaskId"]), AuthenticationService.CurrentUserId) 
                : new AppFramework.Entities.Task();

            currentTask.DynEntityConfigId = assetTypeId;
            currentTask.Name = txtName.Text;
            currentTask.Description = txtDescription.Text;
            currentTask.FunctionType = (int)selectedType;
            currentTask.IsActive = true;
            currentTask.DisplayInSidebar = chkShowAtSidebar.Checked;

            switch (selectedType)
            {
                case Enumerations.TaskFunctionType.ExecuteSearch:
                    currentTask.ExecutableType = (int)Enumerations.TaskExecutableType.Internal;
                    currentTask.ExecutablePath = null;
                    currentTask.FunctionData = txtSearchUrl.Text;
                    break;

                case Enumerations.TaskFunctionType.LaunchBatch:
                    currentTask.ExecutableType = (int)BatchCommonParams.GetExecutableType();
                    currentTask.ExecutablePath = BatchCommonParams.GetExecutablePath();
                    BatchTaskParametersDescriptor batchData = new BatchTaskParametersDescriptor();
                    batchData.Data = BatchCommonParams.GetFunctionData();
                    currentTask.FunctionData = batchData.Serialize();
                    break;

                case Enumerations.TaskFunctionType.ImportFile:
                    currentTask.ExecutableType = (int)importParams.GetExecutableType();
                    currentTask.ExecutablePath = importParams.GetExecutablePath();
                    ImportTaskParametersDescriptor importData = new ImportTaskParametersDescriptor();
                    importData.Data = importParams.GetFunctionData();
                    importData.FileType = (Enumerations.TaskImportFileType) Convert.ToInt32(ddlOutputFileType.SelectedValue);
                    currentTask.FunctionData = importData.Serialize();
                    break;

                case Enumerations.TaskFunctionType.ExportFileSSIS:
                    currentTask.ExecutableType = (int)exportParams.GetExecutableType();
                    currentTask.ExecutablePath = exportParams.GetExecutablePath();
                    var exportData = new ExportTaskParametersDescriptor();
                    exportData.Data = exportParams.GetFunctionData();
                    currentTask.FunctionData = exportData.Serialize();
                    break;

                case Enumerations.TaskFunctionType.CreateAsset:
                    currentTask.ExecutableType = (int)Enumerations.TaskExecutableType.Internal;
                    currentTask.ExecutablePath = null;
                    NewAssetTaskParametrsDescriptor newAssetData = new NewAssetTaskParametrsDescriptor();
                    if (!string.IsNullOrEmpty(ddlView.SelectedValue) && long.Parse(ddlView.SelectedValue) > 0)
                        newAssetData.ScreenId = long.Parse(ddlView.SelectedValue);
                    currentTask.FunctionData = newAssetData.Serialize();
                    break;

                case Enumerations.TaskFunctionType.ExecuteSqlServerAgentJob:
                    currentTask.ExecutableType = (int)Enumerations.TaskExecutableType.Internal;
                    currentTask.ExecutablePath = null;
                    currentTask.FunctionData = dlAgentJobs.SelectedValue;
                    break;
            }

            TasksService.SaveTask(currentTask, AuthenticationService.CurrentUserId);
            Response.Redirect("TasksList.aspx?AssetTypeId=" + Request.QueryString["AssetTypeId"]);
        }
    }
}