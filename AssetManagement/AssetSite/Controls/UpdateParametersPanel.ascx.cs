using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Batch;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.DataProxy;
using AssetSite.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.Unity;

namespace AssetSite.Controls
{
    public partial class UpdateParametersPanel : UserControl
    {
        [Dependency]
        public IAuthenticationService AuthenticationService { get; set; }
        [Dependency]
        public IAssetTypeRepository AssetTypeRepository { get; set; }
        [Dependency]
        public IAssetsService AssetsService { get; set; }
        [Dependency]
        public IUnitOfWork UnitOfWork { get; set; }
        [Dependency]
        public IAttributeValueFormatter AttributeValueFormatter { get; set; }
        [Dependency]
        public IUserService UserService { get; set; }
        [Dependency]
        public IBatchJobManager BatchJobManager { get; set; }
        [Dependency]
        public IDynamicListsService DynamicListsService { get; set; }
        [Dependency]
        public IBatchJobFactory BatchJobFactory { get; set; }

        protected OneType CurrentType { get; set; }

        public List<AttributeElement> SearchAttributes
        {
            get
            {
                SaveValueChangesRepeater();
                return Session[_GUID] as List<AttributeElement>;
            }
        }

        protected List<AttributeElement> CurrentAttributes
        {
            get
            {
                if (Session[_GUID] == null)
                {
                    Session[_GUID] = new List<AttributeElement>();
                }
                return Session[_GUID] as List<AttributeElement>;

            }
            set
            {
                Session[_GUID] = value;
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

        protected string Locale
        {
            get
            {
                return CookieWrapper.Language.Split(new char[] { '-' })[0].ToLower();
            }
        }

        private string _GUID
        {
            get
            {
                if (hfUid.Value == string.Empty)
                {
                    hfUid.Value = Guid.NewGuid().ToString();
                }
                return hfUid.Value;
            }
            set
            {
                hfUid.Value = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptInclude("highlight", "/javascript/jquery.highlight-3.js");
            Page.ClientScript.RegisterClientScriptInclude("hint", "/javascript/jquery.hint.js");

            CurrentType = new OneType(
                AssetTypeRepository.GetByUid(long.Parse(Request.QueryString["TypeUID"])),
                UnitOfWork, 
                AssetsService,
                AssetTypeRepository,
                true);
        }

        private void UpdateRepeater()
        {
            if (CurrentAttributes.Count == 0)
            {
                Repeater1.DataSource = null;
                Repeater1.DataBind();
                return;
            }

            Repeater1.DataSource = CurrentAttributes;
            Repeater1.DataBind();
            List<AttributeElement> elements = CurrentAttributes;
            int i = 0;
            string itemText = "";
            foreach (RepeaterItem row in Repeater1.Items)
            {
                DropDownList opers;
                opers = row.FindControl("ddlAttribs") as DropDownList;
                if (elements[i].AttributeId > 0)
                    opers.SelectedIndex = elements[i].AttributeId;
                if (opers.SelectedItem != null)
                    itemText = elements[i].StartBrackets + opers.SelectedItem.Text;
                opers = row.FindControl("ddlValues") as DropDownList;
                if (elements[i].ItemId > 0)
                    opers.SelectedIndex = elements[i].ItemId;
                if (opers.SelectedItem != null)
                    itemText += opers.SelectedItem.Text;
                itemText += elements[i].EndBrackets;

                TextBox tb = row.FindControl("tbValue") as TextBox;
                DropDownList booleanList = row.FindControl("dlBoolean") as DropDownList;

                tb.Text = elements[i].Text;
                itemText += elements[i].Text;
                if (CurrentType.Attributes[elements[i].AttributeId].Type == Enumerators.DataType.DateTime ||
                    CurrentType.Attributes[elements[i].AttributeId].Type == Enumerators.DataType.CurrentDate)
                {
                    tb.CssClass = "datepicker float-left";
                    ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "_Calendar_" + tb.ClientID,
                              "$(function () { " +
                                   string.Format("InitDatePicker('{0}')", tb.ClientID) +
                               "});", true);
                }
                if (CurrentType.Attributes[elements[i].AttributeId].Type == Enumerators.DataType.Bool)
                {
                    tb.Visible = false;
                    booleanList = row.FindControl("dlBoolean") as DropDownList;
                    booleanList.Visible = true;
                    if (!string.IsNullOrEmpty(elements[i].Text))
                    {
                        booleanList.SelectedValue = elements[i].Text;
                    }
                }

                opers = row.FindControl("ddlSort") as DropDownList;
                if (elements[i].AscDesc > 0)
                    opers.SelectedIndex = elements[i].AscDesc;
                opers = row.FindControl("ddlLogic") as DropDownList;
                if (elements[i].AndOr > 0)
                    opers.SelectedIndex = elements[i].AndOr;
                i++;
            }
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlSender = sender as DropDownList;
            if (ddlSender == null)
                return;

            SaveValueChangesRepeater();

            int index = int.Parse(ddlSender.CssClass);
            int i = CurrentType.Attributes.FindIndex(g => g.AttributeValue.ToString() == ddlSender.SelectedValue);
            CurrentAttributes[index].AttributeId = i;
            CurrentAttributes[index].OperatorId = 0;
            CurrentAttributes[index].DateType = CurrentType.Attributes[i].Configuration.DataTypeEnum;
            if (CurrentAttributes[index].DateType == Enumerators.DataType.Asset ||
                CurrentAttributes[index].DateType == Enumerators.DataType.Assets)
            {
                var nasset = new AssetAttribute(
                    CurrentType.AssetAttributes[CurrentAttributes[index].AttributeId].Configuration, 
                    null,
                    CurrentType.AssetAttributes[CurrentAttributes[index].AttributeId].ParentAsset,
                    AttributeValueFormatter,
                    AssetTypeRepository,
                    AssetsService,
                    UnitOfWork,
                    DynamicListsService);
                nasset.InitData();
                CurrentAttributes[index].AssetAttribute = nasset;
            }
            UpdateRepeater();
        }

        protected void ddlOperators_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlSender = sender as DropDownList;
            if (ddlSender == null)
                return;
            SaveValueChangesRepeater();
            int index = int.Parse(ddlSender.CssClass);

            int i = CurrentType.Attributes[CurrentAttributes[index].AttributeId].Operators.FindIndex(g => g.OperatorValue.ToString() == ddlSender.SelectedValue);
            CurrentAttributes[index].OperatorId = i;

            UpdateRepeater();
        }

        protected void buttonNewRow_Click(object sender, EventArgs e)
        {
            SaveValueChangesRepeater();
            var newAttribute = new AttributeElement();
            newAttribute.DateType = CurrentType.Attributes.FirstOrDefault().Configuration.DataTypeEnum;
            CurrentAttributes.Add(new AttributeElement());
            UpdateRepeater();
        }

        private void SaveValueChangesRepeater()
        {
            if (CurrentAttributes.Count == 0)
                return;

            int i = 0;
            foreach (RepeaterItem row in Repeater1.Items)
            {
                DropDownList opers = row.FindControl("ddlValues") as DropDownList;
                if (opers.SelectedIndex >= 0)
                {
                    CurrentAttributes[i].ItemId = opers.SelectedIndex;
                    CurrentAttributes[i].Value = opers.SelectedValue;
                    CurrentAttributes[i].Text = opers.SelectedItem.Text;
                }

                AssetDropDownListEx adlAssets = row.FindControl("adlAssets") as AssetDropDownListEx;
                
                if (adlAssets.Visible)
                {
                    if (CurrentAttributes[i].AssetAttribute == null)
                    {
                        var nasset = new AssetAttribute(
                            CurrentType.AssetAttributes[CurrentAttributes[i].AttributeId].Configuration, 
                            null,
                            CurrentType.AssetAttributes[CurrentAttributes[i].AttributeId].ParentAsset,
                            AttributeValueFormatter,
                            AssetTypeRepository,
                            AssetsService,
                            UnitOfWork,
                            DynamicListsService);
                        nasset.InitData();
                        CurrentAttributes[i].AssetAttribute = nasset;
                    }
                    adlAssets.AssetAttribute = CurrentAttributes[i].AssetAttribute;
                    CurrentAttributes[i].AssetAttribute = adlAssets.GetAttribute();
                }

                if (CurrentAttributes[i].AssetAttribute == null)
                {
                    var nasset = new AssetAttribute(
                        CurrentType.AssetAttributes[CurrentAttributes[i].AttributeId].Configuration, 
                        null,
                        CurrentType.AssetAttributes[CurrentAttributes[i].AttributeId].ParentAsset,
                        AttributeValueFormatter,
                        AssetTypeRepository,
                        AssetsService,
                        UnitOfWork,
                        DynamicListsService);
                    nasset.InitData();
                    CurrentAttributes[i].AssetAttribute = nasset;
                }

                TextBox tb = row.FindControl("tbValue") as TextBox;
                if (tb.Visible)
                    CurrentAttributes[i].Text = tb.Text.Trim();
                DropDownList booleanList = row.FindControl("dlBoolean") as DropDownList;
                if (booleanList.Visible)
                    CurrentAttributes[i].Text = booleanList.SelectedValue;

                i++;
            }
        }

        protected void ImageButtonDel_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton ddlSender = sender as ImageButton;
            if (ddlSender == null)
                return;
            SaveValueChangesRepeater();
            int index = int.Parse(ddlSender.CssClass);
            CurrentAttributes.RemoveAt(index);
            UpdateRepeater();
        }

