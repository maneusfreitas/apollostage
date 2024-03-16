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
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;

namespace ApolloStageTest
{
    public class MarketControllerTest
    {

        [Fact]
        public async Task Market_ReturnsCorrectView()
        {
            // Arrange
            var controller = new MarketController(null,null,null,null,null);

            // Act
            var result = await controller.market();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("index", viewResult.ViewName);
        }
    }
}
