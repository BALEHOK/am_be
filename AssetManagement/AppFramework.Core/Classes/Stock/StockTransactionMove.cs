using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppFramework.Core.AC.Authentication;

namespace AppFramework.Core.Classes.Stock
{
    public class StockTransactionMove : StockTransactionValidateRest 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StockTransactionMove"/> class.
        /// </summary>
        /// <param name="moveAssetId">The move asset id.</param>
        /// <param name="assetTypeId">The asset type id.</param>
        /// <param name="assetId">The asset id.</param>
        /// <param name="count">The count.</param>
        /// <param name="price">The price.</param>
        /// <param name="description">The description.</param>
        public StockTransactionMove(IAuthenticationService authenticationService, IAssetsService assetsService, 
            long moveAssetId, long assetTypeId, long assetId, decimal count, decimal price, string description, long? locationId, long? fromLocationId)
            : base(authenticationService, assetsService, TransactionTypeCode.Move, assetTypeId, assetId, count, price, description)
        {
            this.LocationId = locationId;
            this.FromLocationId = fromLocationId;
        }
    }
}
