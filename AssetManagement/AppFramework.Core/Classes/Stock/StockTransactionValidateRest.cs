using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes.Validation;
using AppFramework.Core.Validation;

namespace AppFramework.Core.Classes.Stock
{
    public class StockTransactionValidateRest : StockTransaction
    {
        public StockTransactionValidateRest(IAuthenticationService authenticationService, IAssetsService assetsService, 
            TransactionTypeCode type, long assetTypeId, long assetId, decimal count, decimal price, string description)
            : base(authenticationService, assetsService, 
            type, assetTypeId, assetId, count, price, description)
        {

        }

        public override ValidationResult Validate()
        {
            ValidationResult res = base.Validate();
            StockTransactionManager manager = new StockTransactionManager();
            var available = manager.GetAvailableLocationsFor(Asset.ID, Asset.GetConfiguration().ID);
            if (this.FromLocationId.HasValue)
            {
                if (!available.ContainsKey(this.FromLocationId.Value) ||
                    (available.ContainsKey(this.FromLocationId.Value) && available[this.FromLocationId.Value] < this.StockCount))
                {
                    var rest = available.ContainsKey(this.FromLocationId.Value) ? available[FromLocationId.Value] : 0;

                    res +=
                        ValidationResultLine.Error(
                            string.Empty,
                            string.Format("Location has not enough items ({0} rest, {1} requested)", (int) rest, StockCount));
                }
            }

            return res;
        }
    }
}
