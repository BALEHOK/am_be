using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using AppFramework.Core.DataTypes;
using AppFramework.DataProxy;
using AppFramework.Entities;

namespace AppFramework.Core.Classes
{
    //public abstract class TypeAttribute<T> : AssetTypeAttribute where T : DataTypeBase
    //{
    //}

    public class TypeOfAssetAttribute : AssetTypeAttribute
    {
        private readonly IAssetTypeRepository _assetTypeRepository;

        public TypeOfAssetAttribute()
        {
            
        }

        public TypeOfAssetAttribute(
            IAssetTypeRepository assetTypeRepository,
            IUnitOfWork unitOfWork,
            DynEntityAttribConfig data,
            AssetType parentAssetType)
            : base(data, unitOfWork, parentAssetType)
        {
            if (assetTypeRepository == null)
                throw new ArgumentNullException();
            _assetTypeRepository = assetTypeRepository;
        }

        public AssetType GetRelatedAssetType()
        {
            var result = _assetTypeRepository.GetById(RelatedAssetTypeID.Value);
            return result;
        }
    }
}
