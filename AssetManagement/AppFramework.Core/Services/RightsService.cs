using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.DataProxy;
using AppFramework.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFramework.Core.Services
{
    public class RightsService : IRightsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RightsService(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// Assigns the provided list of permissions 
        /// to the given user.
        /// </summary>
        /// <param name="list">RightsList collection</param>
        /// <param name="userId">ID of user</param>
        public void SetPermissionsForUser(IEnumerable<RightsEntry> list, long userId, long currentUserId, long viewId = 0)
        {
            var data = (from entry in list
                        select new Rights
                        {
                            RightsUid = entry.RightsUid,
                            DynEntityConfigId = entry.AssetTypeID,
                            CategoryId = entry.TaxonomyItemId,
                            DepartmentId = entry.DepartmentID,
                            Rights1 = entry.Permission.GetCode(),
                            IsDeny = entry.IsDeny,
                            ViewId = viewId,
                            UserId = userId,
                            UpdateUserId = currentUserId,
                            UpdateDate = DateTime.Now
                        }).ToList();

            // it's a rules updating
            if (viewId > 0)
            {
                // perform the data updating with transaction
                var oldEntities = _unitOfWork.RightsRepository.Get(r => r.ViewId == viewId).ToList();
                // delete from DB all entries with this ViewID
                oldEntities.ForEach(e => _unitOfWork.RightsRepository.Delete(e));
                // insets or update entities
                foreach (var rights in data)
                {
                    // set the same unique ViewId for all entities
                    rights.ViewId = viewId;
                    _unitOfWork.RightsRepository.Insert(rights);
                }
                _unitOfWork.Commit();
            }
            // it's a new rules set creation
            else
            {
                var newViewId =
                    Int64.Parse(
                        _unitOfWork.SqlProvider.ExecuteScalar("SELECT MAX(ViewId) AS Expr1 FROM Rights").ToString()) + 1;
                data.ForEach(i => i.ViewId = newViewId);
                foreach (var item in data)
                    _unitOfWork.RightsRepository.Insert(item);
                _unitOfWork.Commit();
            }
        }
    }
}
