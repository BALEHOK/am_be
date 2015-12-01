using AppFramework.Core.DAL.Adapters;
using AppFramework.Core.Validation;
using Microsoft.Practices.EnterpriseLibrary.Caching;

namespace AppFramework.Core.Classes
{
	using AppFramework.ConstantsEnumerators;
	using AppFramework.Core.AC.Authentication;
	using AppFramework.Core.Classes.Caching;
    using AppFramework.Core.Classes.Validation;
	using AppFramework.Core.DAL;
    using AppFramework.Core.Interfaces;
	using AppFramework.DataProxy;
	using AppFramework.Entities;
	using LinqKit;
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Collections.Specialized;
	using System.Linq;
	using System.Xml.Serialization;

	[Serializable()]
	public class AssetType : ISerializable
	{
		/// <summary>
		/// Gets DB unique ID of DynEntityConfig record, which represents current class
		/// </summary>
		[XmlElement]
		public long UID
		{
			get { return _base.DynEntityConfigUid; }
			set { _base.DynEntityConfigUid = value; }
		}

		public DateTime UpdateDate
		{
			get { return _base.UpdateDate; }
		}

		/// <summary>
		/// Gets AssetType ID
		/// </summary>
		[XmlElement]
		public long ID
		{
			get { return _base.DynEntityConfigId; }
			set { _base.DynEntityConfigId = value; }
		}

		/// <summary>
		/// Gets and sets name of asset type. Can have in different translations.
		/// Name can be used in context of String and TranslatableString as well 
		/// (cast will be performed automatically).
		/// You also can set a new name by assigning a normal string.
		/// </summary>       
		[XmlElement]
		public string Name
		{
			get
			{
				return new TranslatableString(_base.Name).GetTranslation();
			}
			set
			{
				_base.Name = value;

				if (this.ID == 0)
					DBTableName = value;
			}
		}

		[XmlIgnore]
		public string NameInvariant
		{
			get
			{
				return _base.Name;
			}
		}

		/// <summary>
		/// Gets and sets asset type description
		/// </summary>       
		[XmlElement]
		public string Comment
		{
			get { return _base.Comment; }
			set { _base.Comment = value; }
		}

		/// <summary>
		/// Gets and sets type of this entity: abstract, concrete, temporary etc.
		/// </summary>
		[XmlElement]
		public int TypeId
		{
			get { return _base.TypeId; }
			set { _base.TypeId = value; }
		}

		/// <summary>
		/// Gets and sets base type id, if it is inherited asset type
		/// </summary>
		[XmlElement]
		public long? BaseAssetTypeId
		{
			get { return _base.BaseDynEntityConfigId; }
			set { _base.BaseDynEntityConfigId = value; }
		}

		/// <summary>
		/// Gets and sets AssetType context
		/// </summary>
		[XmlElement]
		public long? ContextId
		{
			get { return _base.ContextId; }
			set
			{
				_base.ContextId = value > 0 ? value : null;
			}
		}

		/// <summary>
		/// Gets and sets availability of this asset in system
		/// </summary>
		[XmlElement]
		public bool IsActive
		{
			get { return _base.Active; }
			set { _base.Active = value; }
		}

		/// <summary>
		/// Is this version active version, or it's a history item
		/// </summary>
		[XmlElement]
		public bool IsActiveVersion
		{
			get { return _base.ActiveVersion; }
			set { _base.ActiveVersion = value; }
		}

		/// <summary>
		/// Is this asset available for searching
		/// </summary>
		[XmlElement]
		public bool IsSearchable
		{
			get { return _base.IsSearchable; }
			set { _base.IsSearchable = value; }
		}

		/// <summary>
		/// Should be information about this asset type putted into index table
		/// </summary>
		[XmlElement]
		public bool IsIndexed
		{
			get { return _base.IsIndexed; }
			set { _base.IsIndexed = value; }
		}

		[XmlElement]
		public bool IsContextIndexed
		{
			get { return _base.IsContextIndexed; }
			set { _base.IsContextIndexed = value; }
		}

		/// <summary>
		/// Gets and sets screen layout for this asset type
		/// </summary>
		[XmlElement]
		public int LayoutId
		{
			get { return _base.ScreenLayoutId; }
			set { _base.ScreenLayoutId = value; }
		}

