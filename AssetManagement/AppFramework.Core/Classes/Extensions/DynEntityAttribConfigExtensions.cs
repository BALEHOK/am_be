using AppFramework.Entities;

namespace AppFramework.Core.Classes.Extensions
{
    public static class DynEntityAttribConfigExtensions
    {
        public static string NameLocalized(this DynEntityAttribConfig attribConfig)
        {
            return new TranslatableString(attribConfig.Name).GetTranslation();
        }
    }
}