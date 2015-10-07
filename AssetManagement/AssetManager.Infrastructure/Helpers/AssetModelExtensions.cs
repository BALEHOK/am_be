using AppFramework.Core.Classes;
using AppFramework.Core.DTO;
using AppFramework.Entities;
using AssetManager.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
                       Name = p.Key.Name,
                       Attributes = from attribute in p.Value
                                    select attributeToModelConvertMethod(attribute)
                   };
        }


        public static IEnumerable<AttributeModel> GetAttributes(
            this AssetModel model, long? screenId = null)
        {
            return model
                .Screens
                .Single(s => (screenId.HasValue && s.Id == screenId) || s.IsDefault)
                .Panels
                .SelectMany(p => p.Attributes)
                .ToList();
        }
       
    }
}