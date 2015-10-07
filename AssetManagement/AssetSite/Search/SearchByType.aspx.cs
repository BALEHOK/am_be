using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;
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
    public partial class SearchByType : SearchPage
    {
        [Dependency]
        public IDataTypeService DataTypeService { get; set; }
        [Dependency]
        public IValidationService ValidationService { get; set; }

        protected OneType CurrentType
        {
            get
            {
                if (Session[_GUID + "CurrentType"] != null)
                {
                    return Session[_GUID + "CurrentType"] as OneType;
                }
                return null;
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
            bracketsPanel.Visible = false;
            lWarning.Visible = false;
            if (!IsPostBack)
            {
                ddlTypes.DataTextField = "Key";
                ddlTypes.DataValueField = "Value";
                var assetTypes =
                    (from type in AssetTypeRepository.GetAllPublished()
                        where AuthenticationService.IsReadingAllowed(type)
                        select new KeyValuePair<string, long>(type.Name, type.UID)).ToList();

                if (assetTypes.Count == 0)
                {
                    //user don't have access to any of asset types
                    return;
                }

                ddlTypes.DataSource = assetTypes;
                ddlTypes.DataBind();

                var guid = Request.QueryString["Params"];
                var typeUid = Request.QueryString["TypeUID"];
                if (!string.IsNullOrEmpty(guid) && !string.IsNullOrEmpty(typeUid))
                {
                    _GUID = guid;
                    this.ddlTypes.SelectedValue = typeUid;
                    if (CurrentType == null)
                    {
                        CurrentType = new OneType(AssetTypeRepository.GetByUid(long.Parse(ddlTypes.SelectedValue)),
                            UnitOfWork, AssetsService, AssetTypeRepository);
                    }
                    UpdateRepeater();
                    if (Request.QueryString["Time"] != null)
                    {
                        TimePeriodForSearch period =
                            (TimePeriodForSearch) int.Parse(Request.QueryString["Time"].ToString());
                        rbActive.SelectedIndex = period == TimePeriodForSearch.CurrentTime ? 0 : 1;
                    }
                }
                else
                {
                    if (CurrentType == null)
                    {
                        CurrentType = new OneType(AssetTypeRepository.GetByUid(long.Parse(ddlTypes.SelectedValue)),
                            UnitOfWork, AssetsService, AssetTypeRepository);
                    }
                }
            }
            UpdateRepeater();
        }

        protected void ButtonSearch_Click(object sender, EventArgs e)
        {
            SaveValueChangesRepeater();
            string errorMessage = string.Empty;

            // first, validate all values according theirs datatypes            
            bool valid = true;
            foreach (var element in CurrentAttributes)
            {
                var assetTypeAttribute = AssetTypeRepository.GetAttributeByUid(CurrentType.Attributes[element.AttributeId].AttributeValue);
                if (assetTypeAttribute != null)
                {
                    var result = ValidationService.ValidateDataType(assetTypeAttribute.DataType, element.Text);
                    if (!result.IsValid)
                    {
                        errorMessage += string.Join("<br />", result.ResultLines.Select(r => r.Message).ToArray()) + "<br />";
                        valid = false;
                    }
                }
            }

            if (valid)
            {
                if (CurrentAttributes.Count > 0)
                {
                    TimePeriodForSearch period = rbActive.SelectedValue == "1" ? TimePeriodForSearch.CurrentTime : TimePeriodForSearch.History;
                    var query = "?Params=" + _GUID + "&TypeUID=" + ddlTypes.SelectedValue + "&Time=" + (int)period;

                    Session[Constants.SearchParameters] = Request.Url.GetLeftPart(UriPartial.Path).Replace("ResultByType", "SearchByType") + query;

                    Response.Redirect("~/Search/ResultByType.aspx" + query);
                }
            }
            else
            {
                this.ErrorMessage.Text = errorMessage;
                this.ErrorMessage.Visible = true;
            }
        }

        protected void ddlTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentType = new OneType(AssetTypeRepository.GetByUid(long.Parse(ddlTypes.SelectedValue)), 
                UnitOfWork, AssetsService, AssetTypeRepository);
            CurrentAttributes = new List<AttributeElement>();
            UpdateRepeater();
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
            if (CurrentType != null)
            {
                SaveValueChangesRepeater();
                CurrentAttributes.Add(new AttributeElement
                {
                    DateType = CurrentType.Attributes.First().Configuration.DataTypeEnum
                });
                UpdateRepeater();
            }
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
            foreach (RepeaterItem row in Repeater1.Items)
            {
                DropDownList opers;
                opers = row.FindControl("ddlAttribs") as DropDownList;
                if (elements[i].AttributeId > 0)
                    opers.SelectedIndex = elements[i].AttributeId;
                if (opers.SelectedItem != null)
                    itemText = elements[i].StartBrackets + opers.SelectedItem.Text;
                opers = row.FindControl("ddlOperators") as DropDownList;
                if (elements[i].OperatorId > 0)
                    opers.SelectedIndex = elements[i].OperatorId;
                if (opers.SelectedItem != null)
                    itemText += " " + opers.SelectedItem.Text + " ";
                opers = row.FindControl("ddlValues") as DropDownList;
                if (elements[i].ItemId > 0)
                    opers.SelectedIndex = elements[i].ItemId;
                if (opers.SelectedItem != null)
                    itemText += opers.SelectedItem.Text;
                itemText += elements[i].EndBrackets;

                TextBox tb = row.FindControl("tbValue") as TextBox;
                DropDownList booleanList = row.FindControl("dlBoolean") as DropDownList;
                PlaceAndZipControl ctrl = row.FindControl("apcPlace") as PlaceAndZipControl;

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
                    booleanList.Visible = true;
                    if (!string.IsNullOrEmpty(elements[i].Text))
                    {
                        booleanList.SelectedValue = elements[i].Text;
                    }
                }

                if (CurrentType.Attributes[elements[i].AttributeId].Type == Enumerators.DataType.Place)
                {
                    tb.Visible = false;
                    booleanList.Visible = false;
                    if (!string.IsNullOrEmpty(elements[i].Text))
                    {
                        ctrl.SelectedText = elements[i].Text;
                    }
                }

                lbBrakets.Items.Add(new ListItem(itemText, i.ToString()));

                opers = row.FindControl("ddlSort") as DropDownList;
                if (elements[i].AscDesc > 0)
                    opers.SelectedIndex = elements[i].AscDesc;
                opers = row.FindControl("ddlLogic") as DropDownList;
                if (elements[i].AndOr > 0)
                    opers.SelectedIndex = elements[i].AndOr;
                i++;
            }
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
                    CurrentAttributes[i].ItemId = opers.SelectedIndex;

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

                TextBox tb = row.FindControl("tbValue") as TextBox;
                if (tb.Visible)
                    CurrentAttributes[i].Text = tb.Text.Trim();

                DropDownList booleanList = row.FindControl("dlBoolean") as DropDownList;
                if (booleanList.Visible)
                    CurrentAttributes[i].Text = booleanList.SelectedValue;

                PlaceAndZipControl ctrl = row.FindControl("apcPlace") as PlaceAndZipControl;
                if (ctrl.Visible)
                {
                    CurrentAttributes[i].Text = ctrl.SelectedText;
                    CurrentAttributes[i].Value = ctrl.SelectedText;
                }    

                opers = row.FindControl("ddlSort") as DropDownList;
                CurrentAttributes[i].AscDesc = opers.SelectedIndex;
                opers = row.FindControl("ddlLogic") as DropDownList;
                CurrentAttributes[i].AndOr = opers.SelectedIndex;
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
                    string text = element.StartBrackets + CurrentType.Attributes[element.AttributeId].AttributeText;
                    text += CurrentType.Attributes[element.AttributeId].Operators[element.OperatorId].OperatorText;
                    if (CurrentType.Attributes[element.AttributeId].Operators[element.OperatorId].Items.Count > 0)
                        text += CurrentType.Attributes[element.AttributeId].Operators[element.OperatorId].Items[element.ItemId].Key;
                    else
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

        protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                int i = e.Item.ItemIndex;
                if (i < CurrentAttributes.Count && (
                        CurrentType.Attributes[CurrentAttributes[i].AttributeId].Type == Enumerators.DataType.Asset ||
                        CurrentType.Attributes[CurrentAttributes[i].AttributeId].Type == Enumerators.DataType.Assets ||
                        CurrentType.Attributes[CurrentAttributes[i].AttributeId].Type == Enumerators.DataType.Place
                        )
                    )
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

                    if (CurrentType.Attributes[CurrentAttributes[i].AttributeId].Type == Enumerators.DataType.Place)
                    {
                        PlaceAndZipControl placeControl = e.Item.FindControl("apcPlace") as PlaceAndZipControl;
                        placeControl.AssetAttribute = CurrentAttributes[i].AssetAttribute;
                    }
                    else
                    {
                        AssetDropDownListEx adlAssets = e.Item.FindControl("adlAssets") as AssetDropDownListEx;
                        adlAssets.AssetTypeRepository = this.AssetTypeRepository;
                        adlAssets.AuthenticationService = this.AuthenticationService;
                        adlAssets.AssetAttribute = CurrentAttributes[i].AssetAttribute;
                        adlAssets.Configure(CurrentAttributes[i].AssetAttribute.GetConfiguration());
                    }
                }
                else
                {
                    CurrentAttributes[i].AssetAttribute = null;
                }
            }
        }
    }
}