		/// <summary>
		/// Gets the revision number
		/// </summary>
		[XmlElement]
		public int Revision
		{
			get { return _base.Revision; }
			set { _base.Revision = value; }
		}

		/// <summary>
		/// Gets table name of entity in database. 
		/// </summary>
		[XmlElement]
		public string DBTableName
		{
			get
			{
				return _base.DBTableName;
			}
			set
			{
				_base.DBTableName = Constants.DynTablePrefix
						+ Routines.SanitizeDBObjectName(value);
			}
		}

		/// <summary>
		/// Gets or sets the measure unit.
		/// </summary>
		/// <value>The measure unit.</value>
		[XmlElement]
		public long? MeasureUnitId
		{
			get { return _base.MeasureUnitId; }
			set { _base.MeasureUnitId = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is in stock.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is in stock; otherwise, <c>false</c>.
		/// </value>
		[XmlElement]
		public bool IsInStock
		{
			get { return _base.IsInStock; }
			set { _base.IsInStock = value; }
		}

		[XmlElement]
		public bool ParentChildRelations
		{
			get { return _base.ParentChildRelations; }
			set { _base.ParentChildRelations = value; }
		}

		[XmlElement]
		public bool AllowBorrow
		{
			get { return _base.AllowBorrow; }
			set { _base.AllowBorrow = value; }
		}

		[XmlElement]
		public bool IsUnpublished
		{
			get { return _base.IsUnpublished; }
			set { _base.IsUnpublished = value; }
		}

		[XmlIgnore]
		public int AutoGenerateName
		{
			get { return _base.AutoGenerateName; }
			set { _base.AutoGenerateName = value; }
		}

		[XmlIgnore]
		public Enumerators.TypeAutoGenerateName AutoGenerateNameType
		{
			get { return (Enumerators.TypeAutoGenerateName)_base.AutoGenerateName; }
		}

	    /// <summary>
	    /// Gets collection of panels for attributes
	    /// </summary>
	    [XmlArray]
	    [XmlArrayItem("Panel", typeof (Panel))]
	    public ObservableCollection<Panel> Panels { get; set; }

		/// <summary>
		/// Returns the list of active asset type attributes
		/// </summary>
		[XmlArray]
		[XmlArrayItem("AssetTypeAttribute", typeof(AssetTypeAttribute))]
		public ObservableCollection<AssetTypeAttribute> Attributes
		{
			get
			{
				if (_attributes == null)
				{
					_attributes = new ObservableCollection<AssetTypeAttribute>();
					AllAttributes.Where(a => a.IsActive).ForEach(a => _attributes.Add(a));
					_attributes.CollectionChanged += _attributes_CollectionChanged;
				}
				return _attributes;
			}
			set { _attributes = value; }
		}

		/// <summary>
		/// Returns the list of all asset type attributes
		/// </summary>
		[XmlArray]
		[XmlArrayItem("AssetTypeAttribute", typeof(AssetTypeAttribute))]
		public List<AssetTypeAttribute> AllAttributes
		{
			get
			{
			    if (_allAttributes == null)
			    {
			        _allAttributes = new List<AssetTypeAttribute>();
			        if (_base.DynEntityAttribConfigs.Count == 0 && _base.DynEntityConfigUid > 0)
			            _unitOfWork.DynEntityConfigRepository
                            .LoadProperty(_base, e => e.DynEntityAttribConfigs
                                .Select(d => d.DataType.ValidationList));
			        _allAttributes.AddRange(_base.DynEntityAttribConfigs.OrderBy(a => a.DynEntityAttribConfigUid)
                        .Select(data => new AssetTypeAttribute(data, _unitOfWork, this)));
			    }
			    return _allAttributes;
			}
			set { _allAttributes = value; }
		}
        
		[XmlIgnore]
		public MeasureUnit MeasureUnit
		{
			get
			{
				if (_measureUnit == null && MeasureUnitId.HasValue)
				{
					_measureUnit = MeasureUnit.GetByUid(this.MeasureUnitId.Value);
				}
				return _measureUnit;
			}
		}

		/// <summary>
		/// Gets the base DAL entity
		/// </summary>
		[XmlIgnore]
		public DynEntityConfig Base
		{
			get { return _base; }
			set { _base = value; }
		}
		
		private ObservableCollection<AssetTypeAttribute> _attributes;
        private AssetType _draft;
		private MeasureUnit _measureUnit;
		private List<AssetTypeAttribute> _allAttributes;

		private DynEntityConfig _base;
		private readonly IUnitOfWork _unitOfWork;

	    /// <summary>
	    /// Constructor for the new asset type
	    /// </summary>
	    public AssetType(
	        DynEntityConfig data,
	        IUnitOfWork unitOfWork) // TODO: refactor to remove dependency 
	    {
	        if (data == null)
	            throw new ArgumentNullException("DynEntityConfig");
	        if (unitOfWork == null)
	            throw new ArgumentNullException("IUnitOfWork");
	        _base = data;
	        _base.StartTracking();
	        _unitOfWork = unitOfWork;

	        Panels = new ObservableCollection<Panel>((from panel in _base.AttributePanel
	            select new Panel(panel, panel.AttributePanelAttribute
	                .OrderBy(apa => apa.DisplayOrder)
	                .Select(apa => new AssetTypeAttribute(apa.DynEntityAttribConfig, _unitOfWork))
	                .Where(a => a.IsActive)
	                .ToList())).ToList());
	        Panels.CollectionChanged += _panels_CollectionChanged;
	    }

	    /// <summary>
		/// Default constructor needed for serialization
		/// Horror!
		/// </summary>
		public AssetType()
            : this(new DynEntityConfig(), new UnitOfWork())
		{
			IsActive = true;
			IsActiveVersion = true;
			IsIndexed = true;
			IsContextIndexed = true;
		}

		/// <summary>
		/// Returns the existing asset type by its UID from database
		/// </summary>
		/// <param name="uid">Unique ID of AssetType</param>
		/// <returns>AssetType object</returns>        
		[CacheValue("UID")]
		[Obsolete("This method will be removed. Please move to using AssetTypeRepository class instead")]
		public static AssetType GetByUidDb(long uid)
		{
			var unitOfWork = new UnitOfWork();
			var ibuilder = new IncludesBuilder<DynEntityConfig>();
			ibuilder.Add((e) => e.DynEntityAttribConfigs.Select(a => a.DataType));
			ibuilder.Add((e) => e.AttributePanel);
			var data = unitOfWork.DynEntityConfigRepository
				.SingleOrDefault(
					d => d.DynEntityConfigUid == uid,
					includes: ibuilder.Get());

			if (data != null)
			{
				return new AssetType(data, unitOfWork);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Generates the cache key.
		/// </summary>
		/// <param name="uid">The uid.</param>
		/// <returns></returns>
		[CacheKey("UID")]
		[Obsolete("This method will be removed. Please move to using AssetTypeRepository class instead")]
		public static string GetCacheKeyUid(long uid)
		{
			return string.Format("AssetTypeUid_{0}", uid);
		}

	    /// <summary>
	    /// Returns AssetType by ID where version is active - using cache
	    /// </summary>
	    /// <param name="id">The id.</param>
	    /// <returns></returns>
	    [Obsolete("This method will be removed. Please move to using AssetTypeRepository class instead")]
	    public static AssetType GetByID(long id)
	    {
	        var cache = CacheFactory.GetCache<AssetType>("ID");
	        return cache != null 
                ? cache.Get(id) 
                : GetByIdDb(id);
	    }

	    /// <summary>
		/// Returns AssetType by ID where version is active - from database
		/// </summary>
		/// <param name="uid">Unique ID of AssetType</param>
		/// <returns>List of AssetType objects</returns>        
		[CacheValue("ID")]
		[Obsolete("This method will be removed. Please move to using AssetTypeRepository class instead")]
		public static AssetType GetByIdDb(long id)
		{
			var unitOfWork = new UnitOfWork();
			var ibuilder = new IncludesBuilder<DynEntityConfig>();
			ibuilder.Add((e) => e.DynEntityAttribConfigs.Select(a => a.DataType));
			ibuilder.Add((e) => e.AttributePanel);
			var data = unitOfWork.DynEntityConfigRepository
				.SingleOrDefault(
					dec =>
						dec.DynEntityConfigId == id &&
						dec.ActiveVersion == true,
					includes: ibuilder.Get());

			return data != null 
				? new AssetType(data, unitOfWork) 
				: null;
		}

		/// <summary>
		/// Gets the cache key
		/// </summary>
		/// <param name="id">The id</param>
		/// <returns></returns>
		[CacheKey("ID")]
		[Obsolete("This method will be removed. Please move to using AssetTypeRepository class instead")]
		public static string GetCacheKeyId(long id)
		{
			return string.Format("AssetTypeId_{0}", id);
		}

		public static IEnumerable<AssetType> GetAllReservable()
		{
			var predicate = PredicateBuilder.True<DynEntityConfig>();
			predicate = predicate.And(dec => dec.ActiveVersion == true);
			predicate = predicate.And(dec => dec.Active == true);
			predicate = predicate.And(dec => dec.AllowBorrow == true);
			predicate = predicate.And(dec => dec.IsUnpublished == false);
			var unitOfWork = new UnitOfWork();
			foreach (DynEntityConfig item in unitOfWork.DynEntityConfigRepository.Get(predicate, orderBy: entities => entities.OrderBy(e => e.Name)))
			{
				yield return new AssetType(item, unitOfWork);
			}
		}

		/// <summary>
		/// Returns recent AssetTypes
		/// </summary>
		/// <returns></returns>
		[Obsolete("This method will be removed. Please move to using AssetTypeRepository class instead")]
		public static IEnumerable<AssetType> GetRecent()
		{
			var unitOfWork = new UnitOfWork();
			var predicate = PredicateBuilder.True<DynEntityConfig>();
			predicate = predicate.And(dec => dec.ActiveVersion == true);
			predicate = predicate.And(dec => dec.Active == true);
			predicate = predicate.And(dec => dec.IsUnpublished == false);
			foreach (var item in unitOfWork.DynEntityConfigRepository.Get(predicate, orderBy: items => items.OrderByDescending(i => i.UpdateDate)).Take(10))
			{
				yield return new AssetType(item, unitOfWork);
			}
		}

		/// <summary>
		/// Returns the list of all AssetTypes with active revision.
		/// </summary>
		/// <returns>List of AssetType objects</returns>
		[Obsolete("This method will be removed. Please move to using AssetTypeRepository class instead")]
		public static IEnumerable<AssetType> GetAll()
		{
			var unitOfWork = new UnitOfWork();
			var ibuilder = new IncludesBuilder<DynEntityConfig>();
			ibuilder.Add((e) => e.DynEntityAttribConfigs.Select(a => a.DataType));
			ibuilder.Add((e) => e.AttributePanel);

			var predicate = PredicateBuilder.True<DynEntityConfig>();
			predicate = predicate.And(dec => dec.ActiveVersion == true);
			predicate = predicate.And(dec => dec.Active == true);
			return unitOfWork.DynEntityConfigRepository
							 .Where(predicate, ibuilder.Get())
							 .OrderBy(d => d.Name)
							 .Select(item => new AssetType(item, unitOfWork));
		}


		/// <summary>
		/// Returns the list of all AssetTypes with active revision and which use another assets.
		/// </summary>
		/// <returns>List of AssetType objects</returns>
		[Obsolete("This method will be removed. Please move to using AssetTypeRepository class instead")]
		public static IEnumerable<AssetType> GetAllAvailableForAssets()
		{
			var unitOfWork = new UnitOfWork();
            var attributeRepository = new AttributeRepository(unitOfWork);
			var predicate = PredicateBuilder.True<DynEntityConfig>();
			predicate = predicate.And(dec => dec.ActiveVersion == true);
			predicate = predicate.And(dec => dec.Active == true);
			foreach (DynEntityConfig item in unitOfWork.DynEntityConfigRepository.Get(predicate, orderBy: items => items.OrderBy(i => i.Name)))
			{
			    var itemId = item.DynEntityConfigId;
                if (attributeRepository.FindPublishedSingleOrDefault(a => a.RelatedAssetTypeID == itemId) != null)
				{
					yield return new AssetType(item, unitOfWork);
				}
			}
		}

		/// <summary>
		/// Returns the list of all AssetTypes with active revision
		/// which matches by provided name
		/// </summary>
		/// <param name="name">Name of asset to find</param>
		/// <param name="mode">Loading mode</param>
		[Obsolete("This method will be removed. Please move to using AssetTypeRepository class instead")]
		public static IEnumerable<AssetType> FindByName(string name)
		{
			var unitOfWork = new UnitOfWork();
			var predicate = PredicateBuilder.True<DynEntityConfig>();
			predicate = predicate.And(dec => dec.Name.StartsWith(name));
			predicate = predicate.And(dec => dec.ActiveVersion == true);

			foreach (DynEntityConfig item in unitOfWork.DynEntityConfigRepository.Get(predicate, orderBy: items => items.OrderBy(i => i.Name)))
			{
				yield return new AssetType(item, unitOfWork);
			}
		}

		/// <summary>
		/// Gets the <see cref="AppFramework.Core.Classes.AssetTypeAttribute"/> with the specified name.
		/// </summary>
		/// <value></value>
		public AssetTypeAttribute this[string name]
		{
			get
			{
				return Attributes.FirstOrDefault(a => a.Name == name);
			}
		}
        
	    public void RemoveAttribute(long uid)
		{
			this.Attributes.Where(a => a.UID == uid)
						   .ToList()
						   .ForEach(a => this.Attributes.Remove(a));
			this.Panels.ForEach(p =>
				{
					var attr = p.AssignedAttributes.SingleOrDefault(a => a.UID == uid);
					if (attr != null)
						p.RemoveAssetTypeAttribute(attr);
				});
		}

		public void ReloadAttributes()
		{
			_attributes = null;
		}

		/// <summary>
		/// Serializes the specified stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		public void Serialize(System.IO.Stream stream)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(this.GetType());
			xmlSerializer.Serialize(stream, this);
		}

		/// <summary>
		/// Deserializes the specified stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns></returns>
		public static AssetType Deserialize(System.IO.Stream stream)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(AssetType));
			AssetType assetType = xmlSerializer.Deserialize(stream) as AssetType;
			return assetType;
		}

		/// <summary>
		/// Copies attributes, panels etc from given asset type
		/// </summary>
		/// <param name="at"></param>
		public void CopyFrom(AssetType at)
		{
			this.BaseAssetTypeId = at.ID;
			// copy attributes
			at.Attributes.Where(a => this.Attributes.All(q => q.Name != a.Name))
						 .ToList()
						 .ForEach(a => Attributes.Add(a));

            // copy panels
            IEnumerable<Panel> panels = from p in at.Panels
                                        where Panels.All(q => q.Name != p.Name)
                                        select new Panel(new AttributePanel(), null) { Name = p.Name, Description = p.Description };

            panels.ForEach(p => Panels.Add(p));
			this.IsInStock = at.IsInStock;
			this.MeasureUnitId = at.MeasureUnitId;
		}

		void _attributes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (AssetTypeAttribute item in e.NewItems)
				{
					if (Base.DynEntityAttribConfigs.All(a => !a.Equals(item.Base)))
						Base.DynEntityAttribConfigs.Add(item.Base);
					_allAttributes.Add(item);
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (AssetTypeAttribute item in e.OldItems)
				{
					Base.DynEntityAttribConfigs.Remove(item.Base);
					_allAttributes.Remove(item);
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				Base.DynEntityAttribConfigs.Clear();
				_allAttributes.Clear();
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		void _panels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (Panel item in e.NewItems)
				{
					if (Base.AttributePanel.All(a => !a.Equals(item.Base)))
						Base.AttributePanel.Add(item.Base);
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				foreach (Panel item in e.OldItems)
				{
					Base.AttributePanel.Remove(item.Base);
				}
			}
			else if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				Base.AttributePanel.Clear();
			}
			else
			{
				throw new InvalidOperationException();
			}
		}
		
		public bool IsDbColumnExists(string columnName)
		{
			return DBHelper.IsColumnExists(_unitOfWork, DBTableName, columnName);
		}	   
	}
}
