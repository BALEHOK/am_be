using AppFramework.Core.AC.Authentication;

namespace AppFramework.Core.Classes.Reports
{

    using System;
    using System.Web.UI;

    public class ReportField
    {
        private readonly Entities.ReportField _base;
        private AssetTypeAttribute assetTypeAttribute;
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IAssetsService _assetsService;
        private readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportField"/> class.
        /// </summary>
        /// <param name="field">The field from database</param>
        public ReportField(
            Entities.ReportField field, 
            IAuthenticationService authenticationService,
            IAssetTypeRepository assetTypeRepository,
            IAssetsService assetsService)
        {
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            _assetsService = assetsService;
            if (authenticationService == null)
                throw new ArgumentNullException("authenticationService");
            _authenticationService = authenticationService;
            
            this._base = field;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is null.
        /// </summary>
        /// <value><c>true</c> if this instance is null; otherwise, <c>false</c>.</value>
        public bool IsNull
        {
            get
            {
                return this._base == null;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return _base.Name;
            }
            set
            {
                _base.Name = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is filter.
        /// </summary>
        /// <value><c>true</c> if this instance is filter; otherwise, <c>false</c>.</value>
        public bool IsFilter
        {
            get
            {
                return _base.IsFilter;
            }
            set
            {
                _base.IsFilter = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsVisible
        {
            get
            {
                return _base.IsVisible;
            }
            set
            {
                _base.IsVisible = value;
            }
        }

        /// <summary>
        /// Gets the parent report
        /// </summary>
        /// <value>The report.</value>
        public Report Report
        {
            get
            {
                return Report.GetByUid(this._base.ReportUid);
            }
        }

        /// <summary>
        /// Gets or sets the filter value.
        /// </summary>
        /// <value>The filter value.</value>
        public string FilterValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the filter text - label for displaying in reports and other user layouts.
        /// </summary>
        /// <value>The filter text.</value>
        public string FilterText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the filter control.
        /// </summary>
        /// <value>The filter control.</value>
        public Control FilterControl
        {
            get
            {
                Control c = null;
                var att = this.GetTypeAttribute();
                if (att.IsAsset)
                    c = new DropDownFilterControl(this, _authenticationService,  _assetTypeRepository, _assetsService);
                else
                {
                    switch (att.DataTypeEnum)
                    {
                        case AppFramework.ConstantsEnumerators.Enumerators.DataType.DynList:
                        case AppFramework.ConstantsEnumerators.Enumerators.DataType.DynLists:
                            c = new DynListFilterControl(this);
                            break;
                        default:
                            c = new TextFilterControl();
                            break;
                    }
                }

                return c;
            }
        }

        /// <summary>
        /// Gets the TTX line - line for generating Field Definition file
        /// </summary>
        /// <returns></returns>
        public string GetTtxLine()
        {
            string res = string.Empty;
            string dt;
            switch (this.GetTypeAttribute().DataTypeEnum)
            {
                case AppFramework.ConstantsEnumerators.Enumerators.DataType.Int:
                case AppFramework.ConstantsEnumerators.Enumerators.DataType.Long:
                case AppFramework.ConstantsEnumerators.Enumerators.DataType.Revision:
                case AppFramework.ConstantsEnumerators.Enumerators.DataType.Float:
                case AppFramework.ConstantsEnumerators.Enumerators.DataType.Money:
                case AppFramework.ConstantsEnumerators.Enumerators.DataType.USD:
                case AppFramework.ConstantsEnumerators.Enumerators.DataType.Euro:
                    dt = "Number\t";
                    break;
                case AppFramework.ConstantsEnumerators.Enumerators.DataType.DateTime:
                case AppFramework.ConstantsEnumerators.Enumerators.DataType.CurrentDate:
                    dt = "Datetime\t";
                    break;
                case AppFramework.ConstantsEnumerators.Enumerators.DataType.DynList:
                case AppFramework.ConstantsEnumerators.Enumerators.DataType.DynLists:
                    dt = "Memo\t";
                    break;
                default:
                    dt = "String\t250";
                    break;
            }

            res = string.Format("{0}\t{1}{2}",
                this._base.Name,
                dt,
                System.Environment.NewLine);

            return res;
        }

        /// <summary>
        /// Gets the type attribute.
        /// </summary>
        /// <returns></returns>
        public AssetTypeAttribute GetTypeAttribute()
        {
            if (assetTypeAttribute == null)
            {
                AssetType assetType = this.Report.AssetType;

                if (!this.Name.Contains("."))
                { 
                    assetTypeAttribute = assetType[this.Name];
                    if (assetTypeAttribute == null)
                        throw new Exception(string.Format("{0} not found in attributes", this.Name));
                }
                else
                {
                    var attrPath = this.Name.Split('.');
                    string attrName = attrPath[attrPath.Length - 1];
                    string owningAttrName = attrPath[0];
                
                    var relatedAt = _assetTypeRepository.GetById(assetType[owningAttrName].RelatedAssetTypeID.Value);
                    assetTypeAttribute = relatedAt[attrName];
                }
            }

            return assetTypeAttribute;
        }
    }
}