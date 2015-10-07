using AppFramework.Core.Classes.DynLists;
using AppFramework.Entities;
using AssetManager.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManager.Infrastructure.Helpers
{
    public static class DynListModelExtensions
    {
        public static DynListModel ToDto(this DynamicList list)
        {
            return new DynListModel
            {
                // refer to Base to get rid 
                // of extra proxy class in future
                Uid = list.Base.DynListUid,
                Name = list.Base.Name,
                Items = from item in list.Items
                        where item.Base.ActiveVersion
                        orderby item.Base.DisplayOrder
                        select new DynListItemModel
                        {
                            Id = item.Base.DynListItemId,
                            Uid = item.Base.DynListItemUid,
                            Value = item.Base.Value,
                        }
            };
        }
    }
}