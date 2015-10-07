using System.Web.UI.HtmlControls;
using AppFramework.Core.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetSite.Asset
{
    public enum AttributeMovement
    {
        Added = 0,
        Deleted = 1,
    }

    public class AttributeChange
    {
        public AttributeMovement? Movement { get; set; }
        public string Name { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }

    public class AssetChangeset
    {
        public AssetChangeset(AppFramework.Core.Classes.Asset asset)
        {
            AssetRevisison = asset;
            AttributesChangeset = new List<AttributeChange>();
        }

        public AppFramework.Core.Classes.Asset AssetRevisison { get; private set; }
        public List<AttributeChange> AttributesChangeset { get; set; }
    }

    public partial class History : AssetController
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            long assetTypeId = 0;
            if (Request.QueryString["assetTypeID"] != null)
                long.TryParse(Request.QueryString["assetTypeID"], out assetTypeId);

            long assetId = 0;
            if (Request.QueryString["assetID"] != null)
                long.TryParse(Request.QueryString["assetID"], out assetId);

            gvAssetHistory.Visible = false;
            if (assetTypeId > 0 && assetId > 0)
            {
                AssetType = AssetTypeRepository.GetById(assetTypeId);

                ShowHistoryData(assetTypeId, assetId);

                if (!IsPostBack)
                {
                    ShowLeftPanel();
                }
            }
            else
            {
                Response.Redirect("~/");
            }

            return;
            if (assetTypeId > 0 && assetId > 0)
            {
                AssetType = AssetTypeRepository.GetById(assetTypeId);
                var asset = AssetFactory.GetHistoryAssets(assetTypeId, assetId, 0, 1).FirstOrDefault();
                if (asset != null)
                {
                    gvAssetHistory.Visible = true;
                    gvAssetHistory.CreateColumns(asset);
                    lblAssetName.Text = asset.Name;
                }
                else
                {
                    gvAssetHistory.Visible = false;
                }

                assetsHistoryDataSource.SelectParameters["assetTypeId"].DefaultValue = assetTypeId.ToString();
                assetsHistoryDataSource.SelectParameters["assetId"].DefaultValue = assetId.ToString();

            }
            else
            {
                Response.Redirect("~/");
            }
        }

        private void ShowLeftPanel()
        {
            var taxonomies = new List<AppFramework.Entities.TaxonomyItem>();

            var taxonomyItems = UnitOfWork.TaxonomyItemRepository.GetTaxonomyItemsByAssetTypeId(AssetType.ID);
            foreach (var item in taxonomyItems)
            {
                if (item.Taxonomy.IsActive && item.Taxonomy.ActiveVersion)
                    taxonomies.Add(item);
            }

            foreach (var item in taxonomies)
            {
                bool isCategory = false;
                string path = this.GetParentName(item);

                if (path.Contains("C>"))
                {
                    isCategory = true;
                    path = path.Replace("C>", string.Empty);
                }

                IEnumerable<string> tempPath = path.Split(new char[1] { '>' }, StringSplitOptions.RemoveEmptyEntries).Reverse();
                string nPath = string.Empty;
                foreach (string pathElem in tempPath)
                {
                    nPath += pathElem + " > ";
                }

                nPath += "<a href='../AssetView.aspx?AstType=" + AssetType.ID + "'>" + AssetType.Name + "</a>";

                if (isCategory)
                    litCategoryPath.Text += nPath + "<br/>";
                else
                    litTaxonomies.Text += nPath + "<br/>";
            }
        }

        private string GetParentName(AppFramework.Entities.TaxonomyItem item)
        {
            if (item.ParentItem == null)
            {
                if (item.Taxonomy.IsCategory)
                    return "C>" + item.Name;

                return item.Name;
            }
            if (item.Taxonomy.IsCategory)
                return "C>" + item.Name + ">" + this.GetParentName(item.ParentItem);
            return item.Name + ">" + this.GetParentName(item.ParentItem);
        }

        private void ShowHistoryData(long assetTypeId, long assetId)
        {
            var data = GetHistoryData(assetTypeId, assetId);

            var header = new HtmlTableRow();
            header.Cells.Add(new HtmlTableCell("th") { InnerText = "Revision" });
            header.Cells.Add(new HtmlTableCell("th") { InnerText = "Updated" });
            header.Cells.Add(new HtmlTableCell("th") { InnerText = "Field" });
            header.Cells.Add(new HtmlTableCell("th") { InnerText = "Old Value" });
            header.Cells.Add(new HtmlTableCell("th") { InnerText = "Revision Value" });

            historyTable.Rows.Add(header);

            data.ForEach(dataRow =>
            {
                var row = new HtmlTableRow();
                row.Cells.Add(new HtmlTableCell { InnerText = dataRow.AssetRevisison["Revision"].Value });
                row.Cells.Add(new HtmlTableCell { InnerText = dataRow.AssetRevisison["UpdateDate"].Value.ToString(ApplicationSettings.PersistenceCultureInfo) });
                row.Cells.Add(new HtmlTableCell { InnerText = "" });
                row.Cells.Add(new HtmlTableCell { InnerText = "" });
                row.Cells.Add(new HtmlTableCell { InnerText = "" });
                row.BgColor = "#BFE3AC";

                historyTable.Rows.Add(row);

                dataRow.AttributesChangeset.ForEach(a =>
                {
                    row = new HtmlTableRow();

                    var oldValue = a.OldValue;
                    var newValue = a.NewValue;
                    if (a.Movement != null)
                    {
                        newValue = a.Movement == AttributeMovement.Added ? "Field was added" : "Field was deleted";
                    }

                    var nameCell = new HtmlTableCell { InnerText = a.Name };
                    nameCell.Style.Add("font-weight", "bold");

                    row.Cells.Add(new HtmlTableCell { InnerText = "" });
                    row.Cells.Add(new HtmlTableCell { InnerText = "" });
                    row.Cells.Add(nameCell);
                    row.Cells.Add(new HtmlTableCell { InnerText = oldValue });
                    row.Cells.Add(new HtmlTableCell { InnerText = newValue });
                    historyTable.Rows.Add(row);
                });
            });
        }

        private List<AssetChangeset> GetHistoryData(long typeId, long assetId)
        {
            var exclusions = new[] { "DynEntityUid", "Revision", "UpdateDate", "DynEntityConfigUid", "ActiveVersion" };

            var assets = AssetsService.GetHistoryAssets(typeId, assetId).ToList();

            var historyData = new List<AssetChangeset>();

            for (int i = 0; i < assets.Count; i++)
            {
                var asset = assets[i];
                var prevAsset = i + 1 < assets.Count ? assets[i + 1] : null;

                var prevType = prevAsset != null ? prevAsset.GetConfiguration() : null;
                var curType = asset.GetConfiguration();

                var currentChangeset = new AssetChangeset(asset);

                if (prevType != null)
                {
                    var prevFields = prevType.Attributes.ToDictionary(a => a.Name, a => a.DBTableFieldName);
                    var curFields = curType.Attributes.ToDictionary(a => a.Name, a => a.DBTableFieldName);

                    var deletedFields = prevFields.Except(curFields).ToList();
                    deletedFields.ForEach(f => currentChangeset.AttributesChangeset.Add(new AttributeChange
                    {
                        Movement = AttributeMovement.Deleted,
                        Name = f.Key,
                        OldValue = prevAsset[f.Value].Value,
                    }));
                    var addedFields = curFields.Except(prevFields).ToList();
                    addedFields.ForEach(f => currentChangeset.AttributesChangeset.Add(new AttributeChange
                    {
                        Movement = AttributeMovement.Added,
                        Name = f.Key,
                    }));
                }

                curType.Attributes.ToList().ForEach(attribute =>
                {
                    var fieldName = attribute.DBTableFieldName;

                    if (exclusions.Contains(fieldName))
                        return;

                    var prevValue = "";
                    if (prevType != null && prevAsset != null)
                    {
                        var prevTypeHasField = prevType.Attributes.Any(f => f.DBTableFieldName == fieldName);
                        if (prevTypeHasField)
                            prevValue = prevAsset[fieldName].Value;
                    }

                    var curValue = "";
                    var curTypeHasField = curType.Attributes.Any(f => f.DBTableFieldName == fieldName);
                    if (curTypeHasField)
                    {
                        curValue = asset[fieldName].Value;
                    }

                    if (curValue != prevValue)
                    {
                        var name = curType.Attributes.Single(a => a.DBTableFieldName == fieldName).Name;
                        var isAdded = currentChangeset.AttributesChangeset.Any(c => c.Name == name);
                        if (!isAdded)
                        {
                            currentChangeset.AttributesChangeset.Add(new AttributeChange
                            {
                                Name = curType.Attributes.Single(a => a.DBTableFieldName == fieldName).Name,
                                OldValue = prevValue,
                                NewValue = curValue
                            });
                        }
                    }
                });

                historyData.Add(currentChangeset);
            }

            return historyData;
        }
    }
}
