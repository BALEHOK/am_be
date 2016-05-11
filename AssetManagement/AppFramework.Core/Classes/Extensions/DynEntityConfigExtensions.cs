using AppFramework.Entities;

namespace AppFramework.Core.Classes.Extensions
{
    public static class DynEntityConfigExtensions
    {
        public static string NameLocalized(this DynEntityConfig entityConfig)
        {
            return entityConfig.Name.Localized();
        }
    }
}