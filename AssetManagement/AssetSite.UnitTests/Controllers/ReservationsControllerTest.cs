using AppFramework.DataProxy;
using AppFramework.Entities;
using AssetManager.Infrastructure.Models;
using AssetManager.WebApi.Controllers.Api;
using AssetSite.UnitTests.Fixtures;
using Moq;
using Ploeh.AutoFixture.Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Xunit.Extensions;

namespace AssetSite.UnitTests.Controllers
{
    public class ReservationsControllerTest
    {
        [Theory, AutoDomainData]
        public void ReservationsController_GetAllReservations_ReturnsCollection(
            [Frozen]Mock<IUnitOfWork> unitOfWorkMock,
            List<Reservation> reservations,
            ReservationsController sut)
        {
            // Arrange 
            unitOfWorkMock
                .Setup(x => x.ReservationRepository.Get(null, null, null, null, null))
                .Returns(reservations);
            // Act 
            var result = sut.GetAll();
            // Assert
            Assert.Equal(reservations.Count, result.Count());
        }

        [Theory, AutoDomainData]
        public void ReservationsController_GetById_ReturnsReservation(
            [Frozen]Mock<IUnitOfWork> unitOfWorkMock,
            long reservationId,
            Reservation reservation,
            ReservationsController sut)
        {
            // Arrange 
            unitOfWorkMock
                .Setup(x => x.ReservationRepository.SingleOrDefault(
                    It.IsAny<Expression<Func<Reservation, bool>>>(), It.IsAny<Expression<Func<Reservation, object>>>()))
                .Returns(reservation);
            // Act 
            var result = sut.GetById(reservationId);
            // Assert
            Assert.Equal(reservation.ReservationId, result.Id);
            Assert.Equal(reservation.StartDate, result.StartDate);
            Assert.Equal(reservation.EndDate, result.EndDate);
            Assert.Equal(reservation.Comment, result.Comment);
            Assert.Equal(reservation.State, (short)result.State);
        }
    }
}
