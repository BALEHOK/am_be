using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppFramework.Core.Classes;
using AppFramework.DataProxy;

namespace AppFramework.Core.PL
{
    public class TaskRightsDatasourceAdapter : TaskRightsList
    {
        private readonly TaskRightsList _rights;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetTypeRepository _assetTypeRepository;

        public TaskRightsDatasourceAdapter(
            TaskRightsList rights, 
            IUnitOfWork unitOfWork,
            IAssetTypeRepository assetTypeRepository)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            
            this._rights = rights;
        }

        public List<RightsEntryGridRow> GetEnumerator()
        {
            List<RightsEntryGridRow> returnList = new List<RightsEntryGridRow>();
            foreach (IGrouping<long, TaskRightsEntry> re in _rights.Items.GroupBy(r => r.ViewID))
            {
                List<string> categories = new List<string>();
                List<string> assettypes = new List<string>();
                List<string> departments = new List<string>();
                foreach (TaskRightsEntry entry in re.ToList())
                {
                    if (entry.TaxonomyItemId != 0)
                    {
                        var item = _unitOfWork.AssetsTaxonomiesRepository.Get(at => at.TaxonomyItemId == entry.TaxonomyItemId).FirstOrDefault();
                        if (item != null)
                        {
                            categories.Add(item.TaxonomyItemName + ", ");
                        }
                    }

                    if (entry.DynEntityConfigId != 0)
                    {
                        var at = _assetTypeRepository.GetById((long)entry.DynEntityConfigId);
                        assettypes.Add(at.Name + ", ");
                    }
                }

                string anyString = "Any";
                if (categories.Count == 0) categories.Add(anyString);
                if (assettypes.Count == 0) assettypes.Add(anyString);               

                returnList.Add(new RightsEntryGridRow()
                {
                    AssetTypes = String.Concat(assettypes.Distinct().ToArray()).TrimEndComma(),
                    Categories = String.Concat(categories.Distinct().ToArray()).TrimEndComma(),
                    Departments = String.Concat(departments.Distinct().ToArray()).TrimEndComma(),
                    ViewID = re.Key,
                    IsDeny = re.First().IsDeny
                });
            }
            return returnList;
        }
    }
}
