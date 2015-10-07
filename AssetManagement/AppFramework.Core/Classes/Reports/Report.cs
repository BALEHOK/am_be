
using System.Net;
using AppFramework.Core.AC.Authentication;

namespace AppFramework.Core.Classes.Reports
{
    using AppFramework.Core.Classes;
    using AppFramework.Entities;
    using Common.Logging;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.IO;
    using System.Linq;

    [global::System.Serializable]
    public class ReportFileMissedException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ReportFileMissedException() { }
        public ReportFileMissedException(string message) : base(message) { }
        public ReportFileMissedException(string message, Exception inner) : base(message, inner) { }
        protected ReportFileMissedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Proxy class for manipulating reports
    /// </summary>
    public class Report
    {
        protected Entities.Report _base;
        protected List<ReportField> _fields;
        protected DataTable _finDataTable;
        private AssetType _assetType;
        private ILog _logger = LogManager.GetCurrentClassLogger();
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IAssetsService _assetsService;
        private readonly IAuthenticationService _authenticationService;
        public enum ReportType { Regular = 0, ViewBased = 1 };

        /// <summary>
        /// Initializes a new instance of the <see cref="Report"/> class.
        /// </summary>
        /// <param name="rep">The rep.</param>
        public Report(
            Entities.Report rep, 
            IAuthenticationService authenticationService,
            IAssetTypeRepository assetTypeRepository, 
            IAssetsService assetsService)
        {
            if (rep == null)
                throw new ArgumentNullException();
            _base = rep;
            if (assetTypeRepository == null)
                throw new ArgumentNullException("assetTypeRepository");
            _assetTypeRepository = assetTypeRepository;
            if (assetsService == null)
                throw new ArgumentNullException("assetsService");
            _assetsService = assetsService;
            if (authenticationService == null)
                throw new ArgumentNullException("authenticationService");
            _authenticationService = authenticationService;
            _base.StartTracking();
            _finDataTable = this.GetFinTable();
        }

