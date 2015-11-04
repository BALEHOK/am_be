using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AssetManager.Infrastructure.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace AssetManager.Infrastructure
{
    public interface IModelFactory
    {
        AttributeModel GetAttributeModel(AssetAttribute attribute);

        void AssignValue(AssetAttribute attribute, JToken value);

        void AssignInternalAttributes(Asset asset, long userId);

        AssetModel GetAssetModel(Asset asset, Permission? permission = null);
        void AssignValueUnconditional(AssetAttribute attribute, string value);
    }
}
