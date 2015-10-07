using System.Web.UI;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AssetSite.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Practices.Unity;

namespace AssetSite.Controls
{
    public partial class AssetTemplates : UserControl
    {
        [Dependency]
        public IAssetTypeRepository AssetTypeRepository { get; set; }
        [Dependency]
        public IAssetsService AssetsService { get; set; }
        [Dependency]
        public IAuthenticationService AuthenticationService { get; set; }

        public AppFramework.Core.Classes.Asset Asset { get; set; }
        public AppFramework.Core.Classes.AssetType AssetType { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            DataBind();
        }

        public void BindData(List<AssetTemplate> templates)
        {
            repTemplates.DataSource = templates;
            repTemplates.DataBind();
            DataBind();
        }

        public string GetDivId(object ID)
        {
            return "templateContainer" + ID;
        }

        /// <summary>
        /// Returns html formated Asset fields
        /// </summary>
        /// <param name="templateAsset">of type Assett</param>
        /// <returns>html formated string</returns>
        public string GetAssetHtml(object templateAsset)
        {
            var asset = templateAsset as AppFramework.Core.Classes.Asset;

            string result = string.Empty;

            var exporter = new Exporter(AssetTypeRepository, AssetsService);
            using (var rdr = new StreamReader(exporter.ExportToTxt(asset)))
            {
                result += rdr.ReadToEnd().Replace("\n", "<br/>");
            }

            return result;
        }

        public string GetRestoreUrl(object id)
        {
            return "Step2.aspx?atid=" + this.AssetType.ID + "&tid=" + id.ToString();
        }
        public bool IsEditable()
        {
            return AuthenticationService.GetPermission(Asset).CanWrite();
        }
        public string GetClickScript(object ID)
        {
            return "return RemoveTemplate(" + this.AssetType.UID + "," + ID.ToString() + ");";
        }
        public string GetTableId(object ID)
        {
            return "templateItemContainer" + ID.ToString();
        }
    }
}