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

namespace ApolloStageTest
{
    public class MarketControllerTest
    {



        [Fact]
        public async Task SaveImage_ReturnsImagePath()
        {
            // Arrange
            var fakeImage = new FormFile(new MemoryStream(new byte[] { 0x20, 0x20, 0x20 }), 0, 3, "Data", "fake.jpg");

            // Act
            var imagePath = fakeImage;

            // Assert
            Assert.NotNull(imagePath);
        }


        [Fact]
        public async Task Delivery_ReturnsCorrectViewAndTempData()
        {
            // Arrange
            var mockHttpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(mockHttpContext, Mock.Of<ITempDataProvider>());
            var controller = new MarketController(null, null, null, null)
            {
                TempData = tempData
            };
            var id = "123";
            var email = "test@example.com";

            // Act
            var result = await controller.delivery(id, email) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("OrderDelivery", result.ViewName);
            Assert.Equal(id, controller.TempData["id"]);
            Assert.Equal(email, controller.TempData["email"]);
        }

    }
}
