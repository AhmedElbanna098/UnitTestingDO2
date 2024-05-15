using CarAPI.Entities;
using CarFactoryAPI.Entities;
using CarFactoryAPI.Repositories_DAL;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Moq;
using Moq.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BannaTestingCar
{
    public class OwnerRepositoryTests
    {
        Mock<FactoryContext> factoryContextMock;

        OwnerRepository repository;

        public OwnerRepositoryTests()
        {
            factoryContextMock = new();
            repository = new(factoryContextMock.Object);
        }

        [Fact]
        [Trait("OwnerS","Found")]
        public void GetOwnerById_AskForId10_OwnerObj()
        {
            //Arrange
            List<Owner> owners = new List<Owner>()
            {
                new Owner() { Id = 10 },
                new Owner() { Id = 20 },
                new Owner() { Id = 30 },
            };

            //set up
            factoryContextMock.Setup(o=>o.Owners).ReturnsDbSet(owners);

            //Act
            Owner result = repository.GetOwnerById(10);

            //Assert
            Assert.NotNull(result);
        }
       
    }
}
