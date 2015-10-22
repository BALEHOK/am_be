using System.Web.UI;
using AppFramework.Core.Calculation;
using AppFramework.Core.Classes.ScreensServices;
using AppFramework.Core.PL;
using AppFramework.Core.Validation;
using AppFramework.DataProxy;
using Microsoft.Practices.Unity;

namespace AssetSite.Controls
{
    using AppFramework.ConstantsEnumerators;
    using AppFramework.Core.Classes;
    using AppFramework.Core.ConstantsEnumerators;
    using Asset;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;
    using Core = AppFramework.Core.Classes;
    using AppFramework.Entities;

    public partial class AssetAttributePanels : UserControl
    {
        [Dependency]
        public IAssetTypeRepository AssetTypeRepository { get; set; }
        [Dependency]
        public IAssetsService AssetsService { get; set; }
        [Dependency]
        public IUnitOfWork UnitOfWork { get; set; }
        [Dependency]
        public IAttributeFieldFactory AttributeFieldFactory { get; set; }
        [Dependency]
        public IValidationServiceNew ValidationService { get; set; }
        [Dependency]
        public IAttributeCalculator AttributeCalculator { get; set; }
        [Dependency]
        public IScreensService ScreensService { get; set; }
        [Dependency]
        public IAssetPanelsAdapter AssetPanelsAdapter { get; set; }

        public bool IsValid { get; set; }

        public bool Editable { get; set; }

        public bool MySettingsPage { get; set; }

        public Asset Asset { get; set; }

        private Dictionary<AttributePanel, List<AssetAttribute>> _panels;
        private AssetController _assetController;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Asset == null)
                throw new NullReferenceException();

            var isAdminUser = HttpContext.Current.User.IsInRole(
                PredefinedRoles.Administrators.ToString());

            // limitations for users creation/editing for other than Administrator roles
            if (Asset.IsUser & !isAdminUser)
            {
                var restrictedAttributes = new List<string>(8)
                {
                    "Password",
                    "Email",
                    "Users",
                    "Permissions On Users",
                    "Role",
                    "Staff",
                    "Unionist",
                    "Contact"
                };

                if (Asset.IsNew)
                    Asset[AttributeNames.Role].Value = ((int)PredefinedRoles.OnlyPerson).ToString();
                else
                    restrictedAttributes.Add(AttributeNames.Name);

                foreach (var attribute in Asset.GetConfiguration().Attributes)
                {
                    // Disable attribute editing for User asset if editor is not in Administrators role
                    if (restrictedAttributes.Contains(attribute.Name))
                    {
                        attribute.AllowEditValue = false;
                        attribute.IsShownOnPanel = false;
                    }
                }
            }

            _assetController = new AssetController
            {
                AssetType = Asset.GetConfiguration(),
                Asset = Asset
            };

            long screenId;
            AssetTypeScreen screen;
            _assetController = new AssetController { AssetType = Asset.GetConfiguration(), Asset = Asset };

            if (Request["ScreenId"] != null && long.TryParse(Request["ScreenId"], out screenId))
            {
                // calculate screen formulas
                screen = ScreensService.GetScreenById(screenId);
                Asset = AttributeCalculator.PreCalculateAsset(Asset, screenId);
            }
            else
            {
                screen = ScreensService.GetScreensByAssetTypeUid(Asset.Configuration.UID).Single(s => s.IsDefault);
            }

            _panels = AssetPanelsAdapter.GetPanelsByScreen(Asset, screen);
            panelsRepeater.DataSource = _panels;
            panelsRepeater.DataBind();
        }

        public bool TryGetValidAssetWithDependencies(out Asset asset, out IDictionary<AssetAttribute, Asset> dependencies)
        {
            asset = null;
            dependencies = null;
            IsValid = true;
            ValidationRep.Text = String.Empty;

            //for each panel in repeater's panels
            foreach (RepeaterItem item in panelsRepeater.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    var uiPanel = (NewAttributePanel)item.FindControl("AttributesPanel");
                    foreach (var attribute in uiPanel.GetAttributes().Where(a => a != null))
                    {
                        // validate attribute
                        IsValid &= _validate(attribute);
                    }
                }
            }

            if (string.IsNullOrEmpty(Asset.Name) &&
                (Asset.GetConfiguration().AutoGenerateNameType == Enumerators.TypeAutoGenerateName.InsertOnly ||
                 Asset.GetConfiguration().AutoGenerateNameType == Enumerators.TypeAutoGenerateName.InsertUpdate))
                Asset.Name = Asset.GenerateName();

            if (IsValid)
            {
                asset = Asset;
                dependencies = AssetPanelsAdapter.DependencyDescriptor;
            }

            return IsValid;
        }

        private bool _validate(AssetAttribute attribute)
        {
            if (attribute == null)
            {
                return true;
            }
            
            var validationResult = ValidationService.ValidateAttribute(attribute);
            if (validationResult.IsValid)
                return true;

            ValidationRep.Text += string.Format("<b>Validation of attribute {0} failed</b><br />",
                Server.HtmlEncode(attribute.GetConfiguration().NameLocalized));
            ValidationRep.Text = string.Join("<br />", validationResult.ResultLines.Select(l => l.Message));
            return false;
        }

        public bool IsPanelVisible(bool isLeftColumn)
        {
            return _assetController.IsPanelVisible(isLeftColumn);
        }

        public bool IsSeparatorVisible()
        {
            return _assetController.IsSeparatorVisible();
        }
    }
}