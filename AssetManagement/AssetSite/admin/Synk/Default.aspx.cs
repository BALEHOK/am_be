using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Barcode;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.PL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.Unity;
using IE = AppFramework.Core.Classes.IE;

namespace AssetSite.admin.Synk
{
    public partial class Default : BasePage
    {
        [Dependency]
        public IBarcodeProvider BarcodeProvider { get; set; }
        [Dependency]
        public IE.IImportExportManager ImportExportManager { get; set; }

        /// <summary>
        /// Gets if choosed asset type is type of user.
        /// Then Active Directory DataSource can be shown.
        /// </summary>
        private bool IsUsersImport
        {
            get
            {
                return AssetType != null && 
                    AssetType.ID == AssetTypeRepository.GetPredefinedAssetType(PredefinedEntity.User).ID;
            }
        }

        /// <summary>
        /// Gets the AssetType choosed for importing data
        /// </summary>
        private AssetType AssetType
        {
            get
            {
                return _assetType ?? (_assetType = AssetTypeRepository.GetById(AssetTypeId));
            }
        }
        private AssetType _assetType;

        /// <summary>
        /// Get and set AssetTypeAttributes
        /// </summary>
        private static AssetTypeAttribute[] AssetTypeAttributes
        {
            get
            {
                return HttpContext.Current.Session["AssetTypeAttributes"] == null ? null :
                    HttpContext.Current.Session["AssetTypeAttributes"] as AssetTypeAttribute[];
            }
            set
            {
                HttpContext.Current.Session["AssetTypeAttributes"] = value;
            }
        }

        private IE.BindingInfo Bindings
        {
            get
            {
                if (Session["BindingsSynkWizard"] == null)
                {
                    Session["BindingsSynkWizard"] = new IE.BindingInfo();
                }
                return Session["BindingsSynkWizard"] as IE.BindingInfo;
            }
        }

        private DataSourceType DataSource
        {
            get
            {
                DataSourceType res = DataSourceType.UNKNOWN;
                if (Session["DataSourceSynkWizard"] != null)
                {
                    string ds = Session["DataSourceSynkWizard"].ToString();
                    res = Routines.StringToEnum<DataSourceType>(ds);
                }
                return res;
            }
            set
            {
                Session["DataSourceSynkWizard"] = value.ToString();
            }
        }


        private long AssetTypeId
        {
            get
            {
                long id = 0;
                if (Session["AssetTypeIdSynkWizard"] != null)
                {
                    long.TryParse(Session["AssetTypeIdSynkWizard"].ToString(), out id);
                }
                return id;
            }
            set
            {
                Session["AssetTypeIdSynkWizard"] = value;
            }
        }

        private List<string> Fields
        {
            get
            {
                return Session["FieldsSynkWizard"] as List<string>;
            }
            set
            {
                Session["FieldsSynkWizard"] = value;
            }
        }

        private List<string> Sheets
        {
            get
            {
                return Session["SheetsSynkWizard"] as List<string>;
            }
            set
            {
                Session["SheetsSynkWizard"] = value;
            }
        }

        private string FilePath
        {
            get
            {
                string res = string.Empty;
                if (Session["FilePathSynkWizard"] != null)
                {
                    res = Session["FilePathSynkWizard"].ToString();
                }
                return res;
            }
            set
            {
                Session["FilePathSynkWizard"] = value;
            }
        }

