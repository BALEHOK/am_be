using System.Web;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.ScreensServices;
using AppFramework.Core.ConstantsEnumerators;
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.Core.Interfaces;
using Microsoft.Practices.Unity;

namespace AppFramework.Core.PL
{
    [Themeable(true)]
    public class AssetsGrid : GridView
    {
        [Dependency]
        public IAuthenticationService AuthenticationService { get; set; }

        [Dependency]
        public IScreensService ScreensService { get; set; }

        private Entities.AssetTypeScreen _defaultScreen;

        private int LastColumnIndex
        {
            get { return this.Columns.Count - 1; }
        }

        /// <summary>
        /// Gets or sets if Delete button must be shown.
        /// </summary>
        public bool ShowDeleteButton { get; set; }

        public bool HistoryGrid { get; set; }

        public AssetsGrid()
        {
            AutoGenerateColumns = false;
            AllowPaging = true;
            PageSize = ApplicationSettings.RecordsPerPage;
            ShowDeleteButton = true;
            RowDataBound += new GridViewRowEventHandler(AssetsGrid_RowDataBound);
            Columns.Add(new TemplateField());
        }

        public void CreateColumns(Asset asset)
        {
            if (asset == null)
                throw new ArgumentNullException("Asset");

            Columns.Clear();
            AssetAttribute[] attributes;
            if (!HistoryGrid)
            {
                attributes = asset.Attributes
                                  .Where(a => a.GetConfiguration().IsShownInGrid)
                                  .OrderBy(a => a.GetConfiguration().DisplayOrder)
                                  .ToArray();
            }
            else
            {
                attributes = new AssetAttribute[] { 
                        asset[AttributeNames.Revision],
                        asset[AttributeNames.LocationId],
                        asset[AttributeNames.UpdateDate],
                        asset[AttributeNames.UpdateUserId],
                    };
            }

            foreach (AssetAttribute attr in attributes.TakeWhile((el, index) => index < Constants.MaxColumnsCount))
            {
                if (attr != null)
                {
                    var field = new TemplateField
                    {
                        HeaderText = attr.GetConfiguration().NameLocalized,
                        ItemTemplate = new AttributeItemTemplate(attr, AuthenticationService,
                            (HttpContext.Current.ApplicationInstance as IHttpUnityApplication).UnityContainer),
                    };
                    field.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                    // if attribute is type of financial and reading is denied, then skip adding it
                    Permission permission = AuthenticationService.GetPermission(asset);
                    if (!(attr.GetConfiguration().IsFinancialInfo && !permission.CanRead(true)))
                    {
                        this.Columns.Add(field);
                    }
                }
            }

            TemplateField commandButtons = new TemplateField()
            {
                ItemTemplate = new AssetsGridCommandButtonTemplate()
            };
            this.Columns.Add(commandButtons);
        }

        private void AssetsGrid_RowDataBound(Object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Asset bindedAsset = e.Row.DataItem as Asset;
                if (bindedAsset == null)
                    return;

                Permission assetPermission = AuthenticationService.GetPermission(bindedAsset);
                if (assetPermission.CanRead())
                {
                    AssetsGridCommandButton buttonView = new AssetsGridCommandButton()
                    {
                        ID = "btnView",
                        Type = AssetsGridCommandButton.ButtonType.View,
                        NavigateUrl = string.Format("/Asset/View.aspx?AssetUID={0}&AssetTypeUID={1}",
                                                    bindedAsset.UID, bindedAsset.DynEntityConfigUid)
                    };
                    e.Row.Cells[LastColumnIndex].Controls.Add(buttonView);

                    bool isActive = false;
                    bool.TryParse(bindedAsset[AttributeNames.ActiveVersion].Value, out isActive);
                    if (assetPermission.CanWrite() && isActive)
                    {
                        AssetsGridCommandButton buttonEdit = new AssetsGridCommandButton()
                        {
                            ID = "btnEdit",
                            Type = AssetsGridCommandButton.ButtonType.Edit,
                            NavigateUrl = string.Format("/Asset/Edit.aspx?AssetUID={0}&AssetTypeUID={1}",
                                                        bindedAsset.UID, bindedAsset.DynEntityConfigUid)
                        };

                        e.Row.Cells[LastColumnIndex].Controls.Add(buttonEdit);
                    }

                    if (assetPermission.CanDelete() && ShowDeleteButton && !PredefinedAsset.Contains(bindedAsset))
                    {
                        AssetsGridCommandButton buttonDelete = new AssetsGridCommandButton()
                        {
                            ID = "btnDelete",
                            Type = AssetsGridCommandButton.ButtonType.Delete,
                            NavigateUrl = "javascript:void(0);"
                        };
                        buttonDelete.Attributes.Add("onclick", string.Format("javascript:return DeleteAsset('{0}', {1});", bindedAsset.DynEntityConfigUid, bindedAsset.ID));
                        e.Row.Cells[LastColumnIndex].Controls.Add(buttonDelete);
                    }
                }
                else
                {
                    e.Row.Attributes["style"] = "display:none";
                }
                e.Row.Cells[LastColumnIndex].HorizontalAlign = HorizontalAlign.Right;
            }
        }
    }
}
