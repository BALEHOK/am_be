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
using BatchCore = AppFramework.Core.Classes.Batch;
using IE = AppFramework.Core.Classes.IE;
using AppFramework.DataProxy;
using AppFramework.Core.Classes.IE.Providers;
using System.Xml;

namespace AssetSite.admin.Import
{
    public partial class Default : ImportController
    {
        [Dependency]
        public BatchCore.IBatchJobFactory BatchJobFactory { get; set; }
        [Dependency]
        public IAttributeFieldFactory AttributeFieldFactory { get; set; }
        [Dependency]
        public IBarcodeProvider BarcodeProvider { get; set; }
        [Dependency]
        public IDynListItemService DynListItemService { get; set; }
        [Dependency]
        public IDynamicListsService  DynamicListsService  { get; set; }
        [Dependency]
        public IExcelProvider ExcelProvider { get; set; }

        private AssetType _assetType;

        /// <summary>
        /// Gets if choosed asset type is type of user.
        /// Then Active Directory DataSource can be shown.
        /// </summary>
        private bool IsUsersImport
        {
            get
            {
                return 
                    AssetType != null && 
                    AssetType.ID == AssetTypeRepository.GetPredefinedAssetType(PredefinedEntity.User).ID;
            }
        }

        /// <summary>
        /// Gets the AssetType choosed for importing data
        /// </summary>
        private AssetType AssetType
        {
            get { return _assetType ?? (_assetType = AssetTypeRepository.GetById(AssetTypeId)); }
        }


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

        #region Session objects

        private IE.BindingInfo Bindings
        {
            get
            {
                if (Session["BindingsImportingWizard"] == null)
                {
                    Session["BindingsImportingWizard"] = new IE.BindingInfo();
                }
                return Session["BindingsImportingWizard"] as IE.BindingInfo;
            }
        }

        private DataSourceType DataSource
        {
            get
            {
                DataSourceType res = DataSourceType.UNKNOWN;
                if (Session["DataSourceImportingWizard"] != null)
                {
                    string ds = Session["DataSourceImportingWizard"].ToString();
                    res = Routines.StringToEnum<DataSourceType>(ds);
                }
                return res;
            }
            set
            {
                Session["DataSourceImportingWizard"] = value.ToString();
            }
        }


        private long AssetTypeId
        {
            get
            {
                long id = 0;
                if (Session["AssetTypeIdImportingWizard"] != null)
                {
                    long.TryParse(Session["AssetTypeIdImportingWizard"].ToString(), out id);
                }
                return id;
            }
            set
            {
                Session["AssetTypeIdImportingWizard"] = value;
            }
        }

        private List<string> Fields
        {
            get
            {
                return Session["FieldsImportingWizard"] as List<string>;
            }
            set
            {
                Session["FieldsImportingWizard"] = value;
            }
        }

        private List<string> Sheets
        {
            get
            {
                return Session["SheetsImportingWizard"] as List<string>;
            }
            set
            {
                Session["SheetsImportingWizard"] = value;
            }
        }

        private string FilePath
        {
            get
            {
                string res = string.Empty;
                if (Session["FilePathImportingWizard"] != null)
                {
                    res = Session["FilePathImportingWizard"].ToString();
                }
                return res;
            }
            set
            {
                Session["FilePathImportingWizard"] = value;
            }
        }

