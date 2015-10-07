using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Core.PL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AppFramework.DataProxy;
using Microsoft.Practices.Unity;

namespace AssetSite.Controls
{
    public partial class NewAttributePanel : UserControl, IAssetAttributesPanel
    {
        [Dependency]
        public IAuthenticationService AuthenticationService { get; set; }
        [Dependency]
        public IAssetsService AssetsService { get; set; }
        [Dependency]
        public IAssetTypeRepository AssetTypeRepository { get; set; }
        [Dependency]
        public IUnitOfWork UnitOfWork { get; set; }
        [Dependency]
        public IAttributeFieldFactory AttributeFieldFactory { get; set; }

        public string Header { get; set; }
        public string CssClass { get; set; }
        public List<AssetAttribute> AssignedAttributes { get; set; }
        public bool Editable { get; set; }
        public bool MySettingsPage { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            GridAttributes.RowDataBound += new GridViewRowEventHandler(GridAttributes_RowDataBound);
            GridAttributes.DataSource = AssignedAttributes
                .Where(a => a.GetConfiguration().IsShownOnPanel);
//            GridAttributes.DataSource = AssignedAttributes.ToList();
            GridAttributes.DataBind();
        }

        public string TranslatedHeader
        {
            get
            {
                return (new TranslatableString(this.Header).GetTranslation());
            }
        }

        private void GridAttributes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow && e.Row.DataItem as AssetAttribute != null)
            {
                var attribute = e.Row.DataItem as AssetAttribute;
                if (attribute.GetConfiguration().IsMultipleAssets && !Editable)
                {
                    e.Row.Cells[0].Visible = false;
                    e.Row.Cells[1].ColumnSpan = 2;
                    e.Row.Cells[1].Controls.Add(new LiteralControl(string.Format("<b class=\"{1}\">{0}</b>",
                        attribute.GetConfiguration().NameLocalized, "multiple-assets-padding")));
                }

                var localEditableSetting = this.Editable;
                var permission = AuthenticationService.GetPermission(attribute.ParentAsset);
                if (attribute.GetConfiguration().IsFinancialInfo)
                {
                    if (Editable && permission.CanWrite(true))
                    {
                        AddControlToRow(e, attribute, localEditableSetting);
                    }
                    else
                    {
                        if (permission.CanRead(true))
                        {
                            AddControlToRow(e, attribute, false);
                        }
                        else
                        {
                            e.Row.Cells[0].Visible = false;
                            e.Row.Cells[1].Visible = false;
                        }
                    }
                }
                else
                {
                    AddControlToRow(e, attribute, localEditableSetting);
                }
            }
        }

        private void AddControlToRow(GridViewRowEventArgs e, AssetAttribute attribute, bool isEditable)
        {
            HtmlGenericControl label = new HtmlGenericControl();
            var attrName = attribute.GetConfiguration().NameLocalized;
            label.InnerHtml = Server.HtmlEncode(attrName);
            if (Editable)
            {
                // Mark field as requiered
                bool isRequired = attribute.Configuration.IsRequired &
                                   attribute.Configuration.DBTableFieldName != AttributeNames.UserId &
                                   attribute.Configuration.DBTableFieldName != AttributeNames.OwnerId;

                bool isValid = true;
                //if (IsPostBack && attribute.Validate().Any(v => !v.IsValid))
                //    isValid = false;
                label.InnerHtml = String.Format("{0}&nbsp;<span style=\"display:{1}\" class=\"validation {2}\">*</span>",
                                                          label.InnerHtml,
                                                          (isRequired || !isValid) ? "inline" : "none",
                                                          isValid ? "" : "error");
            }
            e.Row.Cells[0].Controls.Add(label);
            
            var control = AttributeFieldFactory.GetControl(attribute, isEditable, MySettingsPage) as Control;
            
            if (control == null)
                throw new Exception("Control not found for attribute: "+attribute.Configuration.Name);

            var assetAttributeCtl = control as IAssetAttributeControl;
            if (assetAttributeCtl != null)
            {
                assetAttributeCtl.AddAttribute("data-isAttrInputCtl", "");
                assetAttributeCtl.AddAttribute("data-attrUID", attribute.Configuration.UID.ToString());

                var jsCall = string.Format("return utilities_CalculateAssetAttributes({0}, {1})",
                                           attribute.ParentAsset.DynEntityConfigUid, attribute.ParentAsset.UID);
                assetAttributeCtl.AddAttribute("onblur", jsCall);

                // add marker class to control if attribute is calculated by formula
                if (attribute.Configuration.IsCalculated)
                {
                    assetAttributeCtl.AddAttribute("class", "autocalculated");
                    assetAttributeCtl.Editable = false;
                }
            }

            e.Row.Cells[1].Controls.Add(control);

            if (attribute.GetConfiguration().DataTypeEnum == Enumerators.DataType.Asset)
            {
                List<AttributeElement> attributes = new List<AttributeElement>();
                AttributeElement element = new AttributeElement();

                var oneType = new OneType(
                    AssetTypeRepository.GetByUid(attribute.GetConfiguration().AssetTypeUID), 
                    UnitOfWork,
                    AssetsService,
                    AssetTypeRepository);
                int index = oneType.Attributes.FindIndex(item => item.AttributeValue == attribute.Configuration.UID);

                element.AssetAttribute = attribute;
                element.FieldName = attribute.GetConfiguration().DBTableFieldName;
                element.AttributeId = index;
                int operatorIndex = oneType.Attributes[index].Operators.FindIndex(g => g.OperatorText == "==" || g.OperatorText == "LIKE");
                element.OperatorId = operatorIndex;
                element.Text = attribute.Value;
                attributes.Add(element);
                
                string searchGuid = Guid.NewGuid().ToString();
                Session[searchGuid] = attributes;

                int timeType = 1; //active by default
                string relatedSearchString = string.Format("/Search/ResultByType.aspx?Params={0}&TypeUID={1}&Time={2}", searchGuid, attribute.GetConfiguration().AssetTypeUID, timeType);

                Literal literal = new Literal();
                literal.Text = " ";

                HyperLink link = new HyperLink();
                link.Text = "Related items";
                link.NavigateUrl = relatedSearchString;
                link.ForeColor = Color.Silver;


                e.Row.Cells[1].Controls.Add(literal);
                e.Row.Cells[1].Controls.Add(link);
            }

        }

        public List<AssetAttribute> GetAttributes()
        {
            List<AssetAttribute> attributes = new List<AssetAttribute>();
            foreach (GridViewRow row in GridAttributes.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    IAssetAttributeControl attrControl = null;
                    if (row.Cells[1].Controls.Count != 0)
                        attrControl = row.Cells[1].Controls[0] as IAssetAttributeControl;

                    if (attrControl != null)
                    {
                        attributes.Add(attrControl.GetAttribute());
                    }
                }
            }
            return attributes;
        }
    }
}