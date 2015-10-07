using System;
using System.Linq;
using AppFramework.DataProxy;

namespace AppFramework.Core.Classes.SearchEngine.Presentation
{
    class Helper
    {
        private readonly IUnitOfWork _unitOfWork;

        public Helper(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        public string BuildSubtitle(Entities.IIndexEntity entity, AssetType assetType)
        {
            string normalFormat = string.Format("{0}{1}{2}",
                assetType.Name,
                string.IsNullOrEmpty(entity.User) ? string.Empty : string.Format(" &mdash; {0}", entity.User),
                string.IsNullOrEmpty(entity.Location) ? string.Empty : string.Format(" &mdash; {0}", entity.Location));

            var taxonomyItems = _unitOfWork.TaxonomyItemRepository.GetTaxonomyItemsByAssetTypeId(assetType.ID).ToList();
            string dataFormat = string.Format("{0}{1}",
                assetType.Name,
                taxonomyItems.Any() ?
                    string.Format(" ({0})", string.Join(", ", taxonomyItems.Select(t => t.Name))) :
                    string.Empty);
            return string.IsNullOrEmpty(entity.Location) ? dataFormat : normalFormat;
        }
    }
}
