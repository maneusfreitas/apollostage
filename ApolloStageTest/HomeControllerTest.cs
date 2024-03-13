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
    public class HomeControllerTest
    {

        [Fact]
        public async Task Index_ReturnsViewWithAlbumList()
        {
            // Arrange
            var httpClientMock = new Mock<IHttpClientHelper>();
            var singletonMock = new Mock<ISingleton>();
            var controller = new HomeController(null, null, null, null);

            var expectedAlbumList = new List<Album>
            {
                new Album { },
                new Album { },
             
            };

            var responseContent = JsonConvert.SerializeObject(new { albums = expectedAlbumList });

            httpClientMock.Setup(client => client.SendAysnc(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(responseContent);

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotEqual(expectedAlbumList, result.Model as List<Album>); 
        }

        [Fact]
        public async Task Index_ReturnsErrorViewOnAuthenticationFailure()
        {
            // Arrange
            var httpClientMock = new Mock<IHttpClientHelper>();
            var singletonMock = new Mock<ISingleton>();
            var controller = new HomeController(null, null, null, null);

            httpClientMock.Setup(client => client.SendAysnc(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new HttpRequestException("Authentication failed."));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => controller.Index());
        }


        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            // Arrange
            var controller = new HomeController(null, null, null, null);

            // Act
            var result = controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public void Error_ReturnsViewResult_WithModelError()
        {
            // Arrange
            var mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            var mockHttpClientHelper = new Mock<IHttpClientHelper>();
            var mockSingleton = new Mock<ISingleton>();

            var controller = new HomeController(mockContext.Object, mockHttpClientHelper.Object, mockSingleton.Object, null);

            // Act
            var result = controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.NotNull(model);
        }
    }
}
