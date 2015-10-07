using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.ConstantsEnumerators;
using AppFramework.Core.ConstantsEnumerators;
using AppFramework.DataProxy;

namespace AppFramework.Core.Classes.SearchEngine.TypeSearchElements
{
    /// <summary>
    /// Киль ми, плиз! Э бит лэйта!
    /// </summary>
    [Serializable]
    public class OneType
    {
        public List<AssetAttribute> AssetAttributes
        {
            get
            {
                if (object.Equals(null, _assetAttributes))
                {
                    LoadAttributres();
                }
                return _assetAttributes;
            }
        }

        public List<Attribute> Attributes
        {
            get
            {
                if (object.Equals(null, _attributes))
                {
                    LoadAttributres();
                }
                return _attributes;
            }
        }

        private List<Attribute> _attributes;
        private List<AssetAttribute> _assetAttributes;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetsService _assetsService;
        private readonly AssetType _at;
        private readonly bool _returnOnlyEditableAttributes;
        private readonly IAssetTypeRepository _assetTypeRepository;
     
        public OneType(AssetType assetType, 
            IUnitOfWork unitOfWork, 
            IAssetsService assetsService,
            IAssetTypeRepository assetTypeRepository,
            bool returnOnlyEditableAttributes = false)
        {
            if (assetType == null)
                throw new ArgumentNullException();
            if (unitOfWork == null)
                throw new ArgumentNullException();
            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            _assetsService = assetsService;
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            
            _returnOnlyEditableAttributes = returnOnlyEditableAttributes;
            _at = assetType;
            _unitOfWork = unitOfWork;
            LoadAttributres();
        }

        private void LoadAttributres()
        {
            _attributes = new List<Attribute>();
            var newAsset = _assetsService.CreateAsset(_at);
            if (_returnOnlyEditableAttributes)
            {
                _assetAttributes = newAsset.Attributes.Where(
                    a => 
                    (a.Configuration.IsShownOnPanel && 
                     a.Configuration.Editable &&
                     a.Configuration.Name != AttributeNames.Name &&
                     a.Configuration.Name != AttributeNames.Barcode
                    ) 
                    ||
                    (
                    a.Configuration.Name == AttributeNames.Name &&
                    a.Configuration.Parent.AutoGenerateNameType != Enumerators.TypeAutoGenerateName.InsertUpdate
                    ))
                    .
                    OrderBy(a => a.Configuration.DisplayOrder).ToList();

                _attributes.AddRange(_at.Attributes.Where(
                    g => 
                    (g.IsShownOnPanel && 
                    g.Editable &&
                    g.Name != AttributeNames.Name &&
                    g.Name != AttributeNames.Barcode
                    )
                    ||
                    (
                    g.Name == AttributeNames.Name &&
                    g.Parent.AutoGenerateNameType != Enumerators.TypeAutoGenerateName.InsertUpdate
                    ))
                    .OrderBy(a => a.DisplayOrder).Select(attribute => new Attribute(_unitOfWork, 
                        _assetTypeRepository)
                {
                    AttributeValue = attribute.UID,
                    AttributeText = new TranslatableString(attribute.Name).GetTranslation()
                }));
            }
            else
            {
                _assetAttributes = newAsset.Attributes.Where(a => a.Configuration.IsShownOnPanel).
                    OrderBy(a => a.Configuration.DisplayOrder).ToList();
                _attributes.AddRange(_at.Attributes.Where(g => g.IsShownOnPanel).OrderBy(a => a.DisplayOrder)
                    .Select(attribute => new Attribute(_unitOfWork, _assetTypeRepository)
                {
                    AttributeValue = attribute.UID,
                    AttributeText = new TranslatableString(attribute.Name).GetTranslation()
                }));
            }
        }
    }
}
