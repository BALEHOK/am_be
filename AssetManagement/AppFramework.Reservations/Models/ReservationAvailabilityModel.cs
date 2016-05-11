using AppFramework.Entities;
using System.Collections.Generic;

namespace AppFramework.Reservations.Models
{
    public class ReservationAvailabilityModel
    {
        public int IsAvailable { get; set; }

        public ReservationAvailabilityModel(IEnumerable<Reservation> reservations)
        {

        }
    }
}
