using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.Core.Calculation;
using AppFramework.Core.ConstantsEnumerators;
using AssetManager.Infrastructure.Models.TypeModels;
using AssetManagerAdmin.WebApi;

namespace AssetManagerAdmin.Model
{
    public class DataService : IDataService
    {
        public TypesInfoModel TypesInfo { get; set; }
        public UserInfo CurrentUser { get; set; }
        public AssetTypeModel CurrentAssetType { get; set; }
        public AttributeTypeModel CurrentAssetAttribute { get; set; }
        public ServerConfig SelectedServer { get; set; }

        public void GetTypesInfo(string server, Action<TypesInfoModel, Exception> callback)
        {
            var api = new AssetsApi(server, CurrentUser);
            api.GetTypesInfo().ContinueWith(task =>
            {
                var result = task.Result;

                // get a list of relation attributes
                var relationAttributes =
                    result.ActiveTypes.SelectMany(t => t.Attributes.Where(attr => attr.RelationId != 0)).ToList();
                // connect relation types models
                relationAttributes.ForEach(attr =>
                {
                    var relationType = result.ActiveTypes.Single(t => t.Id == attr.RelationId);
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

                callback(result, null);
            });
        }

        public void GetMainMenuItems(Action<List<MainMenuItem>, Exception> callback)
        {
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

            if (CurrentUser.UserModel.UserRole != UserRoles.Administrators)
            {
                if (CurrentUser.UserModel.UserRole == UserRoles.Users)
                    mainMenu.Remove(mainMenu.Single(i => i.Id == 0));

                if (CurrentUser.UserModel.UserRole == UserRoles.SuperUser ||
                    CurrentUser.UserModel.UserRole == UserRoles.Users)
                {
                    mainMenu.Remove(mainMenu.Single(i => i.Id == 1));
                    mainMenu.Remove(mainMenu.Single(i => i.Id == 2));
                }

                if (!CurrentUser.UserModel.UserRights.Contains(SecuredModules.ReportsBuilder))
                    mainMenu.Remove(mainMenu.Single(i => i.Id == 3));
                if (!CurrentUser.UserModel.UserRights.Contains(SecuredModules.InventoryScanner))
                    mainMenu.Remove(mainMenu.Single(i => i.Id == 4));
                if (!CurrentUser.UserModel.UserRights.Contains(SecuredModules.StockScanner))
                    mainMenu.Remove(mainMenu.Single(i => i.Id == 5));
            }

            callback(mainMenu, null);
        }

        public void GetValidationButtons(Action<List<ValidationButton>, Exception> callback)
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
            callback(result, null);
        }
    }
}