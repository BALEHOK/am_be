using AppFramework.Core.AC.Authentication;

namespace AppFramework.Core.Classes.Stock
{
    using System.Collections.Generic;
    using AppFramework.Entities;
    using LinqKit;

    public class StockTransactionIn : StockTransaction
    {
        public StockTransactionIn(IAuthenticationService authenticationService, IAssetsService assetsService, 
            long assetTypeId, long assetUid, decimal count, decimal price, string description, long? locationId)
            : base(authenticationService, assetsService, 
            TransactionTypeCode.In, assetTypeId, assetUid, count, price, description)
        {
            this.LocationId = locationId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StockTransactionIn"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public StockTransactionIn(Entities.DynEntityTransaction item)
            : base(item)
        {
        }

        public override void Commit()
        {
            this.UpdateAssetStockInfo(this.StockCount, this.StockCount * this.StockPrice);
            base.Commit();
        }

        /// <summary>
        /// Gets the incoming open transactions
        /// </summary>
        /// <param name="assetTypeId">The asset type id.</param>
        /// <param name="assetId">The asset uid.</param>
        /// <returns></returns>
        public static IEnumerable<StockTransactionIn> GetIncomingOpen(long assetTypeId, long assetId)
        {
            var predicate = PredicateBuilder.True<DynEntityTransaction>();
            predicate = predicate.And(t => t.DynEntityConfigId == assetTypeId);
            predicate = predicate.And(t => t.DynEntityId == assetId);
            predicate = predicate.And(t => t.TransactionTypeUid == ((int)TransactionTypeCode.In));
            predicate = predicate.And(t => t.CloseDate == null);

            var unitOfWork = new DataProxy.UnitOfWork();
            foreach (var item in unitOfWork.DynEntityTransactionRepository.Get(predicate))
            {
                yield return new StockTransactionIn(item);
            }
        }
    }
}
