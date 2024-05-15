using CarAPI.Entities;
using CarAPI.Models;
using CarAPI.Payment;
using CarAPI.Repositories_DAL;
using CarAPI.Services_BLL;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BannaTestingCar
{
    public class OwnersServiceTest : IDisposable
    {
         Mock<IOwnersRepository> _ownersRepositoryMock;
         Mock<ICarsRepository> _carsRepositoryMock;
         Mock<ICashService> _cashServiceMock;
         OwnersService _ownersService;
        public OwnersServiceTest()
        {
            _ownersRepositoryMock = new Mock<IOwnersRepository>();
            _carsRepositoryMock = new Mock<ICarsRepository>();
            _cashServiceMock = new Mock<ICashService>();
            _ownersService = new OwnersService(_carsRepositoryMock.Object, _ownersRepositoryMock.Object ,_cashServiceMock.Object);
        }
        public void Dispose()
        {
            _ownersRepositoryMock = null;
            _carsRepositoryMock = null;
            _cashServiceMock = null;
            _ownersService = null;
        }

        #region hasCar/hasNoCar
        [Fact]
        [Trait("Category", "Failure")]
        public void BuyCar_OwnerAlreadyHasCar_ReturnsAlreadyHaveCar()
        {
            //Arrange
            var input = new BuyCarInput { CarId = 1, OwnerId = 1, Amount= 1000 };
            var car = new Car { Id = 1 };
            var owner = new Owner { Id = input.OwnerId, Car = car };
            _ownersRepositoryMock.Setup(o=>o.GetOwnerById(input.OwnerId)).Returns(owner);
            _carsRepositoryMock.Setup(o=>o.GetCarById(input.CarId)).Returns(car);

            //Act
            var result = _ownersService.BuyCar(input);

            //Assert
            Assert.Contains("car", result);

        }
        [Fact]
        [Trait("Category","Success")]
        public void BuyCar_OwnerHasNoCar_SuccessfulPurchase()
        {
            //Arrange
            var input = new BuyCarInput { CarId = 1, OwnerId = 1, Amount = 1000 };
            var owner = new Owner { Id = input.OwnerId, Car = null };
            var car = new Car { Id = input.CarId, Price = 1000 };

            _ownersRepositoryMock.Setup(o => o.GetOwnerById(input.OwnerId)).Returns(owner);
            _carsRepositoryMock.Setup(o=>o.GetCarById(input.CarId)).Returns(car);
            _cashServiceMock.Setup(o => o.Pay(car.Price)).Returns("Success");
            _carsRepositoryMock.Setup(o => o.AssignToOwner(input.CarId, input.OwnerId)).Returns(true);

            //Act
            var result = _ownersService.BuyCar(input);

            //Assert
            Assert.Contains("Successfull", result);
        }
        #endregion

        #region funds sufficient/insufficient
        [Fact]
        [Trait("Category", "Failure")]
        public void BuyCar_InsufficientFunds_ReturnsInsufficientFunds()
        {
            // Arrange
            var input = new BuyCarInput { CarId = 1, OwnerId = 1, Amount = 500 };
            var car = new Car { Id = input.CarId, Price = 1000 };
            var owner = new Owner() { Id = 1 };
            _carsRepositoryMock?.Setup(repo => repo.GetCarById(input.CarId)).Returns(car);
            _ownersRepositoryMock?.Setup(o => o.GetOwnerById(input.OwnerId)).Returns(owner);

            // Act
            var result = _ownersService?.BuyCar(input);

            // Assert
            Assert.Contains("Insufficient funds", result);
        }

        [Fact]
        [Trait("Category", "Success")]
        public void BuyCar_SufficientFunds_SuccessfulPurchase()
        {
            // Arrange
            var input = new BuyCarInput { CarId = 1, OwnerId = 1, Amount = 1500 };
            var car = new Car { Id = input.CarId, Price = 1000 };
            var owner = new Owner { Id = input.OwnerId };
            _carsRepositoryMock?.Setup(repo => repo.GetCarById(input.CarId)).Returns(car);
            _ownersRepositoryMock?.Setup(repo => repo.GetOwnerById(input.OwnerId)).Returns(owner);
            _cashServiceMock?.Setup(service => service.Pay(car.Price)).Returns("Success");
            _carsRepositoryMock?.Setup(repo => repo.AssignToOwner(input.CarId, input.OwnerId)).Returns(true);

            // Act
            var result = _ownersService?.BuyCar(input);

            // Assert
            Assert.Contains("Successfull",result);
        }
        #endregion

        #region Transaction success/failure
        [Fact]
        [Trait("Category", "Failure")]
        public void BuyCar_TransactionFails_ReturnsSomethingWentWrong()
        {
            // Arrange
            var input = new BuyCarInput { CarId = 1, OwnerId = 1, Amount = 1500 };
            var car = new Car { Id = input.CarId, Price = 1000 };
            var owner = new Owner { Id = input.OwnerId };
            _carsRepositoryMock?.Setup(repo => repo.GetCarById(input.CarId)).Returns(car);
            _ownersRepositoryMock?.Setup(repo => repo.GetOwnerById(input.OwnerId)).Returns(owner);
            _cashServiceMock?.Setup(service => service.Pay(car.Price)).Returns("Success");
            _carsRepositoryMock?.Setup(repo => repo.AssignToOwner(input.CarId, input.OwnerId)).Returns(false);

            // Act
            var result = _ownersService?.BuyCar(input);

            // Assert
            Assert.Contains("Something went wrong", result);
        }

        [Fact]
        [Trait("Category", "Success")]
        public void BuyCar_TransactionSucceeds_SuccessfulPurchase()
        {
            // Arrange
            var input = new BuyCarInput { CarId = 1, OwnerId = 1, Amount = 1500 };
            var car = new Car { Id = input.CarId, Price = 1000 };
            var owner = new Owner { Id = input.OwnerId };
            _carsRepositoryMock?.Setup(repo => repo.GetCarById(input.CarId)).Returns(car);
            _ownersRepositoryMock?.Setup(repo => repo.GetOwnerById(input.OwnerId)).Returns(owner);
            _cashServiceMock?.Setup(service => service.Pay(car.Price)).Returns("Success");
            _carsRepositoryMock?.Setup(repo => repo.AssignToOwner(input.CarId, input.OwnerId)).Returns(true);

            // Act
            var result = _ownersService?.BuyCar(input);

            // Assert
            Assert.Contains("Successfull", result);
        }
        #endregion
    }
}
