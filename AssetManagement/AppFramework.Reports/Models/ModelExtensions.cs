using System.Collections.Generic;
using System.Linq;
using AppFramework.Entities;
using Newtonsoft.Json;

namespace AppFramework.Reports.Models
{
    public static class ModelExtensions
    {
        public static AssetsContainer ToModel(this IEnumerable<IIndexEntity> entities, string subtitle)
        {
            var dataSource = new List<AssetViewModel>();
            foreach (var entity in entities)
            {
                var assetModel = new AssetViewModel
                {
                    Name = entity.Name,
                    Attributes = GetDisplayAttributes(entity.DisplayValues)
                };
                dataSource.Add(assetModel);
            }
            return new AssetsContainer {Assets = dataSource, ReportSubtitle = subtitle};
        }

        private static List<AssetAttributeViewModel> GetDisplayAttributes(string displayValuesJson)
        {
            return JsonConvert.DeserializeObject<KeyValuePair<string, string>[]>(displayValuesJson)
                .Select(v => new AssetAttributeViewModel {Name = v.Key, Value = v.Value})
                .ToList();
        }
    }
}