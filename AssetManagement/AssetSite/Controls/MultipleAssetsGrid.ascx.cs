using System.Web.UI;
using AppFramework.Core.Classes;
using AppFramework.Core.PL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.Practices.Unity;

namespace AssetSite.Controls
{
    public partial class MultipleAssetsGrid : UserControl, IAssetAttributeControl
    {
        [Dependency]
        public IAssetTypeRepository AssetTypeRepository { get; set; }
        [Dependency]
        public IAssetsService AssetsService { get; set; }
        public AssetAttribute AssetAttribute { get; set; }
        public bool Editable { get; set; }

        public AssetAttribute GetAttribute()
        {
            return AssetAttribute;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var pnl = new System.Web.UI.WebControls.Panel();
            pnl.CssClass = "multiple-assets-panel";
            var table = new Table();
            table.Attributes.Add("width", "100%");
            table.CellSpacing = 0;
            table.CellPadding = 2;
            var at = AssetTypeRepository.GetById(AssetAttribute.GetConfiguration().RelatedAssetTypeID.Value);
            bool isTableHead = false;
            TableRow headerRow = new TableRow();
            headerRow.CssClass = "gridlabels multiple-assets-table-header";
            var visibleCols = new List<string>();
            if (AssetAttribute.CustomMultipleAssetsFields != null)
            {
                visibleCols = AssetAttribute.CustomMultipleAssetsFields;
            }
            else
            {
                visibleCols = at.Attributes.Where(attr => attr.IsShownInGrid).Select(attr => attr.DBTableFieldName).ToList();
            }

            int index = 0;
            foreach (var entry in AssetAttribute.MultipleAssets)
            {
                index++;
                var dataRow = new TableRow();
                var asset = AssetsService.GetAssetById(entry.Key, at);
                if (asset == null)
                {
                    dataRow.Cells.Add(new TableCell()
                    {
                        ColumnSpan = 20,
                        Text = "error loading asset"
                    });
                    continue;
                }
                foreach (var column in visibleCols)
                {
                    if (!isTableHead)
                    {
                        headerRow.Cells.Add(new TableCell()
                        {
                            Text = asset[column].GetConfiguration().NameLocalized
                        });
                    }
                    dataRow.Cells.Add(new TableCell()
                    {
                        Text = asset[column].Value
                    });
                }
                if (!isTableHead)
                {
                    table.Rows.Add(headerRow);
                    isTableHead = true;
                }
                if (index % 2 == 0)
                    dataRow.CssClass = "multiple-assets-table-alternating";
                table.Rows.Add(dataRow);
            }
            pnl.Controls.Add(table);
            gridPlaceHolder.Controls.Add(pnl);
            gridPlaceHolder.Visible = index > 0;
        }

        public void AddAttribute(string name, string value)
        {            
        }
    }
}