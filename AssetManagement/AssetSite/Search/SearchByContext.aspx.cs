using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.DynLists;
using AppFramework.Core.Classes.SearchEngine.ContextSearchElements;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
using AppFramework.Core.DAL;
using AppFramework.DataProxy;
using AssetSite.Controls;
using AssetSite.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Practices.Unity;

namespace AssetSite.Search
{
    public partial class SearchByContext : SearchPage
    {
        [Dependency]
        public IDynamicListsService DynamicListsService { get; set; }

        protected ContextForSearch CurrentType
        {
            get
            {
                if (Session[_GUID + "CurrentType"] != null)
                {
                    return Session[_GUID + "CurrentType"] as ContextForSearch;
                }
                else
                {
                    Session[_GUID + "CurrentType"] = new ContextForSearch(UnitOfWork);
                    return Session[_GUID + "CurrentType"] as ContextForSearch;
                }
            }
            set
            {
                Session[_GUID + "CurrentType"] = value;
            }
        }

        private List<AttributeElement> CurrentAttributes
        {
            get
            {
                if (Session[_GUID] == null)
                    Session[_GUID] = new List<AttributeElement>();
                return Session[_GUID] as List<AttributeElement>;
            }
            set
            {
                Session[_GUID] = value;
            }

        }

        private string _GUID
        {
            get
            {
                if (hfUid.Value == string.Empty)
                    hfUid.Value = Guid.NewGuid().ToString();
                return hfUid.Value;
            }
            set
            {
                hfUid.Value = value;
            }
        }

        private List<AssetType> AvailableAssetTypes
        {
            get
            {
                if (Session["AvailableAssetType"] == null)
                {
                    LoadAvailableAssetTypes();
                }

                return Session["AvailableAssetType"] as List<AssetType>;
            }
        }

        private void LoadAvailableAssetTypes()
        {           
            Session["AvailableAssetType"] = AssetType.GetAllAvailableForAssets().ToList();
        }

        public SearchByContext()
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptInclude("highlight", "/javascript/jquery.highlight-3.js");
            Page.ClientScript.RegisterClientScriptInclude("hint", "/javascript/jquery.hint.js"); 
            bracketsPanel.Visible = false;
            lWarning.Visible = false;
            if (!IsPostBack)
            {
                LoadAvailableAssetTypes();
                var guid = Request.QueryString["Params"];
                if (!string.IsNullOrEmpty(guid))
                {
                    _GUID = guid;
                    UpdateRepeater();
                    if (Request.QueryString["Time"] != null)
                    {
                        int period = (int)TimePeriodForSearch.CurrentTime;
                        int.TryParse(Request.QueryString["Time"], out period);
                        rbActive.SelectedIndex = period == (int)TimePeriodForSearch.CurrentTime ? 0 : 1;
                    }
                }
                else
                {
                    _GUID = Guid.NewGuid().ToString();
                }
            }

            if (Locale != "en")
                Page.ClientScript.RegisterClientScriptInclude("datepicker_localization",
                  string.Format("/javascript/jquery.ui.datepicker-{0}.js", Locale));
        }

        protected void ButtonSearch_Click(object sender, EventArgs e)
        {
            if (Page.IsValid && CurrentAttributes.Count > 0 )
            {
                SaveValueChangesRepeater();
                TimePeriodForSearch period = rbActive.SelectedValue == "1" ? TimePeriodForSearch.CurrentTime : TimePeriodForSearch.History;

                var query = "?Params=" + _GUID + "&Time=" + (int)period;

                Session[Constants.SearchParameters] = Request.Url.GetLeftPart(UriPartial.Path).Replace("ResultByContext", "SearchByContext") + query;

                Response.Redirect("~/Search/ResultByContext.aspx" + query);
            }
        }

