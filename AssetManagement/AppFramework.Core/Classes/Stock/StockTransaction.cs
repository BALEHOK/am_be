using AppFramework.Core.DAL.Adapters;
using AppFramework.Core.Validation;
using AppFramework.DataProxy;

namespace AppFramework.Core.Classes.Stock
{
    using AppFramework.Core.AC.Authentication;
    using AppFramework.Core.DAL;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class for realization logic of stock transactions. Allow get assets in stock system, out them and move between departments
    /// </summary>
    public class StockTransaction
    {
        protected bool isTransactionValid;
        protected Asset asset;
        private DateTime? closeDate;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAssetsService _assetsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockTransaction"/> class.
        /// </summary>
        public StockTransaction(IAuthenticationService authenticationService, IAssetsService assetsService)
        {
            if (authenticationService == null)
                throw new ArgumentNullException();
            if (assetsService == null)
                throw new ArgumentNullException();
            _authenticationService = authenticationService;
            _assetsService = assetsService;
            this.TransactionDate = DateTime.Now;
            this.isTransactionValid = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StockTransaction"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="assetTypeId">The asset type id.</param>
        /// <param name="assetUid">The asset uid.</param>
        /// <param name="count">The count.</param>        
        /// <param name="description">The description.</param>
        /// <param name="comment">The comment.</param>
        public StockTransaction(IAuthenticationService authenticationService, IAssetsService assetsService,
            TransactionTypeCode type, long assetTypeId, long assetId, decimal count, decimal price, string description)
            : this(authenticationService, assetsService)
        {
            this.AssetTypeId = assetTypeId;
            this.AssetId = assetId;
            this.StockCount = count;
            this.TransactionType = type;
            this.Description = description;
            this.StockPrice = price;
            this.RestCount = count;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StockTransaction"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public StockTransaction(Entities.DynEntityTransaction item)
        {
            this.UID = item.DynEntityTransactionUid;
            this.TransactionType = (TransactionTypeCode)item.TransactionTypeUid;
            this.StockCount = item.StockCount;
            this.StockPrice = item.StockPrice;
            this.TransactionDate = item.TransactionDate;
            this.Description = item.Description;
            this.RefTransactionUid = item.RefTransactionUid;
            this.closeDate = item.CloseDate;
            this.RestCount = item.RestCount;
            this.AssetTypeId = item.DynEntityConfigId;
            this.AssetId = item.DynEntityId;
            this.LocationId = item.LocationId;
            this.FromLocationId = item.FromLocationId;
            this.EndUserId = item.EndUserId;
        }

        /// <summary>
        /// Gets or sets the UID.
        /// </summary>
        /// <value>The UID.</value>
        public long UID
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the asset type uid.
        /// </summary>
        /// <value>The asset type uid.</value>
        public long AssetTypeId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the asset uid.
        /// </summary>
        /// <value>The asset uid.</value>
        public long AssetId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ref transaction uid.
        /// </summary>
        /// <value>The ref transaction uid.</value>
        public long? RefTransactionUid
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the type of the asset.
        /// </summary>
        /// <value>The type of the asset.</value>
        public AssetType AssetType
        {
            get
            {
                return AssetType.GetByID(this.AssetTypeId);
            }
        }

        /// <summary>
        /// Gets the asset.
        /// </summary>
        /// <value>The asset.</value>
        public Asset Asset
        {
            get { return asset ?? (asset = _assetsService.GetAssetById(AssetId, AssetType)); }
        }

        /// <summary>
        /// Gets or sets the stock count.
        /// </summary>
        /// <value>The stock count.</value>
        public decimal StockCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the transaction.
        /// </summary>
        /// <value>The type of the transaction.</value>
        public TransactionTypeCode TransactionType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the transaction date.
        /// </summary>
        /// <value>The transaction date.</value>
        public DateTime TransactionDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the asset where moving to
        /// </summary>
        /// <value>The asset</value>
        public long MovingAssetUid
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the stock price.
        /// </summary>
        /// <value>The stock price.</value>
        public decimal StockPrice
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the rest count.
        /// </summary>
        /// <value>The rest count.</value>
        public decimal RestCount
        {
            get;
            set;
        }

        public long? LocationId { get; set; }
        public long? FromLocationId { get; set; }
        public long? EndUserId { get; set; }

        /// <summary>
        /// Gets or sets the close date.
        /// </summary>
        /// <value>The close date.</value>
        public DateTime? CloseDate
        {
            get
            {
                return closeDate;
            }
            set
            {
                closeDate = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this transaction is closed.
        /// </summary>
        /// <value><c>true</c> if this instance is closed; otherwise, <c>false</c>.</value>
        public bool IsClosed
        {
            get
            {
                return closeDate.HasValue;
            }
        }

        public static IEnumerable<StockTransaction> GetAll(long assetTypeId, long assetId)
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            foreach (var item in unitOfWork.DynEntityTransactionRepository.Get(t => t.DynEntityConfigId == assetTypeId &&
                t.DynEntityId == assetId))
            {
                yield return new StockTransaction(item);
            }
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            var unitOfWork = new DataProxy.UnitOfWork();
            Entities.DynEntityTransaction trans = null;
            if (this.UID == 0)
            {
                trans = this.ToDynEntityTransaction();
                unitOfWork.DynEntityTransactionRepository.Insert(trans);
            }
            else
            {
                trans = unitOfWork.DynEntityTransactionRepository.Single(t => t.DynEntityTransactionUid == this.UID);
                trans.RestCount = this.RestCount;
                trans.CloseDate = this.CloseDate;
                unitOfWork.DynEntityTransactionRepository.Update(trans);
            }
            unitOfWork.Commit();
            this.UID = trans.DynEntityTransactionUid;
        }

        /// <summary>
        /// Convert to the DynEntityTransaction
        /// </summary>
        /// <returns></returns>
        protected virtual Entities.DynEntityTransaction ToDynEntityTransaction()
        {
            return new Entities.DynEntityTransaction()
            {
                TransactionTypeUid = (long)this.TransactionType,
                DynEntityId = this.AssetId,
                DynEntityConfigId = this.AssetTypeId,
                StockCount = this.StockCount,
                StockPrice = this.StockPrice,
                Description = this.Description,
                UpdateDate = DateTime.Now,
                UpdateUserId = (long)_authenticationService.CurrentUser.ProviderUserKey,
                TransactionDate = this.TransactionDate,
                RefTransactionUid = this.RefTransactionUid,
                RestCount = this.RestCount,
                CloseDate = this.CloseDate,
                LocationId = this.LocationId,
                FromLocationId = this.FromLocationId,
                EndUserId = this.EndUserId
            };
        }

        /// <summary>
        /// Updates the asset stock info after creating new transaction.
        /// </summary>
        public void SaveAssetStockInfo()
        {
            var unitOfWork = new UnitOfWork();
            var dtService = new DataTypeService(unitOfWork);
            var adapter = new DynColumnAdapter(dtService);
            var provider = new DynTableProvider(unitOfWork, adapter);
            provider.UpdateAsset(Asset);
        }

        /// <summary>
        /// Updates the asset stock info.
        /// </summary>
        /// <param name="cnt">The total CNT.</param>
        /// <param name="prc">The total PRC.</param>
        public void UpdateAssetStockInfo(decimal cnt, decimal prc)
        {
            decimal count = 0;
            decimal.TryParse(this.Asset["StockCount"].Value, out count);
            this.Asset["StockCount"].Value = (count + cnt).ToString();

            decimal price = 0;
            decimal.TryParse(this.Asset["StockPrice"].Value, out price);
            this.Asset["StockPrice"].Value = (price + prc).ToString();
        }

        /// <summary>
        /// Commits this instance. Commit include save transaction in database, updating asset financial (stock) attributes and runs generated transactions (in case of moving assets)
        /// </summary>
        public virtual void Commit()
        {
            if (isTransactionValid)
            {
                this.Save();
                this.SaveAssetStockInfo();
            }
            else
            {
                throw new InvalidOperationException("Stock transaction is not valid!");
            }
        }

        /// <summary>
        /// Validates this instance - for out and moving transactions check rest
        /// </summary>
        /// <returns></returns>
        public virtual ValidationResult Validate()
        {
            ValidationResult res = new ValidationResult();

            //res.IsValid = true; todo: something strange there
            isTransactionValid = true;

            return res;
        }
    }
}