        private IE.LDAPCredentials Credentials
        {
            get
            {
                return Session["CredentialsSynkWizard"] as IE.LDAPCredentials;
            }
            set
            {
                Session["CredentialsSynkWizard"] = value;
            }
        }
      
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["schemaid"] != null)
            {
                long assetTypeId = 0;
                long.TryParse(Request.QueryString["schemaid"].ToString(), out assetTypeId);
                var at = AssetTypeRepository.GetById(assetTypeId);
                // generate schema
                IE.SchemaGenerator sg = new IE.SchemaGenerator(at);
                string schema = sg.GetSchemaAsString();

                // send schema to browser
                Response.Clear();
                Response.ContentType = "application/xml";
                Response.Charset = "utf-8";
                Response.AddHeader("Content-Disposition",
                    string.Format("attachment;filename=schema_{0}.xsd", assetTypeId));
                Response.Write(schema);
                Response.End();
            }
            else if (Request.QueryString["xlsxschemaid"] != null)
            {
                long assetTypeId = 0;
                long.TryParse(Request.QueryString["xlsxschemaid"].ToString(), out assetTypeId);
                var at = AssetTypeRepository.GetById(assetTypeId);
                string tempFilePath = string.Empty;

                // generate schema
                IE.SchemaGenerator sg = new IE.SchemaGenerator(at);
                Stream stream = sg.GetXlsxSchemaAsStream(Server.MapPath("~"), out tempFilePath);
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Close();
                File.Delete(tempFilePath);

                // send schema to browser
                Response.Clear();
                Response.ContentType = "application/x-msexcel";
                Response.AddHeader("Content-Disposition",
                    string.Format("attachment;filename=schema_{0}.xlsx", assetTypeId));
                Response.BinaryWrite(buffer);
                Response.End();

                Response.BinaryWrite(buffer);
                Response.End();
            }

            if (SynkWizard.ActiveStep == WizardStep1 && !IsPostBack)
            {
                atList.DataSource = AssetTypeRepository.GetAllPublished();
                atList.DataTextField = "Name";
                atList.DataValueField = "ID";
                atList.DataBind();
                atList.Items.Insert(0, new ListItem("", "0"));
                HttpContext.Current.Session["BindingsSynkWizard"] = null;
            }

            if (SynkWizard.ActiveStep == WizardStep2 && IsPostBack)
            {
                // save choosed by user DataSource 
                if (dataSourceTypesList.SelectedValue != string.Empty)
                {
                    DataSource = Routines.StringToEnum<DataSourceType>(dataSourceTypesList.SelectedValue);
                }
            }

