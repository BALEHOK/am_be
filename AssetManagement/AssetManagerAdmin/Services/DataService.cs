using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.Core.Calculation;
using AppFramework.Core.ConstantsEnumerators;
using AssetManager.Infrastructure.Models.TypeModels;
using AssetManagerAdmin.WebApi;
using AppFramework.ConstantsEnumerators;
using Common.Logging;
using System.Threading.Tasks;
using AssetManagerAdmin.ViewModels;
using AssetManagerAdmin.Services;
using AssetManagerAdmin.Infrastructure;
using GalaSoft.MvvmLight.Messaging;

namespace AssetManagerAdmin.Model
{
    public class DataService : IDataService
    {
        private readonly IAssetsApiManager _assetsApiManager;
        private readonly ILog _logger;
        private readonly IFrameNavigationService _navigationService;
        private readonly IDialogService _dialogService;

        public AssetTypeModel CurrentAssetType { get; set; }
        public AttributeTypeModel CurrentAssetAttribute { get; set; }

        public DataService(
            IAssetsApiManager assetsApiManager, 
            IFrameNavigationService navigationService,
            IDialogService dialogService,
            ILog logger)
        {
            _navigationService = navigationService;
            _assetsApiManager = assetsApiManager;
            _dialogService = dialogService;
            _logger = logger;
        }
                
        public Task<TypesInfoModel> GetTypesInfo(UserInfo user, string server)
        {
            var api = _assetsApiManager.GetAssetApi(server, user);

            Func<Task<TypesInfoModel>, TypesInfoModel> transform = (task) => 
            {
                if (task.Exception != null)
                {
                    _logger.Error(task.Exception);
                    _dialogService.ShowMessage(new StatusMessage(task.Exception));
                    throw task.Exception;
                }

                var result = task.Result;

                // get a list of relation attributes
                var relationAttributes =
                    result.ActiveTypes.SelectMany(t => t.Attributes.Where(attr => attr.RelationId != 0)).ToList();
                // connect relation types models
                relationAttributes.ForEach(attr =>
                {
                    var relationType = result.ActiveTypes.SingleOrDefault(t => t.Id == attr.RelationId);
                    if (relationType != null)
                        attr.RelationType = relationType;
                });

                // get all attributes
                var allAttributes = result.ActiveTypes.SelectMany(t => t.Attributes).DistinctBy(a => a.Id).ToList();
                // get all panels
                var panels = result.ActiveTypes.SelectMany(t => t.Screens.SelectMany(s => s.Panels)).ToList();
                // connect attribute models to panels
                panels.ForEach(panel =>
                {
                    panel.Attributes.ForEach(panelAttribute =>
                    {
                        // find attribute model by Id in panel attribute
                        var attributeModelInAssettype = allAttributes
                            .Single(attribute => attribute.Id == panelAttribute.AttributeId);

                        var attributeModelInPanel = attributeModelInAssettype.ShallowCopy();
                        if (!string.IsNullOrEmpty(panelAttribute.ScreenFormula))
                        { 
                            // connect screen formula to model
                            attributeModelInAssettype.ScreenFormula = panelAttribute.ScreenFormula; // this is used to higlights type in dropdown
                            attributeModelInPanel.ScreenFormula = panelAttribute.ScreenFormula; // this is used to highlight attribute in list
                        }

                        // fill attribute model
                        panelAttribute.AttributeModel = attributeModelInPanel;
                    });
                });
                return result;
            };

            return api.GetTypesInfo().ContinueWith(
                result => transform(result));
        }

        public List<MenuItemViewModel> GetMainMenuItems(UserInfo user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var mainMenu = MenuConfig.GetMenu(_navigationService);

            if (user.UserModel.UserRole != PredefinedRoles.Administrators.ToString())
            {
                if (user.UserModel.UserRole == PredefinedRoles.Users.ToString())
                    mainMenu.Remove(mainMenu.Single(i => i.Id == 0));

                if (user.UserModel.UserRole == PredefinedRoles.SuperUser.ToString() ||
                    user.UserModel.UserRole == PredefinedRoles.Users.ToString())
                {
                    mainMenu.Remove(mainMenu.Single(i => i.Id == 1));
                    mainMenu.Remove(mainMenu.Single(i => i.Id == 2));
                }

                if (!user.UserModel.UserRights.Contains(SecuredModules.ReportsBuilder))
                    mainMenu.Remove(mainMenu.Single(i => i.Id == 3));
                if (!user.UserModel.UserRights.Contains(SecuredModules.InventoryScanner))
                    mainMenu.Remove(mainMenu.Single(i => i.Id == 4));
                if (!user.UserModel.UserRights.Contains(SecuredModules.StockScanner))
                    mainMenu.Remove(mainMenu.Single(i => i.Id == 5));
            }

            return mainMenu;
        }

        public List<ValidationButton> GetValidationButtons()
        {
            var result = new List<ValidationButton>
            {
                new ValidationButton {Name = "Value", Text = "[@value]"},
                new ValidationButton {Name = "RegEx", Text = "RegEx([''], [@value])"},
                new ValidationButton {Name = "IsDigit", Text = "IsDigit([@value])"},
                new ValidationButton {Name = "IsIP", Text = "IsIP([@value])"},
                new ValidationButton {Name = "IsBarcode", Text = "IsBarcode([@value])"},
                new ValidationButton {Name = "IsEmail", Text = "IsEmail([@value])"},
                new ValidationButton {Name = "IsUrl", Text = "IsUrl([@value])"},
                new ValidationButton {Name = "Unique", Text = "Unique([@value])"},
                new ValidationButton {Name = "SystemUnique", Text = "SystemUnique([@value])"},
                new ValidationButton {Name = "()", Text = "()"},
                new ValidationButton {Name = "and", Text = "and"},
                new ValidationButton {Name = "not", Text = "not"},
                new ValidationButton {Name = "or", Text = "or"},
                new ValidationButton {Name = ">", Text = ">"},
                new ValidationButton {Name = "<", Text = "<"},
                new ValidationButton {Name = "=", Text = "="},
                new ValidationButton {Name = "<>", Text = "<>"},
                new ValidationButton {Name = ">=", Text = ">="},
                new ValidationButton {Name = "<=", Text = "<="}
            };
            return result;
        }
    }
}