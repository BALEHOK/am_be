using System;
using AppFramework.Core.AC.Authentication;
using AppFramework.DataProxy;

namespace AppFramework.Core.Classes.SearchEngine.TypeSearchElements
{
    using System.Collections.Generic;
    using System.Linq;
    using AppFramework.ConstantsEnumerators;

    /// <summary>
    /// Киль ми, плиз!
    /// </summary>
    public class Attribute
    {
        public AssetTypeAttribute Configuration
        {
            get
            {
                if (_configuration == null)
                    LoadConfiguration();
                return _configuration;
            }
        }

        public List<Operator> Operators
        {
            get
            {
                if (_operators == null)
                    LoadOperators();
                return _operators;
            }
        }
        
        private bool IsDynListDropdown
        {
            get
            {
                return _configuration.DataTypeEnum == Enumerators.DataType.DynList
                       || _configuration.DataTypeEnum == Enumerators.DataType.DynLists;
            }
        }

        private bool IsAssetDropdown
        {
            get
            {
                return _configuration.DataTypeEnum == Enumerators.DataType.Asset
                       || _configuration.DataTypeEnum == Enumerators.DataType.Assets;
            }
        }

        public long AttributeValue { get; set; }
        public string AttributeText { get; set; }
        public string FieldText { get; set; }
        public string FieldSql { get; set; }
        public Enumerators.DataType Type { get; set; }

        private AssetTypeAttribute _configuration;
        private List<Operator> _operators = null;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAssetTypeRepository _assetTypeRepository;

        public Attribute(IUnitOfWork unitOfWork, IAssetTypeRepository assetTypeRepository)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
        }

        private void LoadConfiguration()
        {
            _configuration = _assetTypeRepository.GetAttributeByUid(AttributeValue);
        }

        private void LoadOperators()
        {
            if (_configuration == null)
                LoadConfiguration();

            AttributeValue = _configuration.UID;        
            FieldText = _configuration.DBTableFieldName;
            Type = _configuration.DataTypeEnum;

            if (_configuration.DataTypeEnum == Enumerators.DataType.Assets)
            {
                var relatedAT = _assetTypeRepository.GetById(_configuration.RelatedAssetTypeID.Value);
                FieldSql = string.Format("[{0}].[{1}]", relatedAT.DBTableName,
                    relatedAT.Attributes.Single(attr => attr.ID == _configuration.RelatedAssetTypeAttributeID).DBTableFieldName);
            }
            else if (_configuration.DataTypeEnum == Enumerators.DataType.DynList || _configuration.DataTypeEnum == Enumerators.DataType.DynLists)
            {
                FieldSql = "[DynListValue].[DynListItemUid]";
            }
            else
            {
                FieldSql = FieldText;
            }

            var dataTypeUid = _configuration.DataType.Base.DataTypeUid;
            var datatype = _unitOfWork.DataTypeRepository.Single(d => d.DataTypeUid == dataTypeUid,
                include: dt => dt.SearchOperators);
            _operators = (from so in datatype.SearchOperators
                          select new Operator(so)
                          {
                              Items = GetItems(),
                              IsAssetListDropDown = IsAssetDropdown,
                              IsDynListDropDown = IsDynListDropdown
                          }).ToList();
        }

        /// <summary>
        /// Returns the collection of items for dropwown control
        /// </summary>
        /// <returns></returns>
        private List<KeyValuePair<string, long>> GetItems()
        {
            var items = new List<KeyValuePair<string, long>>();
            if (IsDynListDropdown && _configuration.DynamicList != null)
                items.AddRange(
                    _configuration.DynamicList.Items.Select(i => new KeyValuePair<string, long>(i.Value, i.UID))
                                  .ToList());
            return items;
        }
    }
}
