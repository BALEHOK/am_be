﻿using System;
using System.Collections.Generic;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Validation;

namespace AppFramework.Core.Classes
{
    public interface IAssetTypeRepository
    {
        AssetType GetById(long id, bool activeOnly = true);
        AssetType GetByUid(long uid, bool activeOnly = true);
        void Save(AssetType assetType, long currentUserId, List<TaxonomyContainer> containers = null);
        AssetType GetGeneralAssetType();
        Dictionary<long, AssetType> GetTypeRevisions(long typeId);
        IEnumerable<AssetType> GetAllPublished();
        IEnumerable<AssetType> GetRecent();
        AssetType FindByName(string assetTypeName);

        [Obsolete("Use methods of AttributeRepository")] 
        AssetTypeAttribute GetAttributeByUid(long uid);
        AssetType GetPredefinedAssetType(PredefinedEntity entity);
        AssetType GetDraftById(long assetTypeId, int currentRevision);
        void Delete(AssetType assetType);
        ValidationResult ValidateNameUniqueness(string assetTypeName, long assetTypeId);
        bool TryGetById(long id, out AssetType assetType, bool activeOnly = true);
        bool IsPredefinedAssetType(AssetType typeToCheck, PredefinedEntity entity);
    }
}