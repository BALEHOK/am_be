using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFramework.Reports.Models
{
    public static class ModelExtensions
    {
        public static AssetsContainer ToModel(this IEnumerable<Entities.IIndexEntity> entities, string subtitle)
        {
            var dataSource = new List<AssetViewModel>();
            foreach (var entity in entities)
            {
                var assetModel = new AssetViewModel 
                { 
                    Name = entity.Name,
                    Attributes = new List<AssetAttributeViewModel>()
                    {
                        new AssetAttributeViewModel { Name = "Description", Value = entity.Description },
                        new AssetAttributeViewModel { Name = "BarCode", Value = entity.BarCode },
                        new AssetAttributeViewModel { Name = "Department", Value = entity.Department },
                        new AssetAttributeViewModel { Name = "Location", Value = entity.Location },
                    }
                };
                dataSource.Add(assetModel);
            }
            return new AssetsContainer { Assets = dataSource, ReportSubtitle = subtitle };
        }
    }
}
