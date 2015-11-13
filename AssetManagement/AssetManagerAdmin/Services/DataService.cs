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

namespace AssetManagerAdmin.Model
{
    public class DataService : IDataService
    {
        private readonly IAssetsApiManager _assetsApiManager;
        private readonly ILog _logger;

        public AssetTypeModel CurrentAssetType { get; set; }
        public AttributeTypeModel CurrentAssetAttribute { get; set; }

        public DataService(IAssetsApiManager assetsApiManager, ILog logger)
        {
            _assetsApiManager = assetsApiManager;
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
                        var attributeModel =
                            allAttributes.Single(attribute => attribute.Id == panelAttribute.AttributeId);
                        // connect screen formula to model
                        attributeModel.ScreenFormula = panelAttribute.ScreenFormula;
                        // fill attribute model
                        panelAttribute.AttributeModel = attributeModel;
                    });
                });
                return result;
            };

            return api.GetTypesInfo().ContinueWith(result => transform(result));
        }

        public List<MainMenuItem> GetMainMenuItems(UserInfo user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var mainMenu = new List<MainMenuItem>
            {
                //new MainMenuItem
                //{
                //    Id = 0,
                //    Name = "Administration"
                //},
                new MainMenuItem
                {
                    Id = 1,
                    Name = "Calculation and Validation"
                },
//                new MainMenuItem
//                {
//                    Id = 2,
//                    Name = "Validation builder"
//                },
                new MainMenuItem
                {
                    Id = 3,
                    Name = "Reports"
                },
                new MainMenuItem
                {
                    Id = 4,
                    Name = "Inventory scanner"
                },
                new MainMenuItem
                {
                    Id = 5,
                    Name = "Stock scanner"
                }
            };

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