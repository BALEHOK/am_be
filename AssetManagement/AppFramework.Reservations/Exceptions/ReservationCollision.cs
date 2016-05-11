using System;

namespace AppFramework.Reservations.Exceptions
{
    public class ReservationCollision : Exception
    {
        public ReservationCollision(long assetId, long assetTypeId, DateTime startDate, DateTime endDate)
            : base(string.Format(
                "Cannot reserve asset #{0} (asset type #{1}) from {2} till {3} because it is already reserved in that interval",
                assetId, assetTypeId, startDate.Date, endDate.Date))
        {
        }
    }
}