            if (ScriptManager.GetCurrent(this).IsInAsyncPostBack && SynkWizard.ActiveStep == WizardStep4)
            {
                if (DataSource == DataSourceType.XLS || DataSource == DataSourceType.XLSX)
                {
                    ReadExcelFields();
                }
                else
                {
                    throw new NotSupportedException("Only Excel files supported for synchronization");
                }
            }
        }

        protected void atList_Changed(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(atList.Text))
            {
                schemaLink.Visible = false;
                xlsxSchemaLink.Visible = false;
                return;
            }

            schemaLink.Visible = true;
            xlsxSchemaLink.Visible = true;

            schemaLink.NavigateUrl = string.Format("~/admin/Import/Default.aspx?schemaid={0}", atList.SelectedValue);
            xlsxSchemaLink.NavigateUrl = string.Format("~/admin/Import/Default.aspx?xlsxschemaid={0}", atList.SelectedValue);

            long id = 0;
            long.TryParse(atList.SelectedValue, out id);
            AssetTypeId = id;
        }

        protected void SynkWizard_Next(object sender, EventArgs e)
        {
            if (SynkWizard.ActiveStep == WizardStep1)
            {
                long atId = 0;
                long.TryParse(atList.SelectedValue, out atId);
                if (atId > 0)
                {
                    AssetTypeId = atId;
                }
                else
                {
                    HoldStep();

                    messagePanel.Messages.Add(new MessageDefinition()
                    {
                        Status = MessageStatus.Error,
                        Message = "Please choose asset type for import."
                    });
                }
            }
            else if (SynkWizard.ActiveStep == WizardStep2)
            {
                if (DataSource != DataSourceType.AD)
                {
                    if (UploadFile())
                    {
                        if (DataSource == DataSourceType.XLS ||
                            DataSource == DataSourceType.XLSX)
                        {
                            ReadExcelSheets();
                        }
                        else
                        {
                            HoldStep();
                            messagePanel.Messages.Add(new MessageDefinition()
                            {
                                Status = MessageStatus.Error,
                                Message = "File format is not supported."
                            });
                        }
                    }
                }
                else if (DataSource == DataSourceType.AD && IsUsersImport)
                {
                    throw new NotSupportedException("Only Excel files supported for synchronization");
                }
            }
            else if (SynkWizard.ActiveStep == WizardStep3)
            {
                ReadExcelFields();
            }
        }

        private bool UploadFile()
        {
            if (!string.IsNullOrEmpty(showSharedResources.FileName))
            {
                FilePath = showSharedResources.FileName;
                DataSource = IE.ImportExportManager.GetDataSourceTypeByFileName(FilePath);
                return true;
            }
            else
            {
                HoldStep();
                messagePanel.Messages.Add(new MessageDefinition()
                {
                    Status = MessageStatus.Error,
                    Message = "Please select a file to import."
                });

                return false;
            }
           
        }

        private void ReadExcelSheets()
        {
            var result
                = IE.ImportExportManager.GetExcelDataSourceSheets(FilePath);

            if (result.Status.IsSuccess)
            {
                sheetsCheckboxes.DataSource = result.DataSet.ToList();
                sheetsCheckboxes.DataBind();

                if (result.DataSet.Count() == 1)
                {
                    sheetsCheckboxes.Items[0].Selected = true;
                    ReadExcelFields();
                    SynkWizard.MoveTo(WizardStep4);
                }
                else
                {
                    SynkWizard.MoveTo(WizardStep3);
                }
            }
            else
            {
                HandleErrorStatus(result.Status);
            }
        }
        
        private void ReadExcelFields()
        {
            Sheets = GetCheckedSheets().ToList();

            var result
                = IE.ImportExportManager.GetExcelDataSourceFields(FilePath, Sheets);

            if (result.Status.IsSuccess)
            {
                Fields = (from field in result.DataSet
                          select field.Value).ToList();
                BindData();
                SynkWizard.MoveTo(WizardStep4);
            }
            else
            {
                HandleErrorStatus(result.Status);
            }
        }

        private void BindData()
        {
            if (AssetType.DBTableName == Enumerators.DBTableNames.ADynEntityUser.ToString())
            {
                AssetTypeAttributes = 
                    AssetType.Attributes.Where(
                        a => 
                        a.IsShownOnPanel && a.Editable &&
                        ((a.Name != AttributeNames.Name) || (a.Name == AttributeNames.Name && a.Parent.AutoGenerateNameType == Enumerators.TypeAutoGenerateName.None)) &&
                        a.DataTypeEnum != Enumerators.DataType.Password &&
                        a.DataTypeEnum != Enumerators.DataType.Role &&
                        a.DataTypeEnum != Enumerators.DataType.Permission &&
                        a.DataTypeEnum != Enumerators.DataType.File && 
                        a.DataTypeEnum != Enumerators.DataType.Image).
                        OrderBy(a => a.DisplayOrder).
                        ThenBy(a => a.DisplayOrder)
                        .ToArray();
            }
            else
            {
                AssetTypeAttributes = AssetType.Attributes.Where(
                        a => a.IsShownOnPanel && a.Editable &&
                        a.DataTypeEnum != Enumerators.DataType.File && 
                        a.DataTypeEnum != Enumerators.DataType.Image).
                        OrderBy(a => a.DisplayOrder).
                        ThenBy(a => a.DisplayOrder)
                        .ToArray();
            }

            fieldsGrid.DataSource = AssetTypeAttributes;
            fieldsGrid.DataBind();

            synchronizationFieldList.DataSource = AssetTypeAttributes;
            synchronizationFieldList.DataTextField = "Name";
            synchronizationFieldList.DataValueField = "DBTableFieldName";
            synchronizationFieldList.DataBind();
        }

        private IE.ActionResult<IE.BindingInfo> GetBindings()
        {
            var result = new IE.ActionResult<IE.BindingInfo>();
            foreach (GridViewRow row in fieldsGrid.Rows)
            {
                var ataId = row.FindControl("ATA_ID") as HiddenField;
                var list = row.FindControl("datasourceField") as DropDownList;

                var attributeId = long.Parse(ataId.Value);
                var assetTypeAttribute = AssetType.Attributes.SingleOrDefault(a => a.ID == attributeId);
                var binding = Bindings.Bindings.SingleOrDefault(b => b.DestinationAttributeId == attributeId) ??
                              new IE.ImportBinding
                              {
                                  DestinationAttributeId = attributeId,
                                  DataSourceFieldName = list.SelectedValue
                              };

                if (assetTypeAttribute.IsUpdateUser)
                    binding.DefaultValue = AuthenticationService.CurrentUserId.ToString();
                else if (assetTypeAttribute.DataTypeEnum == Enumerators.DataType.Barcode)
                    binding.DefaultValue = BarcodeProvider.DefaultValue;
                else if (assetTypeAttribute.DataTypeEnum == Enumerators.DataType.Asset ||
                         assetTypeAttribute.DataTypeEnum == Enumerators.DataType.Document)
                {
                    var dlRelatedAssetField = row.FindControl("dlRelatedAssetField") as DropDownList;
                    binding.DestinationRelatedAttributeId = long.Parse(dlRelatedAssetField.SelectedValue);
                }

                if (assetTypeAttribute.IsRequired && string.IsNullOrEmpty(binding.DefaultValue))
                {
                    result.Status.IsSuccess = false;
                    result.Status.Errors.Add(
                        string.Format(
                            "The field {0} cannot be empty. Please set a data source binding or provide a fill value.",
                            assetTypeAttribute.Name));
                }

                //if IsRequired then it is possible that data from AD comes empty. But is required !!!
                //Thus setting default value for IsRequired fields is obligatory
                if (string.IsNullOrEmpty(binding.DefaultValue) &&
                    (assetTypeAttribute.IsRequired &&
                     !((assetTypeAttribute.Parent.AutoGenerateNameType ==
                        Enumerators.TypeAutoGenerateName.InsertOnly ||
                        assetTypeAttribute.Parent.AutoGenerateNameType ==
                        Enumerators.TypeAutoGenerateName.InsertUpdate) &&
                       assetTypeAttribute.Name == AttributeNames.Name) &&
                     !(assetTypeAttribute.Name == AttributeNames.Barcode ||
                       assetTypeAttribute.Name == AttributeNames.DynEntityId)))
                {
                    result.Status.IsSuccess = false;
                    result.Status.Errors.Add(string.Format("{0} default value is required",
                        assetTypeAttribute.Name));
                }
                result.Data.Bindings.Add(binding);
            }
            return result;
        }

        protected void fieldsGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (Fields.Count() > 0 && e.Row.RowType == DataControlRowType.DataRow)
            {
                AssetTypeAttribute attribute = e.Row.DataItem as AssetTypeAttribute;

                Literal nameField = e.Row.FindControl("nameField") as Literal;
                nameField.Text = string.Format("{0}{1}", attribute.NameLocalized, 
                    (
                     attribute.IsRequired 
                     && !((attribute.Parent.AutoGenerateNameType == Enumerators.TypeAutoGenerateName.InsertOnly || attribute.Parent.AutoGenerateNameType == Enumerators.TypeAutoGenerateName.InsertUpdate) && attribute.Name == AttributeNames.Name)
                     && !(attribute.Name == AttributeNames.Barcode)
                     && !(attribute.Name == AttributeNames.Email)) ? "*" : string.Empty);

                DropDownList list = e.Row.FindControl("datasourceField") as DropDownList;

                list.DataSource = Fields;
                list.DataBind();
                list.Items.Insert(0, new ListItem());

                string cValue = fieldsGrid.DataKeys[e.Row.RowIndex].Value.ToString().ToLower();

                //exact match
                IEnumerable<string> matches = from attr in Fields
                                              where attr.ToLower() == cValue
                                              select attr;
                if (matches.Count() == 0)
                {
                    //rough match
                    matches = from attr in Fields
                              where attr.ToLower().Contains(cValue)
                              select attr;
                }
                var binding = Bindings.Bindings.SingleOrDefault(b => b.DestinationAttributeId == attribute.ID);
                list.SelectedValue = binding != null 
                    ? binding.DataSourceFieldName 
                    : matches.FirstOrDefault();

                PlaceHolder pl = e.Row.Cells[e.Row.Cells.Count - 1].FindControl("defControl") as PlaceHolder;
                Control ctrl;

                if (pl != null &&
                    !((attribute.Parent.AutoGenerateNameType == Enumerators.TypeAutoGenerateName.InsertOnly ||
                    attribute.Parent.AutoGenerateNameType == Enumerators.TypeAutoGenerateName.InsertUpdate) &&
                    attribute.Name == AttributeNames.Name) &&
                    !(attribute.Name == AttributeNames.Barcode)
                    )
                {
                    var asset = AssetsService.CreateAsset(AssetType);
                    AssetAttribute bindedAttr = asset.Attributes.Single(a => a.GetConfiguration().ID == attribute.ID);
                    ctrl = AttributeFieldFactory.GetControl(bindedAttr, true, false) as Control;
                    pl.Controls.Add(ctrl);
                }
                else
                {
                    e.Row.Cells[e.Row.Cells.Count - 1].FindControl("ctrlWrapper").Visible = false;
                }

                var ltAction = e.Row.FindControl("ltAction") as Literal;
                if (binding != null && !string.IsNullOrEmpty(binding.DefaultValue))
                {
                    ltAction.Text =
                        string.Format(
                            "<span class=\"label\">{0}</span><a href=\"javascript:void(0);\" onclick=\"RemoveValue({1})\" class=\"cmd\">{2}</a>",
                            binding.DefaultValue, e.Row.DataItemIndex, GetLocalResourceObject("linkRemove") ?? "Remove");
                }
                else
                {
                    ltAction.Text =
                        string.Format(
                            "<a href=\"javascript:void(0);\" class=\"cmd\" onclick=\"toggleDefault(" +
                            e.Row.DataItemIndex +
                            ");\"> {0}</a>", GetLocalResourceObject("linkAdd") ?? "Add");
                }
            }
        }

        private void HoldStep()
        {
            SynkWizard.ActiveStepIndex--;
            SynkWizard.ActiveStepIndex++;
        }

        private IEnumerable<string> GetCheckedSheets()
        {
            foreach (ListItem item in sheetsCheckboxes.Items)
            {
                if (item.Selected) yield return item.Text;
            }
        }

        protected void SynkWizard_Finish(object sender, EventArgs e)
        {
            string path = string.Empty;
            IE.StatusInfo status = new IE.StatusInfo();
            IE.ActionResult<IE.BindingInfo> bindingsResult = GetBindings();
            status.Add(bindingsResult.Status);

            if (status.IsSuccess)
            {
                if (DataSource == DataSourceType.XLS ||
                    DataSource == DataSourceType.XLSX)
                {
                    //path = Path.Combine(ApplicationSettings.UploadOnImportPath, Guid.NewGuid() + ".xml");
                    //status.Add(IE.ImportExportManager.ExportExcelToXml(FilePath, AssetType, bindingsResult.Data, Sheets, path));
                    //if (status.IsSuccess)
                    //{
                    //    File.Delete(FilePath);
                    //    FilePath = path;  // refresh state in session
                    //}
                }
                else if (DataSource == DataSourceType.AD)
                {
                    path = Path.Combine(ApplicationSettings.UploadOnImportPath, Guid.NewGuid() + ".xml");
                    bindingsResult.Data = IE.ImportExportManager.ConvertLDAPBindings(bindingsResult.Data);
                    status.Add(IE.ImportExportManager.SaveLDAPUsersToXML(Credentials, bindingsResult.Data, AssetType, path));
                }
                else if (DataSource == DataSourceType.XML)
                {
                    path = FilePath;
                }
            }

            if (status.IsSuccess)
            {
                var job = ImportExportManager.SynkAssets(
                    FilePath, AssetTypeId, bindingsResult.Data, synchronizationFieldList.SelectedItem.Value, Sheets, false);
                CleanSession();
                Response.Redirect(job.NavigateUrl);
            }
            else
            {
                HandleErrorStatus(status);
                BindData();
            }
        }

        private void CleanSession()
        {
            int i = 0;
            while (i < Session.Keys.Count)
            {
                if (Session.Keys[i].EndsWith("SynkWizard"))
                {
                    Session.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        protected void SynkWizard_Cancel(object sender, EventArgs e)
        {
            Response.Redirect("~/admin/");
        }

        protected void testCnnButton_Click(object sender, EventArgs e)
        {
            throw  new NotImplementedException();
        }

        private void HandleErrorStatus(IE.StatusInfo status)
        {
            messagePanel.Messages.Add(status);
            HoldStep();
        }
        
        protected void fileUploadControl_Validate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = !string.IsNullOrEmpty(e.Value);
        }

        /// <summary>
        /// Adds the binding value 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [WebMethod]
        public static bool AssignBinding(int index, string value)
        {
            IE.BindingInfo bindings
                = HttpContext.Current.Session["BindingsSynkWizard"] as IE.BindingInfo;
            if (bindings == null)
                bindings = new IE.BindingInfo();
            bindings.Bindings.Add(new IE.ImportBinding
            {
                DestinationAttributeId = index,
                DataSourceFieldName = value
            });
            HttpContext.Current.Session["BindingsSynkWizard"] = bindings;
            return true;
        }

        /// <summary>
        /// Adds default value to dictionary
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [WebMethod]
        public static bool AssignValue(int index, string value)
        {
            var result = true;
            var bindings = HttpContext.Current.Session["BindingsImportingWizard"] as IE.BindingInfo ??
                           new IE.BindingInfo();

            var attribute = AssetTypeAttributes.Length > index
                ? AssetTypeAttributes[index]
                : null;
            if (attribute == null)
                throw new ArgumentException();
            var binding = bindings.Bindings.SingleOrDefault(b => b.DestinationAttributeId == attribute.ID);

            switch (attribute.DataTypeEnum)
            {
                case Enumerators.DataType.Bool:
                    bool databool;
                    result = bool.TryParse(value, out databool);
                    break;
                case Enumerators.DataType.Float:
                    float datafloat;
                    result = float.TryParse(value, out datafloat);
                    break;
                case Enumerators.DataType.Int:
                    int dataint;
                    result = int.TryParse(value, out dataint);
                    break;
                case Enumerators.DataType.Long:
                    long datalong;
                    result = long.TryParse(value, out datalong);
                    break;
            }


            if (result)
            {
                if (binding == null)
                {
                    bindings.Bindings.Add(new IE.ImportBinding
                    {
                        DestinationAttributeId = attribute.ID,
                        DefaultValue = value.Trim()
                    });
                }
                else
                {
                    binding.DefaultValue = value.Trim();
                }
            }

            HttpContext.Current.Session["BindingsImportingWizard"] = bindings;
            return result;
        }

        [WebMethod]
        public static bool RemoveValue(int index)
        {
            var bindings = HttpContext.Current.Session["BindingsImportingWizard"] as IE.BindingInfo;
            var attribute = AssetTypeAttributes.Length > index
                ? AssetTypeAttributes[index]
                : null;
            if (attribute == null)
                throw new ArgumentException();
            var binding = bindings.Bindings.SingleOrDefault(b => b.DestinationAttributeId == attribute.ID);
            if (binding != null)
                binding.DefaultValue = "";
            HttpContext.Current.Session["BindingsImportingWizard"] = bindings;
            return true;
        }
    }
}
