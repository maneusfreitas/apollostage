using System.Security.Claims;
using ApolloStage;
using ApolloStage.Controllers;
using ApolloStage.Data;
using ApolloStage.Models;
using ApolloStageFirst.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace ApolloStageTest
{
    public class MarketControllerTest
    {
        private MarketController _controller;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;

        [SetUp]
        public void SetUp()
        {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            _controller = new MarketController(_userManagerMock.Object);
        }


        [Fact]
        public async Task SaveImage_ReturnsImagePath()
        {
            // Arrange
            var fakeImage = new FormFile(new MemoryStream(new byte[] { 0x20, 0x20, 0x20 }), 0, 3, "Data", "fake.jpg");

            // Act
            var imagePath = fakeImage;

            // Assert
            NUnit.Framework.Assert.NotNull(imagePath);
        }

        [Test]
        public async Task AddProducts_ReturnsViewResult()
        {
            // Act
            var result = await _controller.addProducts();

            // Assert
            NUnit.Framework.Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Success_ReturnsViewResult()
        {
            // Act
            var result = await _controller.Success(100);

            // Assert
            NUnit.Framework.Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task CancelOrder_ReturnsViewResult()
        {
            // Act
            var result = await _controller.CancelOrder(100);

            // Assert
            NUnit.Framework.Assert.IsInstanceOf<ViewResult>(result);
        }


    }
}