using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppFramework.Core.Classes.DynLists;
using AppFramework.DataProxy;
using AppFramework.Entities;
using AppFramework.Core.Exceptions;
using LinqKit;
using AppFramework.ConstantsEnumerators;
using Microsoft.Practices.Unity;

namespace AppFramework.Core.Classes
{
    public interface IDynamicListsService
    {
        /// <summary>
        /// Insert or update dynamic list and all items
        /// </summary>
        void Save(DynList entity);

        /// <summary>
        /// Gets the list by uid.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <returns></returns>
        DynamicList GetByUid(long uid);

        /// <summary>
        /// Gets all lists
        /// </summary>
        /// <returns></returns>
        List<DynamicList> GetAll();

        /// <summary>
        /// Deletes this instance from data base
        /// </summary>
        void Delete(DynList entity);

        IEnumerable<DynListValue> GetListValuesByAttribute(AssetAttribute attribute);

        IEnumerable<DynamicListValue> GetLegacyListValues(AssetTypeAttribute attrConfig, long assetUid);

        DynamicList GetByAttributeId(long attributeId);

        DynListItem GetListItemById(long dynListItemId);
    }

    public class DynamicListsService : IDynamicListsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAttributeRepository _attributeRepository;

        public DynamicListsService(IUnitOfWork unitOfWork, IAttributeRepository attributeRepository)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;

            if (attributeRepository == null)
                throw new ArgumentNullException("attributeRepository");
            _attributeRepository = attributeRepository;
        }

        /// <summary>
        /// Insert or update dynamic list and all items
        /// </summary>
        public void Save(DynList entity)
        {
            if (entity.DynListUid > 0)
            {
                _unitOfWork.DynListRepository.Update(entity);
            }
            else
            {
                _unitOfWork.DynListRepository.Insert(entity);
            }
            _unitOfWork.Commit();
        }

        /// <summary>
        /// Gets the list by uid.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <returns></returns>
        public DynamicList GetByUid(long uid)
        {
            var entity = _unitOfWork.DynListRepository.SingleOrDefault(
                d => d.DynListUid == uid,
                include: dl => dl.DynListItems.Select(dli => dli.AssociatedDynList));
            if (entity == null)
                throw new DynListNotFoundException("Cannot find DynList by uid " + uid);
            return new DynamicList(entity);
        }

        /// <summary>
        /// Gets all lists
        /// </summary>
        /// <returns></returns>
        public List<DynamicList> GetAll()
        {
            return _unitOfWork.DynListRepository.Get(dl => dl.Active, orderBy: lists => lists.OrderBy(l => l.Name))
                .Select(d => new DynamicList(d)).ToList();
        }

        /// <summary>
        /// Deletes this instance from data base
        /// </summary>
        public void Delete(DynList entity)
        {
            _unitOfWork.DynListRepository.Delete(entity);
            _unitOfWork.Commit();
        }

        public IEnumerable<DynListValue> GetListValuesByAttribute(AssetAttribute attribute)
        {
            var predicate = PredicateBuilder.True<DynListValue>();
            predicate = predicate.And(dlv => dlv.DynEntityAttribConfigUid == attribute.Configuration.UID);
            predicate = predicate.And(dlv => dlv.DynEntityConfigUid == attribute.Configuration.AssetTypeUID);
            predicate = predicate.And(dlv => dlv.AssetUid == attribute.ParentAsset.UID);
            return _unitOfWork.DynListValueRepository
                .Where(predicate)
                .OrderBy(dlv => dlv.ParentListId)
                .ToList();
        }

        [Obsolete("This method is for backwards compatibility")]
        public IEnumerable<DynamicListValue> GetLegacyListValues(
            AssetTypeAttribute attrConfig, long assetUid)
        {
            var predicate = PredicateBuilder.True<DynListValue>();
            predicate = predicate.And(dlv => dlv.DynEntityAttribConfigUid == attrConfig.UID);
            predicate = predicate.And(dlv => dlv.DynEntityConfigUid == attrConfig.AssetTypeUID);
            predicate = predicate.And(dlv => dlv.AssetUid == assetUid);
            var dlvQuery = _unitOfWork.DynListValueRepository
                .Where(predicate)
                .OrderBy(dlv => dlv.ParentListId)
                .AsQueryable();
            var dliQuery = _unitOfWork.DynListItemRepository.AsQueryable();
            var dlvs =
                from list in dlvQuery
                from item in dliQuery
                where item.DynListUid == list.DynListUid
                      && item.DynListItemId == list.DynListItemUid
                select new { ListValue = list, ListItem = item };
            foreach (var entity in dlvs.ToList())
            {
                var listValue = new DynamicListValue(entity.ListValue);
                if (entity.ListItem != null)
                {
                    listValue.Value = entity.ListItem.Value;
                    listValue.DynamicListItemId = entity.ListItem.DynListItemId;
                }
                yield return listValue;
            }
        }
               
        public DynamicList GetByAttributeId(long attributeId)
        {
            var attribute = _attributeRepository.GetPublishedById(attributeId);
            return GetByUid(attribute.DynListUid.GetValueOrDefault());
        }

        public DynListItem GetListItemById(long dynListItemId)
        {
            return _unitOfWork.DynListItemRepository.SingleOrDefault(
                i => i.DynListItemId == dynListItemId && i.ActiveVersion);
        }
    }
}
