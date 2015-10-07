using AppFramework.Core.Classes;
using AppFramework.Core.Classes.IE;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace AssetSite.Asset
{
    public class AssetController : BasePage
    {
        public AssetType AssetType { get; set; }
        public AppFramework.Core.Classes.Asset Asset { get; set; }
        protected string RestorePath = string.Empty;
        protected string RestoreFileName = string.Empty;

        protected int PanelNumber = 0;

        protected void CheckRestoreState(Controls.RestoreAssetMessage restorePanel)
        {
            var restore = !string.IsNullOrEmpty(Request["RestoreState"]) && Request["RestoreState"] == "1";
            var delete = !string.IsNullOrEmpty(Request["DeleteState"]) && Request["DeleteState"] == "1";

            // if on first page load state exist and now not state restoring - show restore panel
            var stateExist = CheckRestoreState(restore, delete);

            if (IsPostBack || !stateExist || restore || delete) 
                return;

            restorePanel.Visible = true;
            restorePanel.CreationTime =
                File.GetCreationTime(RestoreFileName).ToString(
                    ApplicationSettings.DisplayCultureInfo.DateTimeFormat);
        }

        protected void Save(Controls.AssetAttributePanels panel, string redirectOnSuccess = "",
                            bool calculateEditable = true)
        {
            AppFramework.Core.Classes.Asset asset;
            IDictionary<AssetAttribute, AppFramework.Core.Classes.Asset> dependencies;
            var isValid = panel.TryGetValidAssetWithDependencies(out asset, out dependencies);

            if (isValid)
            {
                try
                {
                    AssetsService.InsertAsset(asset, dependencies);

                    if (!string.IsNullOrEmpty(RestoreFileName) && File.Exists(RestoreFileName))
                        File.Delete(RestoreFileName);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    // serialize to XML to restore in case of database failure            
                    if (!string.IsNullOrEmpty(RestorePath) && !string.IsNullOrEmpty(RestoreFileName))
                    {
                        try
                        {
                            Directory.CreateDirectory(RestorePath);
                            using (var fs = File.Open(RestoreFileName, FileMode.Create))
                            {
                                Asset.Serialize(fs);
                            }
                        }
                        catch (Exception serializationException)
                        {
                            Logger.Warn(serializationException);
                        }
                    }
                    throw;
                }

                if (string.IsNullOrEmpty(redirectOnSuccess))
                {
                    redirectOnSuccess = string.Format("~/Asset/View.aspx?AssetUID={0}&AssetTypeUID={1}",
                                                      Asset.UID, Asset.GetConfiguration().UID);
                    long screenId;
                    if (Request["ScreenId"] != null && long.TryParse(Request["ScreenId"].ToString(), out screenId))
                        redirectOnSuccess += "&ScreenId=" + screenId;
                }
                Response.Redirect(redirectOnSuccess);
            }
        }

        /// <summary>
        /// Determines to which column this panel is relates
        /// </summary>
        /// <param name="columnCode"></param>
        /// <returns></returns>
        public bool IsPanelVisible(bool isLeftColumn)
        {
            // have only OneColumn layout for old UI
            bool res = true & isLeftColumn;
            PanelNumber += isLeftColumn ? 1 : 0;
            return res;
        }

        public bool IsSeparatorVisible()
        {
            // have only OneColumn layout for old UI
            return false;
        }

        /// <summary>
        /// Gets layout CSS file name.
        /// </summary>
        /// <returns>Path to CSS style file for selected assetType layout</returns>
        public virtual string LayoutCssClass()
        {
            // hardcoded OneColumn layout;
            const string cssClass = ".panels_leftcol_container {width:100%;} .panels_rightcol_container { width:100%;}";
            return cssClass;
        }

        /// <summary>
        /// Checks the state of the restore point and load serializaed state if needed. 
        /// Returns true if save point exsits, returns false if point not exist
        /// </summary>
        public bool CheckRestoreState(bool restoreState, bool deleteState)
        {
            var dirName = AuthenticationService.CurrentUserId.ToString();
            RestorePath = Path.Combine(ApplicationSettings.TempFullPath, dirName) + "\\";
            RestoreFileName = RestorePath + ImportExportManager.GetFileName(AssetType.Name, AssetType.ID);
            var savePointExists = File.Exists(RestoreFileName);

            if (restoreState && savePointExists)
            {
                if (!deleteState)
                {
                    using (var fs = File.Open(RestoreFileName, FileMode.Open))
                    {
                        try
                        {
                            Asset.Deserialize(fs);
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn("Cannot restore previously saved asset.", ex);
                        }
                    }
                }
                File.Delete(RestoreFileName);
            } 
            else if (deleteState)
            {
                File.Delete(RestoreFileName);
            }
            return savePointExists;
        }
    }
}
