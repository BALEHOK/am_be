using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.Extensions;
using AppFramework.Entities;
using AssetManager.Infrastructure.Models;
using Common.Logging;

namespace AssetManager.Infrastructure.Helpers
{
    public static class AssetModelExtensions
    {
        public static IEnumerable<AssetPanelModel> ToPanelModels(
            this Dictionary<AttributePanel, List<AssetAttribute>> panelsAttributes,
            Func<AssetAttribute, AttributeModel> attributeToModelConvertMethod)
        {
            return from p in panelsAttributes
                select new AssetPanelModel
                {
                    Id = p.Key.AttributePanelId,
                    Name = p.Key.Name.Localized(),
                    IsChildAssets = p.Key.IsChildAssets,
                    ChildAssetAttrId = p.Key.ChildAssetAttrId.GetValueOrDefault(),
                    Attributes = from attribute in p.Value
                        select attributeToModelConvertMethod(attribute)
                };
        }

        public static IEnumerable<AttributeModel> GetAttributes(
            this AssetModel model, long? screenId = null)
        {
            var logger = LogManager.GetCurrentClassLogger();
            AssetScreenModel screen = null;

            if (screenId.HasValue)
            {
                screen = model.Screens.SingleOrDefault(s => s.Id == screenId.Value);

                if (screen == null)
                {
                    logger.WarnFormat(
                        @"Screen with id {0} not found in model (id: {1}, assetTypeId: {2}).
                        Fallback to default screen.", 
                        screenId, model.Id, model.AssetTypeId);

                    screen = GetDefaultScreen(model, logger);
                }
            }
            else
            {
                screen = GetDefaultScreen(model, logger);
            }

            return screen
                .Panels
                .SelectMany(p => p.Attributes)
                .ToList();
        }

        private static AssetScreenModel GetDefaultScreen(AssetModel model, ILog logger)
        {
            AssetScreenModel screen;
            var defaultScreensCount = model.Screens.Where(s => s.IsDefault).Count();

            if (defaultScreensCount == 0)
            {
                throw new Exception(string.Format(
                    @"Cannot get attributes from model (id: {0}, assetTypeId: {1}): 
                        default screen not defined and no screenId provided.",
                    model.Id, model.AssetTypeId));
            }
            else if (defaultScreensCount > 1)
            {
                logger.WarnFormat(
                    @"More than one default screen found in model 
                        (id: {0}, assetTypeId: {1}). Taking first one.",
                    model.Id, model.AssetTypeId);

                screen = model.Screens.First(s => s.IsDefault);
            }
            else
            {
                screen = model.Screens.Single(s => s.IsDefault);
            }

            return screen;
        }
    }
}