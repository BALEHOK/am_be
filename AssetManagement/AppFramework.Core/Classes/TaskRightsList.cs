using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Classes
{
    /// <summary>
    /// Holds all the information about task rights 
    /// for current user
    /// </summary>
    public class TaskRightsList
    {
        /// <summary>
        /// Gets and sets the list of rights
        /// </summary>
        public List<TaskRightsEntry> Items { get; set; }

        /// <summary>
        /// Class constructor with 
        /// </summary> 
        public TaskRightsList()
        {
            Items = new List<TaskRightsEntry>();
        }

        /// <summary>
        /// Returns the list of permissions 
        /// by given ViewID
        /// </summary>
        /// <param name="viewID"></param>
        public static TaskRightsList GetByViewID(long viewID)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            var rights = unitOfWork.TaskRightRepository.Get(r => r.ViewId == viewID);
            var rList = new TaskRightsList();
            foreach (var entry in rights)
            {
                rList.Items.Add(new TaskRightsEntry(entry));
            }
            return rList;
        }

        /// <summary>
        /// Assigns the provided list of permissions 
        /// to the given user.
        /// </summary>
        /// <param name="list">TaskRightsList collection</param>
        /// <param name="userId">ID of user</param>
        public static void SetToUserByUserID(TaskRightsList list, long userId, long viewId = 0)
        {
            var data = (from entry in list.Items
                        select new AppFramework.Entities.TaskRights()
                         {
                             TaskRightsId = entry.TaskRightsId,
                             TaxonomyItemId = entry.TaxonomyItemId,
                             DynEntityConfigId = entry.DynEntityConfigId,
                             UserId = userId,
                             ViewId = viewId,
                             IsDeny = entry.IsDeny
                         }).ToList();

            var unitOfWork = new DataProxy.UnitOfWork();
            if (viewId > 0)
            {
                var oldEntities = unitOfWork.TaskRightRepository.Get(r => r.ViewId == viewId).ToList();
                oldEntities.ForEach(e => unitOfWork.TaskRightRepository.Delete(e));
                foreach (var taskrights in data)
                {
                    taskrights.ViewId = viewId;
                    unitOfWork.TaskRightRepository.Insert(taskrights);
                }
                unitOfWork.Commit();
            }
            else
            {
                long newViewId;
                long.TryParse(
                    unitOfWork.SqlProvider.ExecuteScalar("SELECT MAX(ViewId) AS Expr1 FROM TaskRights").ToString(),
                    out newViewId);
                newViewId += 1;
                data.ForEach(i => i.ViewId = newViewId);
                foreach (var item in data)
                    unitOfWork.TaskRightRepository.Insert(item);
                unitOfWork.Commit();
            }
        }

        /// <summary>
        /// Deletes the list of entries by View ID
        /// </summary>
        /// <param name="viewId"></param>
        public static void DeleteByViewID(long viewId)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            var entities = unitOfWork.TaskRightRepository.Where(r => r.ViewId == viewId);
            unitOfWork.TaskRightRepository.Delete(entities);
            unitOfWork.Commit();
        }


    }
}
