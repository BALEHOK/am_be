namespace AssetSite.admin.Reports.Add
{
    using System;
    using System.IO;
    using System.Web.UI;
    using AppFramework.Core.Classes;

    public partial class SelectTemplate : ReportPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //FinTmplLinks.Visible = this.report.IsFinancial;
            CurrentTemplate.Text = this.report.Template;
            AssetTypeName.Text = this.report.AssetType.Name;
            if (!Page.IsPostBack)
            { 
                SyncWithCurrentItem.Checked = this.report.SyncWithItem;
            }

            Page.DataBind();
        }


        protected void SetTemplateClick(object sender, EventArgs e)
        {
            if (TemplateFile.HasFile)
            {
                string templName = TemplateFile.FileName;
                if (!ValidateTemplate(templName))
                {
                    var message = GetLocalResourceObject("WrongTemplate").ToString().Replace("'", @"\'");
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "alert_wrong_template", "alert('" + message + "')", true);
                    return;
                }

                int num = 0;
                string fileName = Server.MapPath(ApplicationSettings.TemplatesPath) + templName;

                while (File.Exists(fileName))
                {
                    num++;
                    string nName = string.Empty;
                    string name = Path.GetFileNameWithoutExtension(templName);
                    if (!System.Text.RegularExpressions.Regex.Match(name,
                            @"[(]\d+[)]$").Success)   // for the first time just add ordinal number to file name
                    {
                        nName = name + string.Format("({0})", num);
                    }
                    else
                    {
                        nName = System.Text.RegularExpressions.Regex.Replace(name,
                            @"[(]\d+[)]$",
                            string.Format("({0})", num));
                    }
                    templName = nName + Path.GetExtension(templName);
                    fileName = Server.MapPath(ApplicationSettings.TemplatesPath) + templName;
                }

                TemplateFile.SaveAs(fileName);
                this.report.Template = templName;
            }
            else
            {
                if (string.IsNullOrEmpty(this.report.Template))
                {
                    this.report.Template = "Draft";
                }
            }

            this.report.SyncWithItem = SyncWithCurrentItem.Checked;
            this.report.Save();
            Response.Redirect("~/admin/Reports/Default.aspx");
        }

        private bool ValidateTemplate(string templName)
        {
            return !string.IsNullOrEmpty(templName) && templName.EndsWith(".rpt");
        }
    }
}
