using AppFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFramework.Core.Classes.Extensions
{
    public static class DynEntityConfigExtensions
    {
        public static string NameLocalized(this DynEntityConfig entityConfig)
        {
            return new TranslatableString(entityConfig.Name).GetTranslation();
        }
    }
}
