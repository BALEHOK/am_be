namespace AppFramework.Core.Classes.Tasks.Runners
{
    using DataProxy;
    using ConstantsEnumerators;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using AppFramework.Tasks;
    using AppFramework.Tasks.Models;

    class AssetCreationRunner : ITaskRunner
    {
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public long? ScreenId { get; set; }
        public long DynEntityConfigId { get; set; }

        public AssetCreationRunner(
            IAssetTypeRepository assetTypeRepository,
            IUnitOfWork unitOfWork)
        {
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }
       
        public TaskResult Run(Entities.Task task)
        {
            if (DynEntityConfigId == 0)
                throw new ArgumentException("DynEntityConfigId is not set");

            var resultArguments = new Dictionary<string, object>(2)
            {
                { "DynEntityConfigId", DynEntityConfigId }
            };

            if (ScreenId.HasValue)
            {
                // ScreenId connected to the type via [DynEntityConfigUid], so it may refer to and old revision
                // Batch will create a new ScreeenId record, let's check if we could find newest one
                var assetType = _assetTypeRepository.GetById(DynEntityConfigId);
                var screensSource = _unitOfWork.AssetTypeScreenRepository.AsQueryable();
                var latestScreen = (from screen in screensSource
                                    from referringScreen in screensSource
                                    where screen.DynEntityConfigUid == assetType.UID &&
                                       screen.ScreenUid == referringScreen.ScreenUid &&
                                       referringScreen.ScreenId == ScreenId.Value
                                    select screen).FirstOrDefault();

                if (latestScreen != null)
                    ScreenId = latestScreen.ScreenId;

                resultArguments.Add("ScreenId", ScreenId);
            }
            
            return new TaskResult((Enumerations.TaskFunctionType)task.FunctionType)
            {
                NavigationResult = string.Format(QueryStrings.CreateAssetUrlFormat, DynEntityConfigId) +
                    (ScreenId.HasValue ? "&ScreenId=" + ScreenId : string.Empty),
                NavigationResultArguments = resultArguments,                    
            };
        }
    }
}