        protected void ddlTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlTypes = sender as DropDownList;
            SaveValueChangesRepeater();
            int index = int.Parse(ddlTypes.CssClass);
            if (CurrentAttributes[index] != null)
            {
                var currentType = AvailableAssetTypes.FirstOrDefault(at => at.ID == long.Parse(ddlTypes.SelectedValue));
                CurrentAttributes[index].AssetAttribute = new AssetAttribute(
                    AssetTypeRepository.GetAttributeByRelatedAssetTypeAttributeId(currentType.ID),
                    new DynColumn(), 
                    null,
                    AttributeValueFormatter,
                    AssetTypeRepository,
                    AssetsService,
                    UnitOfWork,
                    DynamicListsService);
            }
            UpdateRepeater();
        }

        protected void ddlDynList_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlDynLists = sender as DropDownList;
            SaveValueChangesRepeater();
            int index = int.Parse(ddlDynLists.CssClass);
            if (CurrentAttributes[index] != null)
            {
                CurrentAttributes[index].DynListId = long.Parse(ddlDynLists.SelectedValue);
            }
            UpdateRepeater();
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlSender = sender as DropDownList;
            if (ddlSender == null)
                return;
            SaveValueChangesRepeater();
            int index = int.Parse(ddlSender.CssClass);
            int i = CurrentType.Attributes.FindIndex(g => g.Value.ToString() == ddlSender.SelectedValue);
            var currentAttribute = CurrentType.Attributes.FirstOrDefault(g => g.Value.ToString() == ddlSender.SelectedValue);
            CurrentAttributes[index].Text = string.Empty;
            if (currentAttribute != null)
            {
                CurrentAttributes[index].DateType = currentAttribute.DataTypeEnum;
            }
            CurrentAttributes[index].AttributeId = i;
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
            var contextAttribute = CurrentType.Attributes.FirstOrDefault();
            if (contextAttribute != null)
            {
                CurrentAttributes.Add(new AttributeElement()
                    {
                        DateType = contextAttribute.DataTypeEnum,
                    });
            }
            else
                CurrentAttributes.Add(new AttributeElement());
            Repeater1.DataBind();
            UpdateRepeater();
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
            lbBrakets.Items.Clear();
            foreach (RepeaterItem o in Repeater1.Items)
            {
                DropDownList opers;
                opers = o.FindControl("ddlAttribs") as DropDownList;
                if (elements[i].AttributeId > 0)
                    opers.SelectedIndex = elements[i].AttributeId;
                if (opers.SelectedItem != null)
                    itemText = elements[i].StartBrackets + opers.SelectedItem.Text;
                opers = o.FindControl("ddlOperators") as DropDownList;
                if (elements[i].OperatorId > 0)
                    opers.SelectedIndex = elements[i].OperatorId;
                if (opers.SelectedItem != null)
                    itemText += opers.SelectedItem.Text;
                itemText += elements[i].EndBrackets;

                TextBox tb = o.FindControl("tbValue") as TextBox;
                DropDownList booleanList = o.FindControl("dlBoolean") as DropDownList;

                if (CurrentType.Attributes[elements[i].AttributeId].DataTypeEnum == Enumerators.DataType.DateTime)
                {
                    tb.CssClass = "datepicker float-left";
                    ScriptManager.RegisterClientScriptBlock(Page, this.GetType(), "_Calendar_" + tb.ClientID,
                              "$(function () { " +
                                   string.Format("InitDatePicker('{0}')", tb.ClientID) +
                               "});", true);
                }
                if (CurrentType.Attributes[elements[i].AttributeId].DataTypeEnum == Enumerators.DataType.Bool)
                {
                    tb.Visible = false;
                    booleanList = o.FindControl("dlBoolean") as DropDownList;
                    booleanList.Visible = true;
                    if (!string.IsNullOrEmpty(elements[i].Text))
                    {
                        booleanList.SelectedValue = elements[i].Text;
                    }
                }

                tb.Text = elements[i].Text;
                itemText += elements[i].Text;

                lbBrakets.Items.Add(new ListItem(itemText, i.ToString()));

                opers = o.FindControl("ddlSort") as DropDownList;
                if (elements[i].AscDesc > 0)
                    opers.SelectedIndex = elements[i].AscDesc;
                opers = o.FindControl("ddlLogic") as DropDownList;
                if (elements[i].AndOr > 0)
                    opers.SelectedIndex = elements[i].AndOr;

                i++;
            }
        }

        private void SaveValueChangesRepeater()
        {
            List<AttributeElement> elements = CurrentAttributes;
            if (elements.Count == 0)
                return;
            int i = 0;
            foreach (RepeaterItem o in Repeater1.Items)
            {
                DropDownList opers;
                TextBox tb = o.FindControl("tbValue") as TextBox;
                DropDownList booleanList = o.FindControl("dlBoolean") as DropDownList;

                elements[i].Text = tb.Text.Trim();
                if (!tb.Visible && !booleanList.Visible)
                {
                    AssetDropDownListEx adlAssets = o.FindControl("adlAssets") as AssetDropDownListEx;
                    if (adlAssets.Visible)
                    {
                        adlAssets.AssetAttribute = elements[i].AssetAttribute;
                        elements[i].AssetAttribute = adlAssets.GetAttribute();
                    }
                    else
                    {
                        DropDownList ddlDynLists = o.FindControl("ddlDynLists") as DropDownList;
                        elements[i].DynListId = long.Parse(ddlDynLists.SelectedValue);
                        DropDownList ddlDynListItems = o.FindControl("ddlDynListItems") as DropDownList;
                        if (!string.IsNullOrEmpty(ddlDynListItems.SelectedValue))
                        {
                            elements[i].DynListItemId = long.Parse(ddlDynListItems.SelectedValue);
                            elements[i].Text = ddlDynListItems.SelectedItem.Text;
                        }
                    }
                }

                if (booleanList.Visible)
                    elements[i].Text = booleanList.SelectedValue;

                opers = o.FindControl("ddlSort") as DropDownList;
                elements[i].AscDesc = opers.SelectedIndex;
                opers = o.FindControl("ddlLogic") as DropDownList;
                elements[i].AndOr = opers.SelectedIndex;
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

        protected void buttonNInsertBrakets_Click(object sender, EventArgs e)
        {
            SaveValueChangesRepeater();
            UpdateRepeater();
            if (CurrentAttributes.Count > 0)
                bracketsPanel.Visible = true;
            hfRemove.Value = "0";
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            if (hfRemove.Value == "1")
            {
                RemoveClicked();
                return;
            }
            int start = -1;
            int end = -1;
            for (int i = 0; i < lbBrakets.Items.Count; i++)
            {
                if (lbBrakets.Items[i].Selected)
                {
                    if (start < 0)
                        start = i;
                    end = i;
                }
            }
            if (start >= 0)
            {
                CurrentAttributes[start].StartBrackets += "(";
                CurrentAttributes[end].EndBrackets += ")";
            }
            UpdateRepeater();
        }

        private void RemoveClicked()
        {
            int start = -1;
            int end = -1;
            foreach (ListItem item in lbBrakets.Items)
            {
                if (item.Selected)
                {
                    if (start < 0)
                        start = int.Parse(item.Value);
                    end = int.Parse(item.Value);
                }
            }
            if (start >= 0)
            {
                if (string.IsNullOrEmpty(CurrentAttributes[start].StartBrackets) ||
                    string.IsNullOrEmpty(CurrentAttributes[end].EndBrackets))
                {
                    bracketsPanel.Visible = true;
                    lWarning.Visible = true;
                    return;
                }
                CurrentAttributes[start].StartBrackets = CurrentAttributes[start].StartBrackets.Substring(1);
                CurrentAttributes[end].EndBrackets = CurrentAttributes[end].EndBrackets.Substring(1);
            }
            UpdateRepeater();
        }

        protected void buttonRemoveBrakets_Click(object sender, EventArgs e)
        {
            SaveValueChangesRepeater();
            UpdateRepeater();
            bracketsPanel.Visible = true;
            GenerateToRemove();
            hfRemove.Value = "1";
        }

        private void GenerateToRemove()
        {
            int counter = 0;
            lbBrakets.Items.Clear();
            foreach (AttributeElement element in CurrentAttributes)
            {
                if (!string.IsNullOrEmpty(element.StartBrackets) || !string.IsNullOrEmpty(element.EndBrackets))
                {
                    string text = element.StartBrackets + CurrentType.Attributes[element.AttributeId].Text;
                    text += CurrentType.Attributes[element.AttributeId].Operators[element.OperatorId].OperatorText;
                    text += element.Text;
                    text += element.EndBrackets;
                    lbBrakets.Items.Add(new ListItem(text, counter.ToString()));
                }
                counter++;
            }
            if (lbBrakets.Items.Count == 0)
                bracketsPanel.Visible = false;
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
                AttributeElement attribute = e.Item.DataItem as AttributeElement;
                if (attribute.DateType == Enumerators.DataType.Bool)
                {
                    RequiredFieldValidator valueValidator = e.Item.FindControl("valueValidator") as RequiredFieldValidator;
                    valueValidator.Enabled = false;
                }
                else if (attribute.DateType == Enumerators.DataType.DynList)
                {
                    RequiredFieldValidator valueValidator = e.Item.FindControl("valueValidator") as RequiredFieldValidator;
                    valueValidator.Enabled = false;
                    (e.Item.FindControl("tbValue") as TextBox).Visible = false;
                    DropDownList ddlDynLists = e.Item.FindControl("ddlDynLists") as DropDownList;
                    ddlDynLists.Visible = true;
                    ddlDynLists.DataSource = DynamicListsService.GetAll();
                    ddlDynLists.DataBind();
                    ddlDynLists.SelectedValue = attribute.DynListId.ToString();
                    DropDownList ddlDynListItems = e.Item.FindControl("ddlDynListItems") as DropDownList;
                    ddlDynListItems.Visible = true;
                    ddlDynListItems.DataSource = DynamicListsService.GetByUid(long.Parse(ddlDynLists.SelectedValue)).Items;
                    ddlDynListItems.DataBind();
                    ddlDynListItems.SelectedValue = attribute.DynListItemId.ToString();
                }
                else if (attribute.DateType == Enumerators.DataType.Asset)
                {
                    DropDownList ddlTypes = e.Item.FindControl("ddlTypes") as DropDownList;
                    if (AvailableAssetTypes.Count > 0)
                    {
                        RequiredFieldValidator valueValidator = e.Item.FindControl("valueValidator") as RequiredFieldValidator;
                        valueValidator.Enabled = false;
                        (e.Item.FindControl("tbValue") as TextBox).Visible = false;
                        ddlTypes.Visible = true;
                        ddlTypes.DataSource = AvailableAssetTypes;
                        ddlTypes.DataBind();
                        if (attribute.AssetAttribute == null)
                        {
                            var currentType = AvailableAssetTypes.FirstOrDefault();
                            attribute.AssetAttribute = new AssetAttribute(
                                AssetTypeRepository.GetAttributeByRelatedAssetTypeAttributeId(currentType.ID),
                                new DynColumn(), 
                                null,
                                AttributeValueFormatter,
                                AssetTypeRepository,
                                AssetsService,
                                UnitOfWork,
                                DynamicListsService);
                        }
                        ddlTypes.SelectedValue = attribute.AssetAttribute.Configuration.RelatedAssetTypeID.ToString();
                        AssetDropDownListEx adlAssets = e.Item.FindControl("adlAssets") as AssetDropDownListEx;
                        adlAssets.Visible = true;
                        adlAssets.AssetAttribute = attribute.AssetAttribute;
                        adlAssets.Configure(attribute.AssetAttribute.GetConfiguration());
                    }
                }
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
    }
}