        private IE.LDAPCredentials Credentials
        {
            get
            {
                return Session["CredentialsImportingWizard"] as IE.LDAPCredentials;
            }
            set
            {
                Session["CredentialsImportingWizard"] = value;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Schema downloading
            if (Request.QueryString["schemaid"] != null)
            {
                long assetTypeId = 0;
                long.TryParse(Request.QueryString["schemaid"].ToString(), out assetTypeId);
                AssetType at = AssetType.GetByID(assetTypeId);
                if (at != null)
                {
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
            }
            else if (Request.QueryString["xlsxschemaid"] != null)
            {
                long assetTypeId = 0;
                long.TryParse(Request.QueryString["xlsxschemaid"].ToString(), out assetTypeId);
                AssetType at = AssetType.GetByID(assetTypeId);
                if (at != null)
                {
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
            }
            #endregion

            if (ImportingWizard.ActiveStep == WizardStep1 && !IsPostBack)
            {
                atList.DataSource = AssetTypeRepository.GetAllPublished();
                atList.DataTextField = "Name";
                atList.DataValueField = "ID";
                atList.DataBind();
                atList.Items.Insert(0, new ListItem("", "0"));
                HttpContext.Current.Session["BindingsImportingWizard"] = null;
            }
            else if (AssetTypeId == 0 && ImportingWizard.ActiveStep != WizardStep1)
            {
                Response.Redirect("~/admin/Import");
            }

            if (ImportingWizard.ActiveStep == WizardStep2 && IsPostBack)
            {
                // save choosed by user DataSource 
                if (dataSourceTypesList.SelectedValue != string.Empty)
                {
                    this.DataSource = Routines.StringToEnum<DataSourceType>(dataSourceTypesList.SelectedValue);
                }
            }

            if (ScriptManager.GetCurrent(this).IsInAsyncPostBack && ImportingWizard.ActiveStep == WizardStep4)
            {

                if (DataSource == DataSourceType.XLS || DataSource == DataSourceType.XLSX)
                {
                    ReadExcelFields();
                }
                else
                {
                    throw new NotSupportedException("XML Data Source is not supported");
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

        protected void ImportingWizard_Next(object sender, EventArgs e)
        {
            if (ImportingWizard.ActiveStep == WizardStep1)
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
            else if (ImportingWizard.ActiveStep == WizardStep2)
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
            else if (ImportingWizard.ActiveStep == WizardStep3)
            {
                ReadExcelFields();
            }
        }

        private bool UploadFile()
        {
            if (fileUploadControl.HasFile)
            {
                var filename = fileUploadControl.FileName;
                while (File.Exists(Path.Combine(ApplicationSettings.UploadOnImportPath, filename)))
                {
                    filename = string.Format("{0}-{1}", Guid.NewGuid(), fileUploadControl.FileName);
                }

                FilePath = Path.Combine(ApplicationSettings.UploadOnImportPath, filename);
                fileUploadControl.SaveAs(FilePath);

                // detect file type programmatically
                DataSource = IE.ImportExportManager.GetDataSourceTypeByFileName(fileUploadControl.FileName);
            }
            else
            {
                HoldStep();
                messagePanel.Messages.Add(new MessageDefinition()
                {
                    Status = MessageStatus.Error,
                    Message = "Please select a file to import."
                });
            }
            return fileUploadControl.HasFile;
        }

        private void ReadExcelSheets()
        {
            var result = ExcelProvider.GetExcelSheetNames(FilePath);

            if (ExcelProvider.Status.IsSuccess)
            {
                sheetsCheckboxes.DataSource = result.ToList();
                sheetsCheckboxes.DataBind();

                if (result.Count() == 1)
                {
                    sheetsCheckboxes.Items[0].Selected = true;
                    ReadExcelFields();
                    ImportingWizard.MoveTo(WizardStep4);
                }
                else
                {
                    ImportingWizard.MoveTo(WizardStep3);
                }
            }
            else
            {
                HandleErrorStatus(ExcelProvider.Status);
            }
        }

        private void ReadExcelFields()
        {
            Sheets = GetCheckedSheets().ToList();
            Fields = ExcelProvider.GetFields(FilePath, Sheets);

            if (ExcelProvider.Status.IsSuccess)
            {
                BindData();
                ImportingWizard.MoveTo(WizardStep4);
            }
            else
            {
                HandleErrorStatus(ExcelProvider.Status);
            }
        }

        private void BindData()
        {
            AssetTypeAttributes = new []
            {
                AssetType.Attributes.Single(a => a.Name == AttributeNames.DynEntityId),
                AssetType.Attributes.Single(a => a.Name == AttributeNames.ActiveVersion)
            }
            .Union(AssetType.Attributes.Where(a => a.IsShownOnPanel && a.Editable &&
                                                    a.DataTypeEnum != Enumerators.DataType.File &&
                                                    a.DataTypeEnum != Enumerators.DataType.Image)
            .OrderBy(a => a.DisplayOrder))
            .ToArray();
            fieldsGrid.DataSource = AssetTypeAttributes;
            fieldsGrid.DataBind();
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
                var binding = Bindings.Bindings.SingleOrDefault(b => b.DestinationAttributeId == attributeId)
                              ?? new IE.ImportBinding();

                binding.DestinationAttributeId = attributeId;
                binding.DataSourceFieldName = list.SelectedValue;

                var dlRelatedAssetField = row.FindControl("dlRelatedAssetField") as DropDownList;
                if (assetTypeAttribute.IsUpdateUser)
                {
                    binding.DefaultValue = AuthenticationService.CurrentUserId.ToString();
                }
                else if (assetTypeAttribute.Name == AttributeNames.DynEntityId)
                {
                    binding.DefaultValue = "0";
                }
                else if (assetTypeAttribute.DataTypeEnum == Enumerators.DataType.Barcode)
                {
                    binding.DefaultValue = BarcodeProvider.GenerateBarcode();
                }
                else if ((assetTypeAttribute.DataTypeEnum == Enumerators.DataType.Asset ||
                         assetTypeAttribute.DataTypeEnum == Enumerators.DataType.Document) && 
                        !string.IsNullOrEmpty(dlRelatedAssetField.SelectedValue))
                {
                    binding.DestinationRelatedAttributeId = long.Parse(dlRelatedAssetField.SelectedValue);
                }
                else if (assetTypeAttribute.DBTableFieldName == AttributeNames.ActiveVersion && 
                    string.IsNullOrEmpty(binding.DefaultValue))
                {
                    binding.DefaultValue = "true";
                }

                if (assetTypeAttribute.IsRequired &&
                    (string.IsNullOrEmpty(binding.DataSourceFieldName) || string.IsNullOrEmpty(binding.DefaultValue)) &&
                    !((assetTypeAttribute.Parent.AutoGenerateNameType == Enumerators.TypeAutoGenerateName.InsertOnly ||
                       assetTypeAttribute.Parent.AutoGenerateNameType == Enumerators.TypeAutoGenerateName.InsertUpdate) &&
                       assetTypeAttribute.Name == AttributeNames.Name))
                {
                    result.Status.IsSuccess = false;
                    result.Status.Errors.Add(
                        string.Format(
                            "{0} cannot be empty. Please set both data source binding and default value.",
                            assetTypeAttribute.Name));
                }
                result.Data.Bindings.Add(binding);
            }
            return result;
        }

        protected void fieldsGrid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (Fields.Any() && e.Row.RowType == DataControlRowType.DataRow)
            {
                var attribute = e.Row.DataItem as AssetTypeAttribute;

                // left side
                var nameField = e.Row.FindControl("nameField") as Literal;
                nameField.Text = attribute.Name;
                var lblRequired = e.Row.FindControl("lblRequired") as Label;
                lblRequired.Visible = (attribute.IsRequired &&
                                       !((attribute.Parent.AutoGenerateNameType ==
                                          Enumerators.TypeAutoGenerateName.InsertOnly ||
                                          attribute.Parent.AutoGenerateNameType ==
                                          Enumerators.TypeAutoGenerateName.InsertUpdate)
                                         && attribute.Name == AttributeNames.Name) &&
                                       attribute.Name != AttributeNames.Barcode);

                if (attribute.DataTypeEnum == Enumerators.DataType.Asset)
                {
                    var dlRelatedAssetField = e.Row.FindControl("dlRelatedAssetField") as DropDownList;
                    dlRelatedAssetField.Visible = true;
                    var relatedAssetType = AssetTypeRepository.GetById(attribute.RelatedAssetTypeID.Value);
                    dlRelatedAssetField.DataSource = relatedAssetType.Attributes.Select(a => new {a.ID, a.Name});
                    dlRelatedAssetField.DataTextField = "Name";
                    dlRelatedAssetField.DataValueField = "ID";
                    var displayFieldAttribute =
                        relatedAssetType.Attributes.Single(a => a.ID == attribute.RelatedAssetTypeAttributeID.Value);
                    dlRelatedAssetField.SelectedIndex = relatedAssetType.Attributes.IndexOf(displayFieldAttribute);
                    dlRelatedAssetField.DataBind();
                }

                // right side
                var list = e.Row.FindControl("datasourceField") as DropDownList;
                list.DataSource = from field in Fields
                                  select new { Name = XmlConvert.DecodeName(field), Value = field };
                list.DataValueField = "Value";
                list.DataTextField = "Name";
                list.DataBind();
                list.Items.Insert(0, new ListItem());

                var attributeName = fieldsGrid.DataKeys[e.Row.RowIndex].Value.ToString().ToLower();
                var matches = from field in Fields
                              let decoded = XmlConvert.DecodeName(field)
                              where decoded.ToLower().Contains(attributeName)
                              select field;

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
                    !(attribute.Name == AttributeNames.Barcode))
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
            ImportingWizard.ActiveStepIndex--;
            ImportingWizard.ActiveStepIndex++;
        }

        private IEnumerable<string> GetCheckedSheets()
        {
            foreach (ListItem item in sheetsCheckboxes.Items)
            {
                if (item.Selected) yield return item.Text;
            }
        }

        protected void ImportingWizard_Finish(object sender, EventArgs e)
        {
            BindData();

            var path = FilePath;
            var status = new IE.StatusInfo();
            var bindingsResult = GetBindings();
            status.Add(bindingsResult.Status);

            if (status.IsSuccess)
            {
                var job = BatchJobFactory.CreateImportAssetsJob(
                    AuthenticationService.CurrentUserId,
                    path, AssetTypeId, bindingsResult.Data, Sheets, true);
                CleanSession();
                Response.Redirect(job.NavigateUrl);
            }
            else
            {
                HandleErrorStatus(status);
            }
        }

        private void CleanSession()
        {
            int i = 0;
            while (i < Session.Keys.Count)
            {
                if (Session.Keys[i].EndsWith("ImportingWizard"))
                {
                    Session.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        protected void ImportingWizard_Cancel(object sender, EventArgs e)
        {
            Response.Redirect("~/admin/");
        }

        protected void testCnnButton_Click(object sender, EventArgs e)
        {
            TestLDAPConnection();
        }

        private void TestLDAPConnection()
        {
            Credentials = GetCredentials();
            IE.ActionResult<bool> result = IE.ImportExportManager.CheckADConnection(Credentials);
            if (result.Status.IsSuccess)
            {
                messagePanel.Messages.Add(new MessageDefinition()
                {
                    Message = "Success",
                    Status = MessageStatus.Normal
                });
            }
            else
            {
                HandleErrorStatus(result.Status);
            }
        }

        private void HandleErrorStatus(IE.StatusInfo status)
        {
            messagePanel.Messages.Add(status);
            HoldStep();
        }

        private IE.LDAPCredentials GetCredentials()
        {
            var credentials = new IE.LDAPCredentials
            {
                Domain = domainNameTextbox.Text,
                UserName = userNameTextbox.Text,
                Password = passwordTextbox.Text
            };
            return credentials;
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
            var bindings = HttpContext.Current.Session["BindingsImportingWizard"] as IE.BindingInfo ??
                           new IE.BindingInfo();
            bindings.Bindings.Add(new IE.ImportBinding
            {
                DestinationAttributeId = index,
                DataSourceFieldName = value
            });
            HttpContext.Current.Session["BindingsImportingWizard"] = bindings;
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
                    case Enumerators.DataType.DynList:
                        long listuid;
                        result = long.TryParse(value, out listuid);
                        if (result)
                        {
                            var service = new DynListItemService(new UnitOfWork());
                            var list = service.GetByUid(listuid);
                            if (list != null)
                                value = list.ID.ToString();
                        }
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
