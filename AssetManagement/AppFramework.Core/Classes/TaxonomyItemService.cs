using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppFramework.Core.AC.Authentication;
using AppFramework.DataProxy;

namespace AppFramework.Core.Classes
{
    public class TaxonomyItemService : ITaxonomyItemService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TaxonomyItemService(
            IAuthenticationService authenticationService,
            IAssetTypeRepository assetTypeRepository,
            IUnitOfWork unitOfWork)
        {
            if (authenticationService == null)
                throw new ArgumentNullException("authenticationService");
            _authenticationService = authenticationService;
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Returns TaxonomyItem by its unique ID and active=1
        /// </summary>
        /// <param name="uid">Unique ID</param>
        /// <returns>TaxonomyItem</returns>
        public TaxonomyItem GetActiveItemById(long id)
        {
            var entity = (from it in _unitOfWork.TaxonomyItemRepository.Get(include: ti => ti.Taxonomy)
                          where it.TaxonomyItemId == id && it.ActiveVersion
                          select it).FirstOrDefault();
            return entity != null 
                ? new TaxonomyItem(entity) 
                : null;
        }


        /// <summary>
        /// Returns TaxonomyItem by its unique ID
        /// </summary>
        /// <param name="uid">Unique ID</param>
        /// <returns>TaxonomyItem</returns>
        public TaxonomyItem GetByUid(long uid)
        {
            return new TaxonomyItem(
                _unitOfWork
                .TaxonomyItemRepository
                .Single(ti => ti.TaxonomyItemUid == uid, include: t => t.Taxonomy));
        }


        // TODO : refactor
        /// <summary>
        /// Saves taxonomy item and all associated asset types
        /// </summary>
        public void Save(Entities.TaxonomyItem taxonomyItem)
        {
            taxonomyItem.UpdateDate = DateTime.Now;
            taxonomyItem.UpdateUserId = _authenticationService.CurrentUserId;

            if (taxonomyItem.TaxonomyItemUid == 0)
                _unitOfWork.TaxonomyItemRepository.Insert(taxonomyItem);
            else
                _unitOfWork.TaxonomyItemRepository.Update(taxonomyItem);

            throw new NotImplementedException();
            //List<AssetType> dbAssetTypes = _getAssignedAssetTypes(taxonomyItem);
            //foreach (AssetType dbType in dbAssetTypes)
            //{
            //    var memType = AssignedAssetTypes.SingleOrDefault(at => at.UID == dbType.UID);
            //    if (memType != null)
            //    {
            //        // remove from memory - already exists in DB
            //        AssignedAssetTypes.Remove(memType);
            //    }
            //    else
            //    {
            //        var data = _unitOfWork.DynEntityConfigTaxonomyRepository.SingleOrDefault(dect =>
            //           dect.DynEntityConfigId == dbType.ID && dect.TaxonomyItemId == Id);
            //        if (data != null)
            //        {
            //            // remove from DB - relation does not exists anymore
            //            _unitOfWork.DynEntityConfigTaxonomyRepository.Delete(data);
            //        }
            //    }
            //}

            //// add all other assetTypes to database
            //foreach (var at in AssignedAssetTypes)
            //{
            //    _unitOfWork.DynEntityConfigTaxonomyRepository.Insert(new DynEntityConfigTaxonomy()
            //    {
            //        DynEntityConfigId = at.ID,
            //        TaxonomyItemId = Id
            //    });
            //}
            _unitOfWork.Commit();
        }

        public TaxonomyItem GetParentItem(Entities.TaxonomyItem taxonomyItem)
        {
            if (!taxonomyItem.ParentTaxonomyItemUid.HasValue)
                return null;

            if (taxonomyItem.ParentItem == null)
                _unitOfWork.TaxonomyItemRepository.LoadProperty(taxonomyItem, e => e.ParentItem);
            return new TaxonomyItem(taxonomyItem.ParentItem);
        }

        /// <summary>
        /// Returns the list of all assets assigned to this taxonomy item
        /// </summary>
        /// <returns>List of AssetType</returns>
        public List<AssetType> GetAssignedAssetTypes(Entities.TaxonomyItem taxonomyItem)
        {
            var dectSource = _unitOfWork.DynEntityConfigTaxonomyRepository.AsQueryable();
            var decSource = _unitOfWork.DynEntityConfigRepository.AsQueryable();
            var items = (from dect in dectSource
                         from dec in decSource
                         where dec.DynEntityConfigId == dect.DynEntityConfigId
                             && dect.TaxonomyItemId == taxonomyItem.TaxonomyItemId
                             && dec.ActiveVersion
                             && dec.Active
                         select dect).ToList();
            var types = new List<AssetType>(items.Count);
            foreach (var item in items)
            {
                var type = _assetTypeRepository.GetById(item.DynEntityConfigId);
                if (_authenticationService.IsReadingAllowed(type))
                    types.Add(type);
            }
            return types;
        }
   }
}
