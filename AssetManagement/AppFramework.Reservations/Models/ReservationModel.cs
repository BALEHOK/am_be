using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppFramework.Reservations.Models
{
    public class ReservationModel
    {
        public long Id { get; set; }

        [Range(1, long.MaxValue)]
        public long BorrowerId { get; set; }

        public long BorrowerConfigId { get; set; }

        public string BorrowerName { get; set; }

        public string Comment { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public ReservationState State { get; set; }

        public IEnumerable<ReservedAssetModel> ReservedAssets { get; set; }

        public static ReservationModel FromEntity(Entities.Reservation entity)
        {
            return new ReservationModel
            {
                Id = entity.ReservationId,
                BorrowerId = entity.BorrowerId,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Comment = entity.Comment,
                State = (ReservationState)entity.State,
            };
        }

        public static Entities.Reservation ToEntity(ReservationModel model)
        {
            return UpdateEntityFromModel(new Entities.Reservation(), model); 
        }

        public static Entities.Reservation UpdateEntityFromModel(Entities.Reservation existingEntity, ReservationModel model)
        {
            existingEntity.ReservationId = model.Id;
            existingEntity.BorrowerId = model.BorrowerId;
            existingEntity.StartDate = model.StartDate;
            existingEntity.EndDate = model.EndDate;
            existingEntity.Comment = model.Comment;
            existingEntity.State = (short)model.State;
            return existingEntity;
        }
               
    }

}
