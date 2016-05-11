using AppFramework.Core.Classes.Extensions;
using AppFramework.DataProxy;

namespace AppFramework.Core.Classes
{
    using AppFramework.Core.AC.Authentication;
    using AppFramework.Entities;
    using System;

    [Serializable]
    public class EntityContext
    {
        private Context _base;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// Gets the base DAL entity
        /// </summary>
        public Context Base
        {
            get
            {
                return _base;
            }
        }


        /// <summary>
        /// DB OriginalName of context
        /// </summary>
        public string OriginalName
        {
            get
            {
                return _base.Name;
            }
        }

        /// <summary>
        /// DB ID of context
        /// </summary>
        public long ID
        {
            get { return _base.ContextId; }
        }

        /// <summary>
        /// Name of entity context type
        /// </summary>
        public string Name
        {
            get { return _base.Name.Localized(); }
            set
            {
                string name = value;
                if (name.Length > 60)
                {
                    name = name.Substring(0, 60);
                }
                _base.Name = name;
            }
        }

        /// <summary>
        /// Data type of context
        /// </summary>    
        public long DataTypeUid
        {
            get { return _base.DataTypeUid; }
            set { _base.DataTypeUid = value; }
        }

        /// <summary>
        /// Gets and sets if current context is active and available in system.
        /// </summary>
        public bool IsActive
        {
            get { return _base.IsActive; }
            set { _base.IsActive = value; }
        }

        /// <summary>
        /// Class constructor with properties initialization by provided data
        /// </summary>
        /// <param name="data">Context table record</param>
        public EntityContext(Context data, IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            _base = data;
            _base.StartTracking();
        }

        /// <summary>
        /// Deletes the context entity by its ID
        /// </summary>
        /// <param name="id"></param>
        [Obsolete]
        public static void Delete(long id)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            var ctx = unitOfWork.ContextRepository.Single(c => c.ContextId == id);
            ctx.IsActive = false;
            unitOfWork.ContextRepository.Update(ctx);
            unitOfWork.Commit();
        }

        /// <summary>
        /// Inserts the entity into database, if it's new one
        /// or updates exisiting
        /// </summary>
        /// <param name="item">EntityContext item</param>
        [Obsolete]
        public void Save(long currentUserId)
        {
            Base.UpdateDate = DateTime.Now;
            Base.UpdateUserId = currentUserId;
            if (Base.ContextId > 0)
                _unitOfWork.ContextRepository.Update(Base);
            else
                _unitOfWork.ContextRepository.Insert(Base);
            _unitOfWork.Commit();
        }
    }
}
