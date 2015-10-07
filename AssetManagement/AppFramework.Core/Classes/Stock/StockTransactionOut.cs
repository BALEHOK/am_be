using AppFramework.Core.AC.Authentication;

namespace AppFramework.Core.Classes.Stock
{
    using System;
    using System.Linq;

    public class StockTransactionOut : StockTransactionValidateRest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StockTransactionOut"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="assetTypeId">The asset type id.</param>
        /// <param name="assetUid">The asset uid.</param>
        /// <param name="count">The count.</param>
        /// <param name="description">The description.</param>
        public StockTransactionOut(IAuthenticationService authenticationService, IAssetsService assetsService, 
            long assetTypeId, long assetId, decimal count, decimal price, string description, long? endUserId, long? fromLocationId)
            : base(authenticationService, assetsService, 
            TransactionTypeCode.Out, assetTypeId, assetId, count, price, description)
        {
            this.TransactionType = TransactionTypeCode.Out;
            this.EndUserId = endUserId;
            this.FromLocationId = fromLocationId;
        }

		public override void Commit()
		{
			this.UpdateAssetStockInfo(-this.StockCount, this.StockCount * this.StockPrice);//TODO: not sure about the price
			base.Commit();
		}

    }
}
