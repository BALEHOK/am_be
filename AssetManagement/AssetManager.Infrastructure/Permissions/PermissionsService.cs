using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.DataProxy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetManager.Infrastructure.Permissions
{
    public class PermissionsService : IPermissionsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PermissionsService(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        public List<RightsEntry> GetUserRights(long userId)
        {
            return _unitOfWork
                .RightsRepository.Get(r => r.UserId == userId)
                .Select(x => new RightsEntry(x))
                .ToList();
        }
    }
}
