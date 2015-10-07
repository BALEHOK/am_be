using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.DynLists;
using AppFramework.DataProxy;
using AssetSite.Helpers;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.Practices.Unity;

namespace AssetSite.Wizard
{
    public partial class EditAttribute : WizardController
    {
        [Dependency]
        public IDataTypeService DataTypeService { get; set; }
        [Dependency]
        public IDynamicListsService DynamicListsService { get; set; }

        protected AssetTypeAttribute assetTypeAttribute;

        public EditAttribute()
        {
        }

        private List<AssetTypeAttribute> newAttributes
        {
            get
            {
                var list = Session["WizardNewAttributesList"] as List<AssetTypeAttribute>;
                if (list == null)
                {
                    list = new List<AssetTypeAttribute>();
                }
                return list; 
            }
            set
            {
                Session["WizardNewAttributesList"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (AssetType == null)
            {
                Response.Redirect("~/Wizard/Step1.aspx");
            }

            if (!string.IsNullOrEmpty(Request.QueryString["AttrUID"]))
            {
                assetTypeAttribute =
                    AssetType.AllAttributes.Single(a => a.UID == long.Parse(Request.QueryString["AttrUID"]));
            }
            else
            {
                assetTypeAttribute = new AssetTypeAttribute()
                    {
                        DisplayOrder = AssetType.Attributes.Max(a => a.DisplayOrder) + 1
                    };

                var attributesList = this.newAttributes;
                attributesList.Add(assetTypeAttribute);
                this.newAttributes = attributesList;
            }

            if (!IsPostBack)
            {
                var dataTypeService = new DataTypeService(UnitOfWork);
                comboDataType.DataSource = dataTypeService.GetAll().Where(dt => dt.IsInternal == false);
                comboDataType.DataTextField = "Name";
                comboDataType.DataValueField = "UID";
                comboDataType.DataBind();
                comboDataType.Enabled = assetTypeAttribute.AllowEditConfig && newAttributes.Contains(assetTypeAttribute);

                if (assetTypeAttribute.DataType != null)
                {
                    if (assetTypeAttribute.DataTypeEnum == Enumerators.DataType.Assets ||
                        assetTypeAttribute.DataTypeEnum == Enumerators.DataType.Asset ||
                        assetTypeAttribute.DataTypeEnum == Enumerators.DataType.DynList ||
                        assetTypeAttribute.DataTypeEnum == Enumerators.DataType.DynLists)
                    {
                        if (assetTypeAttribute.DataTypeEnum == Enumerators.DataType.Assets)
                        {
                            comboDataType.SelectedValue = dataTypeService.GetByType(Enumerators.DataType.Asset).UID.ToString();
                            chkMultiAssets.Checked = true;
                        }
                        else if (assetTypeAttribute.DataTypeEnum == Enumerators.DataType.DynLists)
                        {
                            comboDataType.SelectedValue = dataTypeService.GetByType(Enumerators.DataType.DynList).UID.ToString();
                            DynListMulti.Checked = true;
                        }
                        else if (assetTypeAttribute.DataTypeEnum == Enumerators.DataType.DynList)
                        {
                            comboDataType.SelectedValue = dataTypeService.GetByType(Enumerators.DataType.DynList).UID.ToString();
                        }

                        ShowAssetTypesList();

                        if (assetTypeAttribute.RelatedAssetTypeID > 0)
                            comboAssetTypes.SelectedValue = assetTypeAttribute.RelatedAssetTypeID.ToString();
                        ShowAssetTypeAttributesList();
                        if (assetTypeAttribute.RelatedAssetTypeAttributeID > 0)
                            comboAssetTypeAttributes.SelectedValue = assetTypeAttribute.RelatedAssetTypeAttributeID.ToString();
                    }
                    else
                    {
                        comboDataType.SelectedValue = assetTypeAttribute.DataType.Base.DataTypeUid.ToString();
                    }
                }
                else
                {
                    comboDataType.SelectedValue = dataTypeService.GetByType(Enumerators.DataType.String).UID.ToString();
                }

                comboDataType_SelectedIndexChanged(comboAssetTypes, null);
                txtName.Text = assetTypeAttribute.Name;
                txtDescription.Text = assetTypeAttribute.Comment;

                chkActive.Checked = assetTypeAttribute.IsActive;
                chkActive.Enabled = assetTypeAttribute.AllowEditConfig;

                chkFinancial.Checked = assetTypeAttribute.IsFinancialInfo;
                chkFinancial.Enabled = assetTypeAttribute.AllowEditConfig;

                var isPredefinedAssetType = UnitOfWork
                    .PredefinedAttributesRepository
                    .SingleOrDefault(pa => pa.DynEntityConfigID == AssetType.ID) != null;

                chkRequired.Checked = assetTypeAttribute.IsRequired;
                chkRequired.Enabled = assetTypeAttribute.AllowEditConfig && !isPredefinedAssetType;

                comboContext.SelectedValue = assetTypeAttribute.ContextId.ToString();
                
                formulaEditor.Visible = assetTypeAttribute.IsCalculated;
                chboxIsFormulaAttribute.Checked = assetTypeAttribute.IsCalculated;
                if (assetTypeAttribute.IsCalculated)
                {
                    formulaText.Text = assetTypeAttribute.FormulaText;
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            (Master as MasterPageWizard).NextButton.Text = Resources.Global.SaveText;
            (Master as MasterPageWizard).PreviousButton.Visible = false;
            (Master as MasterPageWizard).CancelButton.Text = Resources.Global.CancelText;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            (Master as MasterPageWizard).WizardMenu.CurrentStepIndex = 3;
        }

        protected override void btnNext_Click(object sender, EventArgs e)
        {
            if (txtName.Text == String.Empty)
            {
                Alert.Show("Name of attribute cannot be empty");
            }
            else
            {
                if (_saveAttribute())
                {
                    // new attribute
                    if (AssetType.AllAttributes.All(a => a.UID != assetTypeAttribute.UID))
                    {
                        AssetType.Attributes.Add(assetTypeAttribute);
                    }
                    Response.Redirect("~/Wizard/Step3.aspx");
                }
            }
        }

        private bool _saveAttribute()
        {
            assetTypeAttribute.Name = txtName.Text;

            if ((assetTypeAttribute.ID == 0 && AssetType.IsDbColumnExists(assetTypeAttribute.DBTableFieldName)) ||
                AssetType.Attributes.Where(a => a.UID != assetTypeAttribute.UID)
                         .Any(a => a.Name.ToLower() == assetTypeAttribute.Name.ToLower()))
            {
                string error = "Attribute with such name already exists. Please choose another name.";
                try
                {
                    error = GetLocalResourceObject("AttributeNameAlreadyInUse").ToString();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

                lblNameValidationErrors.Text = error;
                lblNameValidationErrors.Visible = true;
                return false;
            }

            assetTypeAttribute.Comment = txtDescription.Text;
            if (assetTypeAttribute.IsActive != chkActive.Checked)
            {
                assetTypeAttribute.IsActive = chkActive.Checked;
                AssetType.ReloadAttributes();
            }

            // save formula
            assetTypeAttribute.FormulaText = formulaText.Text;            
            long dataTypeUid = long.Parse(comboDataType.SelectedValue);
            var dt = DataTypeService.GetByUid(dataTypeUid);
            assetTypeAttribute.DataType = dt;


            if (chkMultiAssets.Checked && phAssetType.Visible)
                assetTypeAttribute.DataType = DataTypeService.GetByType(Enumerators.DataType.Assets);

            if (DynListMulti.Checked && DynLists.Visible)
                assetTypeAttribute.DataType = DataTypeService.GetByType(Enumerators.DataType.DynLists);

            long dynListId;
            if (DynListDropDown.Visible && long.TryParse(DynListDropDown.SelectedValue, out dynListId))
            {
                assetTypeAttribute.DynamicListUid = dynListId;
                assetTypeAttribute.IsDynListValue = true;
            }

            switch (dt.Code)
            {
                    // assiciate Document attribute with predefined id's
                case Enumerators.DataType.Document:
                    PredefinedAttribute predAttr = PredefinedAttribute.Get(PredefinedEntity.Document);
                    assetTypeAttribute.RelatedAssetTypeID = predAttr.DynEntityConfigID;
                    assetTypeAttribute.RelatedAssetTypeAttributeID = predAttr.DynEntityAttribConfigID;
                    break;
                default:
                    long assetTypeID;
                    long assetTypeAttributeID;
                    if (long.TryParse(comboAssetTypes.SelectedValue, out assetTypeID) &&
                        long.TryParse(comboAssetTypeAttributes.SelectedValue, out assetTypeAttributeID) &&
                        assetTypeID > 0 && assetTypeAttributeID > 0)
                    {
                        assetTypeAttribute.RelatedAssetTypeID = assetTypeID;
                        assetTypeAttribute.RelatedAssetTypeAttributeID = assetTypeAttributeID;
                    }
                    break;
            }

            long contextID;
            if (long.TryParse(comboContext.SelectedValue, out contextID))
            {
                assetTypeAttribute.ContextId = contextID;
            }

            assetTypeAttribute.IsFinancialInfo = chkFinancial.Checked;
            assetTypeAttribute.IsRequired = chkRequired.Checked;
            return true;
        }

        protected void comboDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var dataTypeId = long.Parse(comboDataType.SelectedValue);
            var contexts = UnitOfWork.ContextRepository
                .Get(c => c.IsActive && c.DataType.DataTypeUid == dataTypeId, items => items.OrderBy(i => i.Name))
                .Select(record => new EntityContext(record, UnitOfWork))
                .ToList();
            comboContext.DataSource = contexts;
            comboContext.DataTextField = "Name";
            comboContext.DataValueField = "ID";
            comboContext.DataBind();
            comboContext.Enabled = assetTypeAttribute.AllowEditConfig;
            comboContext.Items.Insert(0, new ListItem(Resources.Global.SelectText, string.Empty));
            ShowAssetTypesList();
        }

        private void ShowAssetTypesList()
        {
            long dataTypeUID;
            long.TryParse(comboDataType.SelectedValue, out dataTypeUID);
            var dataType = DataTypeService.GetByUid(dataTypeUID);

            chboxIsFormulaAttribute.Enabled = false;
            phAssetType.Visible = false;
            phAssetTypeAttribute.Visible = false;
            comboAssetTypesValidator.Visible = false;
            comboAssetTypeAttributesValidator.Visible = false;
            DynLists.Visible = false;
            DynListValidator.Visible = false;

            if (dataType != null)
            {
                switch (dataType.Code)
                {
                    case Enumerators.DataType.Asset:
                        phAssetType.Visible = true;
                        phAssetTypeAttribute.Visible = true;
                        comboAssetTypesValidator.Visible = true;
                        comboAssetTypes.DataSource = AssetTypeRepository.GetAllPublished();
                        comboAssetTypes.DataTextField = "Name";
                        comboAssetTypes.DataValueField = "ID";
                        comboAssetTypes.DataBind();
                        break;

                    case Enumerators.DataType.DynList:
                        DynListDropDown.DataSource = DynamicListsService.GetAll();
                        DynListDropDown.DataBind();
                        if (this.assetTypeAttribute != null && this.assetTypeAttribute.DynamicListUid.HasValue)
                        {
                            DynListDropDown.SelectedValue = this.assetTypeAttribute.DynamicListUid.Value.ToString();
                        }
                        DynLists.Visible = true;
                        DynListValidator.Visible = true;
                        break;

                    case Enumerators.DataType.Text:
                    case Enumerators.DataType.String:                    
                    case Enumerators.DataType.Char:
                    case Enumerators.DataType.Int:
                    case Enumerators.DataType.Long:
                    case Enumerators.DataType.Float:
                    case Enumerators.DataType.Money:
                    case Enumerators.DataType.Euro:
                    case Enumerators.DataType.Bool:
                    case Enumerators.DataType.DateTime:
                    case Enumerators.DataType.CurrentDate:
                        chboxIsFormulaAttribute.Enabled = true;
                        break;

                    default:
                        break;
                }
            }
        }

        protected void comboAssetTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowAssetTypeAttributesList();
        }

        private void ShowAssetTypeAttributesList()
        {
            long selectedAssetTypeID;
            long.TryParse(comboAssetTypes.SelectedValue, out selectedAssetTypeID);
            if (phAssetType.Visible)
            {
                var selectedAssetType = AssetTypeRepository.GetById(selectedAssetTypeID);
                phAssetTypeAttribute.Visible = true;
                comboAssetTypeAttributesValidator.Visible = true;

                comboAssetTypeAttributes.DataSource
                    = selectedAssetType.Attributes
                    .Where(a => a.DataTypeEnum != Enumerators.DataType.Asset
                                && a.DataTypeEnum != Enumerators.DataType.Assets
                                && a.IsShownOnPanel);

                comboAssetTypeAttributes.DataTextField = "Name";
                comboAssetTypeAttributes.DataValueField = "ID";
                comboAssetTypeAttributes.DataBind();
                //comboAssetTypeAttributes.Items.Insert(0, new ListItem("", ""));
                if (comboAssetTypeAttributes.Items.Count > 1)
                    comboAssetTypeAttributes.SelectedIndex = 1;
            }
        }

        protected override void btnClose_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Wizard/Step3.aspx");
        }

        protected void chboxIsFormulaAttribute_OnCheckedChanged(object sender, EventArgs e)
        {
            formulaEditor.Visible = chboxIsFormulaAttribute.Checked;
        }
    }
}
