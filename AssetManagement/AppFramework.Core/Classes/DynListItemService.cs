using System;
using AppFramework.Core.Classes.DynLists;
using AppFramework.DataProxy;
using AppFramework.Entities;

namespace AppFramework.Core.Classes
{
    public interface IDynListItemService
    {
        DynamicListItem GetByUid(long uid);
        void Delete(DynListItem item);
        void Update(DynListItem item);
    }

    public class DynListItemService : IDynListItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DynListItemService(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        public DynamicListItem GetByUid(long uid)
        {
            return new DynamicListItem(_unitOfWork.DynListItemRepository.Single(dl => dl.DynListItemUid == uid,
                include: dli => dli.AssociatedDynList));
        }

        public void Delete(DynListItem item)
        {
            _unitOfWork.DynListItemRepository.Delete(item);
            _unitOfWork.Commit();
        }

        public void Update(DynListItem item)
        {
            _unitOfWork.DynListItemRepository.Update(item);
            _unitOfWork.Commit();
        }
    }
}
