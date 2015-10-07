using AppFramework.Core.Classes;
using AppFramework.Core.ConstantsEnumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Microsoft.Practices.Unity;

namespace AssetSite.Controls
{
    public partial class MultipleAssetsList : UserControl
    {
        [Dependency]
        public IAssetTypeRepository AssetTypeRepository { get; set; }

        public string AssetTypeName { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptInclude("highlight", "/javascript/jquery.highlight-3.js");
            Page.ClientScript.RegisterClientScriptInclude("hint", "/javascript/jquery.hint.js");
            AssetType aType = AssetTypeRepository.FindByName(this.AssetTypeName);
            this.DialogContainer.Attributes.Add("title", aType.Name);

            string onselect = "return OnMultipleSelect('" + lstSelectedAssets.ClientID + "','" + lstAssets.ClientID +
                              "','" + DialogContainer.ClientID + "','" + hdfAddedItems.ClientID + "');";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "_DlgInitalize_" + this.ClientID,
                                                        "$(function () {$('#" + this.DialogContainer.ClientID +
                                                        "').dialog({ autoOpen: false, width: 420, height: 570, buttons : { '" +
                                                        Resources.Global.CancelText
                                                        +
                                                        "' : function() { $(this).dialog('close'); }, 'Ok': function() { $(this).dialog('close'); " +
                                                        onselect
                                                        + " } } });});", true);

            var parametersString = aType.ID + "," + 
                aType[AttributeNames.Name].ID +
                ",'" + tbSearchPattern.ClientID + "','" + lstAssets.ClientID + "'";

            lbtnSearch.Attributes.Add("onclick",
                                      "OnOpened(" + parametersString + ");" +
                                      "return ShowDialog('" + this.DialogContainer.ClientID + "');");

            lbtnDoSearch.Attributes.Add("onclick", "return OnTextChangedReturnsIdName(" + parametersString + ");");
        }

        protected override object SaveControlState()
        {
            return (object)this.AssetTypeName;
        }

        protected override void LoadControlState(object savedState)
        {
            if (savedState != null)
            {
                this.AssetTypeName = savedState.ToString();
            }
        }

        public List<long> GetAssetIds()
        {
            List<long> result = new List<long>();

            string[] removedIds = hdfDeletedItems.Value.TrimEnd(new char[1] { ',' }).Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Distinct().ToArray();
            string[] addedIds = hdfAddedItems.Value.TrimEnd(new char[1] { ',' }).Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Distinct().ToArray();

            foreach (string addedId in addedIds)
            {
                if (!removedIds.Contains(addedId))
                {
                    result.Add(long.Parse(addedId));
                }
            }

            return result;
        }
    }
}