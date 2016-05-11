using System;
using System.Collections.Generic;
using System.IO;
using AppFramework.ConstantsEnumerators;
using System.Web.UI.WebControls;
using System.Linq;

namespace AssetSite.Controls
{
    public partial class TaskCommonParams : System.Web.UI.UserControl
    {
        public bool OnlySSIS { get; set; }
        public AppFramework.Tasks.Enumerations.TaskExecutableType ExecutableType { get; set; }
        public string ExecutablePath { get; set; }
        public List<KeyValuePair<string, string>> FunctionData { get; set; }
        public string HiddenFieldId
        {
            get { return hfldAddedParams.ClientID; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (OnlySSIS)
            {
                ddlExecType.SelectedIndex = 0;
                ddlExecType.Enabled = false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack &&
                !String.IsNullOrEmpty(Request.QueryString["Edit"]) &&
                !String.IsNullOrEmpty(Request.QueryString["TaskId"]))
            {
                lblExecutablePath.Text = this.ExecutablePath;
                ddlExecType.SelectedValue = ((int)this.ExecutableType).ToString();
                if (FunctionData != null)
                {
                    this.RestoreFunctionData(this.FunctionData);
                }
                fuExecValidator.Visible = string.IsNullOrEmpty(ExecutablePath);
            }
            if (ddlExecType.SelectedValue == "3")
            {
                RegularParams.Visible = false;
                PredefinedTaskParams.Visible = true;
            }
            else
            {
                RegularParams.Visible = true;
                PredefinedTaskParams.Visible = false;
            }
        }

        public string GetExecutablePath()
        {
            if (ddlExecType.SelectedValue == "3")
            {
                return DropDownListTasks.SelectedValue;
            }

            string path = lblExecutablePath.Text;
            if (fuExec.HasFile)
            {
                string rootPath = "~/App_Data/";
                if (ddlExecType.SelectedValue == "1")
                    rootPath += "Exec/";
                else
                    rootPath += "Packages/";

                if (!Directory.Exists(Server.MapPath(rootPath)))
                    Directory.CreateDirectory(Server.MapPath(rootPath));

                rootPath += DateTime.Now.ToString("yyyyMMdd_HHMMss_") + fuExec.FileName;
                fuExec.SaveAs(Server.MapPath(rootPath));
                path = rootPath;
            }
            return path;
        }

        public List<KeyValuePair<string, string>> GetFunctionData()
        {
            string[] pairs = hfldAddedParams.Value.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();
            foreach (string pair in pairs)
            {
                string[] parts = pair.Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length > 1)
                    result.Add(new KeyValuePair<string, string>(parts[0], string.Join(":", parts.Skip(1).ToList())));
            }
            return result;
        }

        public AppFramework.Tasks.Enumerations.TaskExecutableType GetExecutableType()
        {
            return (AppFramework.Tasks.Enumerations.TaskExecutableType)Convert.ToInt32(ddlExecType.SelectedValue);
        }

        private void RestoreFunctionData(List<KeyValuePair<string, string>> taskParams)
        {
            if (taskParams != null)
            {
                string script = "";
                foreach (KeyValuePair<string, string> pair in taskParams)
                {
                    script += String.Format("AddParamWithValue('{0}','{1}');", pair.Key, Server.UrlEncode(pair.Value));
                }
                litScript.Text = "<script>" + script + "</script>";
            }
        }

        protected void OnFileUploadValidation(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = false;
            if (!string.IsNullOrEmpty(args.Value))
            {
                string ext = args.Value.Split('.').Last().ToLower();
                if (OnlySSIS)
                {
                    fuExecFileExtValidator.Text = "File type must be DTSX";
                    if (new string[] { "dtsx" }.Contains(ext))
                        args.IsValid = true;
                }
                else
                {
                    fuExecFileExtValidator.Text = "File type must be EXE or DTSX";
                    if (new string[] { "exe", "dtsx" }.Contains(ext))
                        args.IsValid = true;
                }
            }
        }
    }
}