        public Report(IAuthenticationService authenticationService, IAssetTypeRepository assetTypeRepository, IAssetsService assetsService)
            : this(new Entities.Report(), authenticationService, assetTypeRepository, assetsService)
        {
            Type = ReportType.Regular;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Report"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isFinancial">if set to <c>true</c> [is financial].</param>
        public Report(string name, bool isFinancial, long? assetTypeId, IAuthenticationService authenticationService, IAssetTypeRepository assetTypeRepository, IAssetsService assetsService)
            : this(new Entities.Report()
            {
                Name = name,
                IsFinancial = isFinancial,
                ReportFile = string.Empty,
                DynConfigId = assetTypeId,
            }, authenticationService, assetTypeRepository, assetsService)
        {

        }

        /// <summary>
        /// Gets the structure of table with financial info
        /// </summary>
        /// <returns></returns>
        private DataTable GetFinTable()
        {
            var t = new DataTable();
            t.Columns.Add(new DataColumn() { ColumnName = "AssetType" });
            t.Columns.Add(new DataColumn() { ColumnName = "AssetTypeId" });
            t.Columns.Add(new DataColumn() { ColumnName = "Asset" });
            t.Columns.Add(new DataColumn() { ColumnName = "AssetId" });
            t.Columns.Add(new DataColumn() { ColumnName = "Price" });
            t.Columns.Add(new DataColumn() { ColumnName = "Count" });

            return t;
        }

        /// <summary>
        /// Gets the UID.
        /// </summary>
        /// <value>The UID.</value>
        public long UID
        {
            get
            {
                return this._base.ReportUid;
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
                return this._base.Name;
            }
            set
            {
                this._base.Name = value;
            }
        }

        /// <summary>
        /// Gets or sets the asset type id.
        /// </summary>
        /// <value>The asset type id.</value>
        public long? AssetTypeId
        {
            get
            {
                return this._base.DynConfigId;
            }
            set
            {
                this._base.DynConfigId = value;
            }
        }

        /// <summary>
        /// Gets the type of the asset.
        /// </summary>
        /// <value>The type of the asset.</value>
        public AssetType AssetType
        {
            get
            {
                if (_assetType == null)
                {
                    if (this.AssetTypeId.HasValue && this.AssetTypeId.Value != 0)
                        _assetType = AssetType.GetByID(this.AssetTypeId.Value);
                    else // asset type not selected, return generic asset type with only predefined values
                    {
                        _assetType = _assetTypeRepository.GetGeneralAssetType();
                    }
                }
                return _assetType;
            }
        }

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <value>The fields.</value>
        public ReadOnlyCollection<ReportField> Fields
        {
            get
            {
                if (_fields == null)
                {
                    _fields = _base.ReportField.Select(r => 
                        new ReportField(r, _authenticationService, _assetTypeRepository, _assetsService)).ToList();
                }
                return _fields.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the filter fields.
        /// </summary>
        /// <value>The filter fields.</value>
        public IEnumerable<ReportField> FilterFields
        {
            get
            {
                return this.Fields.Where(f => f.IsFilter);
            }
        }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>The template.</value>
        public string Template
        {
            get
            {
                return this._base.ReportFile;
            }
            set
            {
                this._base.ReportFile = value;
            }
        }

        public ReportType Type
        {
            get
            {
                return (ReportType)_base.Type;
            }
            set
            {
                _base.Type = (int)value;
            }
            
        }

        /// <summary>
        /// Gets the template full path.
        /// </summary>
        /// <value>The template full path.</value>
        public string TemplateFullPath
        {
            get
            {
                if (System.Web.HttpContext.Current != null)
                {
                    string path = string.Format("{0}{1}",
                        System.Web.HttpContext.Current.Server.MapPath(ApplicationSettings.TemplatesPath),
                        this.Template);
                    return path;
                }
                else
                {
                    return this.Template;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this report include financial info.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is financial; otherwise, <c>false</c>.
        /// </value>
        public bool IsFinancial
        {
            get { return _base.IsFinancial; }
            set { _base.IsFinancial = true; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether should report include only currently open item in dataset or user all data found with given filter
        /// </summary>
        /// <value>
        ///   <c>true</c> if include only currently open item into report; otherwise, <c>false</c>.
        /// </value>
        public bool SyncWithItem
        {
            get
            {
                return _base.SyncWithOpenItem;
            }
            set
            {
                _base.SyncWithOpenItem = value;
            }
        }

        /// <summary>
        /// Gets all reports
        /// </summary>
        /// <returns></returns>
        [Obsolete("Use IReportService instead")]
        public static IEnumerable<Report> GetAll()
        {
            throw new NotImplementedException();
            //var unitOfWork = new DataProxy.UnitOfWork();
            //var reports = unitOfWork.ReportRepository.Get();
            //return reports.Where(r => !string.IsNullOrEmpty(r.ReportFile)).Select(r => new Report(r));
        }

        /// <summary>
        /// Gets the report by uid.
        /// </summary>
        /// <param name="uid">The uid.</param>
        /// <returns></returns>
        [Obsolete("Use IReportService instead")]
        public static Report GetByUid(long uid)
        {
            throw new NotImplementedException();
            //var unitOfWork = new DataProxy.UnitOfWork();
            //return new Report(unitOfWork.ReportRepository.Single(r => r.ReportUid == uid, include: r => r.ReportField));
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        [Obsolete("Use IReportService instead")]
        public void Save()
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            if (_base.ReportUid > 0)
            {
                unitOfWork.ReportRepository.Update(_base);
            }
            else
            {
                unitOfWork.ReportRepository.Insert(_base);
            }
            unitOfWork.Commit();
        }

        /// <summary>
        /// Adds the field.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
        /// <param name="isFilter">if set to <c>true</c> [is filter].</param>
        public void AddField(string name, bool isVisible, bool isFilter)
        {
            _base.ReportField.Add(new Entities.ReportField
            {
                ReportUid = 0,
                Name = name,
                IsVisible = isVisible,
                IsFilter = isFilter
            });
        }

        protected object GetAttributeValue(Asset asset, string fieldName)
        {
            object value = null;
            if (fieldName.Contains("."))
            {
                var attrName = GetFullAttributeName(fieldName);
                var linked = GetLinkedAsset(asset, attrName.Item1);
                if (linked != null)
                {
                    value = GetValue(linked, attrName.Item2);
                }
            }
            else
            {
                value = GetValue(asset, fieldName);
            }

            return value;
        }

        private static Asset GetLinkedAsset(Asset asset, string attributeName)
        {
            var attr = asset.Attributes.FirstOrDefault(a => a.GetConfiguration().Name == attributeName);
            if (attr != null && attr.GetConfiguration().IsAsset)
            {
                return attr.RelatedAsset;
            }
            else
            {
                return null;
            }
        }

        private static Tuple<string, string> GetFullAttributeName(string fieldName)
        {
            var components = fieldName.Split('.');
            if (components.Length != 2)
            {
                throw new InvalidOperationException(string.Format("Invalid path '{0}'", fieldName));
            }
            else
            {
                return new Tuple<string, string>(components[0], components[1]);
            }
        }

        protected object GetValue(Asset asset, string fieldName)
        {
            var attr = asset.Attributes.FirstOrDefault(a => a.GetConfiguration().Name == fieldName);
            if (attr != null)
            {
                return attr.Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the financial data.
        /// </summary>
        /// <returns></returns>
        public virtual DataTable GetFinancialData()
        {
            return this._finDataTable;
        }

        /// <summary>
        /// Sets the filter value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void SetFilterValue(string name, string value, string text)
        {
            if (_fields != null)
            {
                ReportField fld = _fields.SingleOrDefault(f => f.Name == name);
                if (fld != null)
                {
                    fld.FilterValue = value;
                    fld.FilterText = text;
                }
            }
        }

        /// <summary>
        /// Deletes this instance from database
        /// </summary>
        public void Delete()
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            unitOfWork.ReportRepository.Delete(_base);
            unitOfWork.Commit();
        }


        public long? AssetUid { get; set; }

        public static object GetReportsForAssetType(long assetTypeId)
        {
            return Report.GetAll().Where(r => r.AssetTypeId == assetTypeId);
        }

    	public IEnumerable<IIndexEntity> EntityIndicesFromSearch
    	{
    		get;set;
    	}
    }
}