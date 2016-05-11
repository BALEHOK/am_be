namespace AppFramework.Reservations.Models
{
    public class ReservedAssetModel
    {
        public long AssetId { get; set; }

        public long AssetTypeId { get; set; }

        public string AssetName { get; set; }

        public string AssetTypeName { get; set; }
    }
}