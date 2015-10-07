using AppFramework.Core.ConstantsEnumerators;
using AppFramework.Entities;

namespace AppFramework.Core.Classes
{
    public class ReservationDescriptor
    {
        public string Name { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ReservationDate { get; set; }
        public string Borrower { get; set; }
        public long BorrowerId { get; set; }
        public string Reserver { get; set; }
        public bool IsClosed { get; set; }
        public bool IsBorrowed { get; set; }
        public bool IsNotBorrowed { get { return !IsBorrowed; } }
        public long ReservationUid { get; set; }
        public long ReserverUid { get; set; }
        public long AssetUid { get; set; }
        public long AssetTypeUid { get; set; }
        public string Description { get; set; }

        public static ReservationDescriptor ReservationToDescriptor(
            Reservation reservation, 
            AssetType assetType, 
            AssetType userAssetType,
            IAssetsService assetsService)
        {
            var desc = new ReservationDescriptor
            {
                ReservationUid = reservation.ReservationUID,
                Name = assetsService.GetAssetByUid(reservation.DynEntityUID, assetType)[AttributeNames.Name].Value,
                Borrower = assetsService.GetAssetById(reservation.Borrower, userAssetType)[AttributeNames.Name].Value,
                BorrowerId = reservation.Borrower,
                Reserver = assetsService.GetAssetById(reservation.UpdateUserId, userAssetType)[AttributeNames.Name].Value,
                IsClosed = reservation.IsClosed,
                EndDate = reservation.EndDate.ToShortDateString(),
                ReservationDate = reservation.ReservationDate.ToShortDateString(),
                StartDate = reservation.StartDate.ToShortDateString(),
                ReserverUid = reservation.UpdateUserId,
                IsBorrowed = reservation.IsBorrowed,
                AssetUid = reservation.DynEntityUID,
                AssetTypeUid = reservation.DynEntityConfigUID,
                Reason = reservation.Reason,
                Description = string.Format("Reason: {0}<br/>Is Damaged: {1}<br/>Remark:{2}",
                    reservation.Reason, reservation.IsDamaged, reservation.Remarks)
            };
            return desc;
        }

        public string Reason { get; set; }
    }
}