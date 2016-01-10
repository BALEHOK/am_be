using AssetManagerAdmin.Services;
using AssetManagerAdmin.ViewModels;
using System;
using System.Collections.Generic;

namespace AssetManagerAdmin.Infrastructure
{
    class MenuConfig
    {
        public static List<MenuItemViewModel> GetMenu(IFrameNavigationService navigationService)
        {
            Func<string, object, Action> navigateTo = (pageKey, parameter) =>
            {
                return () => { navigationService.NavigateTo(pageKey, parameter); };
            };

            return new List<MenuItemViewModel>
            {
                new MenuItemViewModel
                {
                    Name = "File",
                    MenuItems = new List<MenuItemViewModel>
                    {
                        new MenuItemViewModel
                        {
                            Name = "Logout",
                            OnClick = navigateTo(
                                ViewModelLocator.AuthViewKey,
                                AppActions.LoggingOut)
                        }
                    },
                },
                new MenuItemViewModel
                {
                    Id = 1,
                    Name = "Formulas and Validation",
                    MenuItems = new List<MenuItemViewModel>
                    {
                        new MenuItemViewModel
                        {
                            Name = "Database Formulas",
                            OnClick = navigateTo(
                                ViewModelLocator.FormulaBuilderKey,
                                FormulaBuilderContextType.DbFormulas)
                        },
                        new MenuItemViewModel
                        {
                            Name = "Screen Formulas",
                            OnClick = navigateTo(
                                ViewModelLocator.FormulaBuilderKey,
                                FormulaBuilderContextType.ScreenFormulas)
                        },
                        new MenuItemViewModel
                        {
                            Name = "Validation",
                            OnClick = navigateTo(
                                ViewModelLocator.FormulaBuilderKey,
                                FormulaBuilderContextType.Validation)
                        },
                    }
                },
                new MenuItemViewModel
                {
                    Id = 3,
                    Name = "Report Builder",
                    OnClick = navigateTo(
                        ViewModelLocator.ReportBuilderKey,
                        null)
                },
                new MenuItemViewModel
                {
                    Id = 4,
                    Name = "Inventory scanner"
                },
                new MenuItemViewModel
                {
                    Id = 5,
                    Name = "Stock scanner"
                }
            };
        }
    }
}
