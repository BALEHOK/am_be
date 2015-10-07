using System;
using System.Linq;
using System.Runtime.InteropServices;
using AppFramework.DataProxy;
using AppFramework.Entities;

namespace AppFramework.Core.Classes
{
    public interface IDataFactory
    {
        T Get<T>(long id, long? typeId = null);
    }

    public class DataFactory : IDataFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetsService _assetsService;
        private readonly IAssetTypeRepository _typeRepository;
        private readonly IAttributeValueFormatter _attributeValueFormatter;
        private readonly IDynamicListsService _dynamicListsService;

        public DataFactory(
            IUnitOfWork unitOfWork, 
            IAssetsService assetsService, 
            IAssetTypeRepository typeRepository,
            IAttributeValueFormatter attributeValueFormatter,
            IDynamicListsService dynamicListsService)
        {
            _unitOfWork = unitOfWork;
            _assetsService = assetsService;
            _typeRepository = typeRepository;
            _attributeValueFormatter = attributeValueFormatter;
            _dynamicListsService = dynamicListsService;
        }

        public T Get<T>(long id, long? typeId = null)
        {
            dynamic result = default(T);

            if (typeof(T) == typeof(AssetTypeAttribute))
            {
                var attribConfig = GetActiveAttributeConfig(id);
                var assetType = new AssetType(attribConfig.DynEntityConfig, _unitOfWork);
                result = new AssetTypeAttribute(attribConfig, _unitOfWork, assetType);
            }
            else if (typeof(T) == typeof(AssetAttribute))
            {
                var attributeType = Get<AssetTypeAttribute>(id);
                var attribute = new AssetAttribute(
                    attributeType, null, null, _attributeValueFormatter, _typeRepository, _assetsService, _unitOfWork, _dynamicListsService);
                attribute.InitData();

                result = attribute;
            }
            else if (typeof (T) == typeof (AssetType))
            {
                result = _typeRepository.GetById(id);
            }
            else if (typeof (T) == typeof (Asset))
            {
                if (typeId == null)
                    throw new ArgumentException(@"Asset type Id should be provided to get an Asset", "typeId");

                var assetType = Get<AssetType>((long)typeId);
                result = _assetsService.GetAssetById(id, assetType);                
            }

            return result;
        }

        private DynEntityAttribConfig GetActiveAttributeConfig(long id)
        {
            var attributeConfig =
                _unitOfWork.DynEntityAttribConfigRepository.Get(
                    a => a.ActiveVersion && a.DynEntityAttribConfigId == id, include: config => config.DynEntityConfig)
                    .Single();
            return attributeConfig;
        }
    }
}