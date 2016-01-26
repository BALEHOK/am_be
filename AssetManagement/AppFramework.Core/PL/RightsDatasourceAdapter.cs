using AppFramework.DataProxy;

namespace AppFramework.Core.PL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AppFramework.ConstantsEnumerators;
    using AppFramework.Core.Classes;
    using AppFramework.Core.ConstantsEnumerators;
    using AppFramework.Core.AC.Authentication;

    public class RightsDatasourceAdapter
    {
        private readonly List<RightsEntry> _rights;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IAssetsService _assetsService;
        private readonly IUnitOfWork _unitOfWork;

        public RightsDatasourceAdapter(
            List<RightsEntry> rights,
            IUnitOfWork unitOfWork,
            IAssetTypeRepository assetTypeRepository,
            IAssetsService assetsService)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            _assetsService = assetsService;
            _rights = rights;
        }

        public List<RightsEntryGridRow> GetEnumerator()
        {
            List<RightsEntryGridRow> returnList = new List<RightsEntryGridRow>();
            foreach (IGrouping<long, RightsEntry> re in _rights.GroupBy(r => r.ViewID))
            {
                List<string> categories = new List<string>();
                List<string> assettypes = new List<string>();
                List<string> departments = new List<string>();
                foreach (RightsEntry entry in re.ToList())
                {
                    if (entry.TaxonomyItemId != 0)
                    {
                        var item = _unitOfWork.TaxonomyItemRepository.GeTaxonomyItembyId(entry.TaxonomyItemId);
                        if (item != null)
                        {
                            categories.Add(item.Name + ", ");
                        }
                    }
                    if (entry.DepartmentID != 0)
                    {
                        var at = _assetTypeRepository.GetPredefinedAssetType(PredefinedEntity.Department);
                        var dept = _assetsService.GetAssetById(entry.DepartmentID, at);
                        if (dept != null)
                            departments.Add(dept[AttributeNames.Name].Value + ", ");
                    }
                    if (entry.AssetTypeID != 0)
                    {
                        var at = _assetTypeRepository.GetById(entry.AssetTypeID);
                        assettypes.Add(at.Name + ", ");
                    }
                }

                string anyString = "Any";
                if (categories.Count == 0) categories.Add(anyString);
                if (assettypes.Count == 0) assettypes.Add(anyString);
                if (departments.Count == 0) departments.Add(anyString);

                var permission = re.First().Permission;
                returnList.Add(new RightsEntryGridRow()
                {
                    AssetTypes = String.Concat(assettypes.Distinct().ToArray()).TrimEndComma(),
                    Categories = String.Concat(categories.Distinct().ToArray()).TrimEndComma(),
                    Departments = String.Concat(departments.Distinct().ToArray()).TrimEndComma(),
                    Permission = permission.CanDelete() ? "RWRW" : permission.ToString().Replace('D', '-'),
                    ViewID = re.Key,
                    IsDeny = re.First().IsDeny,
                    CanDelete = permission.CanDelete()
                });
            }
            return returnList;
        }

    }
}