        protected void ImageButtonDown_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton ddlSender = sender as ImageButton;
            if (ddlSender == null)
                return;
            SaveValueChangesRepeater();
            int index = int.Parse(ddlSender.CssClass);
            if (index >= CurrentAttributes.Count - 1)
                return;
            CurrentAttributes.Reverse(index, 2);
            UpdateRepeater();
        }

        protected void ImageButtonUp_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton ddlSender = sender as ImageButton;
            if (ddlSender == null)
                return;
            SaveValueChangesRepeater();
            int index = int.Parse(ddlSender.CssClass);
            if (index <= 0)
                return;
            CurrentAttributes.Reverse(index - 1, 2);
            UpdateRepeater();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            CurrentAttributes = new List<AttributeElement>();
            UpdateRepeater();
        }

        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                int i = e.Item.ItemIndex;
                if (i < CurrentAttributes.Count && (CurrentType.Attributes[CurrentAttributes[i].AttributeId].Type == Enumerators.DataType.Asset ||
                        CurrentType.Attributes[CurrentAttributes[i].AttributeId].Type == Enumerators.DataType.Assets))
                {
                    if (CurrentAttributes[i].AssetAttribute == null)
                    {
                        var nasset = new AssetAttribute(
                            CurrentType.AssetAttributes[CurrentAttributes[i].AttributeId].Configuration, 
                            null,
                            CurrentType.AssetAttributes[CurrentAttributes[i].AttributeId].ParentAsset,
                            AttributeValueFormatter,
                            AssetTypeRepository,
                            AssetsService,
                            UnitOfWork,
                            DynamicListsService);
                        nasset.InitData();
                        CurrentAttributes[i].AssetAttribute = nasset;
                    }

                    var adlAssets = e.Item.FindControl("adlAssets") as AssetDropDownListEx;
                    adlAssets.AssetAttribute = CurrentAttributes[i].AssetAttribute;
                    adlAssets.Configure(CurrentAttributes[i].AssetAttribute.GetConfiguration());
                }
                else
                {
                    CurrentAttributes[i].AssetAttribute = null;
                }
            }
        }

        protected void updateButton_Click(object sender, EventArgs e)
        {
            SaveValueChangesRepeater();

            //hot fix for related asset
            foreach (var attribute in SearchAttributes)
            {
                if (attribute.AssetAttribute != null && (!String.IsNullOrEmpty(attribute.AssetAttribute.Value) || !String.IsNullOrEmpty(attribute.AssetAttribute.ValueAsId.ToString())))
                {
                    attribute.Text = attribute.AssetAttribute.Value;
                    attribute.Value = attribute.AssetAttribute.ValueAsId.ToString();
                }
            }

            if (CurrentAttributes.Count != 0)
            {
                if (Request.QueryString["Params"] != null && Request.QueryString["SearchId"] != null &&
                    Request.QueryString["TypeUID"] != null)
                {
                    var userId = AuthenticationService.CurrentUserId;
                    var updateJob = BatchJobFactory.CreateUpdateAssetsJob(
                        Request.QueryString["SearchId"],
                        Request.QueryString["TypeUID"],
                        AuthenticationService.CurrentUserId,
                        SearchAttributes);

                    BatchJobManager.SaveJob(updateJob);
                    Response.Redirect(updateJob.NavigateUrl);
                }
            }
        }
    }
}