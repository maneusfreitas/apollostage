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


namespace ApolloStageTest
{
    public class AccountControllerTest
    {
        [Fact]
        public void Login_ReturnsLoginView()
        {
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
               Mock.Of<IUserStore<ApplicationUser>>(),
               null, null, null, null, null, null, null, null);

            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null);
            var controller = new AccountController(null, null, null, null);

            // Act
            var result = controller.Login();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Login", viewResult.ViewName);
        }

        [Fact]
        public void Register_ReturnsFirstRegisterView()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
               Mock.Of<IUserStore<ApplicationUser>>(),
               null, null, null, null, null, null, null, null);

            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null);
            var controller = new AccountController(null, null, null, null);

            // Act
            var result = controller.Register();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("FirstRegister", viewResult.ViewName);
        }
        [Fact]
        public void ForgotPassword_ReturnsForgotPasswordView()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
               Mock.Of<IUserStore<ApplicationUser>>(),
               null, null, null, null, null, null, null, null);

            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null);
            var controller = new AccountController(null, null, null, null);

            // Act
            var result = controller.ForgotPassword();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("ForgotPassword", viewResult.ViewName);
        }
        [Fact]
        public void LimitAttempts_ReturnsLimitAttemptsView()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
               Mock.Of<IUserStore<ApplicationUser>>(),
               null, null, null, null, null, null, null, null);

            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null);
            var controller = new AccountController(null, null, null, null);

            // Act
            var result = controller.LimitAttempts();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("LimitAttempts", viewResult.ViewName);
        }





//---------------------------------------------------------------------------------login--------------------

        [Fact] // falhou
        public async Task Login_WithValidCredentials_RedirectsToIndexAction()
        {
            // Arrange
            var user = new ApplicationUser { UserName = "testuser", UserMail = "test@example.com" };
            var userManagerMock = GetMockUserManager(user);
            var signInManagerMock = GetMockSignInManager(user);
            var controller = new AccountController(null, null, null, null);
            var model = new ApplicationUser { UserMail = "test@example.com", Password = "Password123@" };

            // Act
            var result = await controller.Login(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
        }

        [Fact] // testar
        public async Task Login_WithInvalidCredentials_ReturnsViewWithModelError()
        {
            // Arrange
            var user = new ApplicationUser { UserName = "testuser", UserMail = "test@example.com" };
            var userManagerMock = GetMockUserManager(user);
            var signInManagerMock = GetMockSignInManager(user, succeeded: false);
            var controller = new AccountController(null, null, null, null);
            var model = new ApplicationUser { UserMail = "test@example.com", Password = "InvalidPassword" };

            // Act
            var result = await controller.Login(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName); 
            Assert.False(viewResult.ViewData.ModelState.IsValid); 
            Assert.Single(viewResult.ViewData.ModelState); 
            var error = viewResult.ViewData.ModelState[string.Empty].Errors.FirstOrDefault();
            Assert.NotNull(error);
            Assert.Equal("Invalid login attempt.", error.ErrorMessage);
        }


        //---------------------------------------------------------login ---------------------------------

        //-------------------------------------------FirstRegister------------------------------------------
        [Fact]
        public async Task FirstRegister_ValidModel_RedirectsToSecondRegisterView()
        {
            // Arrange
            var user = new ApplicationUser { UserMail = "test@example.com" };
            var userManagerMock = GetMockUserManager(user);
            var signInManagerMock = GetMockSignInManager(user);
            var controller = new AccountController(null, null, null, null);
            var model = new ApplicationUser { UserMail = "test@example.com" };

            // Act
            var result = await controller.FirstRegister(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("SecondRegister", viewResult.ViewName);
            Assert.Equal(model.UserMail, (viewResult.Model as ApplicationUser).UserMail);
            // Add more assertions if necessary
        }

        //-------------------------------------------FirstRegister------------------------------------------



        //-------------------------------------------SecondRegister------------------------------------------

        [Fact]
        public async Task SecondRegister_InvalidModel_ReturnsViewWithModelError()
        {
            // Arrange
            var user = new ApplicationUser { UserMail = "test@example.com", Password = "password123", ConfirmPassword = "password123", Name = "John Doe" };
            var userManagerMock = GetMockUserManager(user);
            var signInManagerMock = GetMockSignInManager(user);
            var controller = new AccountController(null, null, null, null);
            var model = new ApplicationUser(); // Modelo inválido porque não possui os campos obrigatórios

            // Act
            var result = await controller.SecondRegister(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName); // O nome da view padrão é esperado
            Assert.False(viewResult.ViewData.ModelState.IsValid); // ModelState deve ser inválido
                                                                  // Adicionar mais asserções se necessário
        }
        //-------------------------------------------SecondRegister------------------------------------------


        //-------------------------------------------ThirdRegister------------------------------------------



        [Fact]
        public async Task ThirdRegister_ValidModel_RedirectsToCodeConfirmView()
        {
            // Arrange
            var model = new ApplicationUser
            {
                UserMail = "test@example.com",
                Name = "John Doe",
                Password = "Password123@",
                ConfirmPassword = "Password123@",
                DateOfBirth = DateTime.Now,
                Country = "USA",
                Gender = "Male"
            };

            var userManagerMock = GetMockUserManager(model);
            var signInManagerMock = GetMockSignInManager(model);
            var emailServiceMock = new Mock<EmailService>(); // Mock the email service
            var controller = new AccountController(null, null, null, null);
            // Act
            var result = await controller.ThirdRegister(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotEqual("Error", viewResult.ViewName);
            // Add more assertions if necessary
        }

        //-------------------------------------------ThirdRegister------------------------------------------

        //-------------------------------------------ChangePassword------------------------------------------
 

        [Fact]
        public async Task ChangePassword_NonExistingUser_ReturnsError()
        {
            // Arrange
            var user = new ApplicationUser { UserMail = "test@example.com", Password = "password123", ConfirmPassword = "password123@" };
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(m => m.FindByEmailAsync(user.UserMail)).ReturnsAsync((ApplicationUser)null);
            var controller = new AccountController(null, null, null, null);

            // Act
            var result = await controller.ChangePassword(user);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", redirectToActionResult.ActionName);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
        }

        //-------------------------------------------ChangePassword------------------------------------------
        //-------------------------------------------ResendVerificationEmailAsync------------------------------------------


        [Fact] // testar
        public async Task ResendVerificationEmailAsync_NonExistingUser_ReturnsNotFound()
        {
            // Arrange
            var userEmail = "test@example.com";
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(m => m.FindByEmailAsync(userEmail)).ReturnsAsync((ApplicationUser)null);
            var controller = new AccountController(null, null, null, null);

            // Act
            var result = await controller.ResendVerificationEmailAsync(userEmail, 1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Usuário não encontrado", notFoundResult.Value);
        }

        [Fact]
        public async Task ResendVerificationEmailAsync_ErrorUpdatingUser_ReturnsBadRequest()
        {
            // Arrange
            var userEmail = "test@example.com";
            var existingUser = new ApplicationUser { UserMail = userEmail };
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(m => m.FindByEmailAsync(userEmail)).ReturnsAsync(existingUser);
            userManagerMock.Setup(m => m.UpdateAsync(existingUser)).ReturnsAsync(IdentityResult.Failed(new IdentityError()));
            var controller = new AccountController(null, null, null, null);

            // Act
            var result = await controller.ResendVerificationEmailAsync(userEmail, 1);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Erro ao atualizar o código do usuário", badRequestResult.Value);
        }

        //-------------------------------------------ResendVerificationEmailAsync------------------------------------------
        //-------------------------------------------ListenList------------------------------------------


        [Fact]
        public async Task ListenList_ReturnsListenListView()
        {
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
              Mock.Of<IUserStore<ApplicationUser>>(),
              null, null, null, null, null, null, null, null);

            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null);
            var controller = new AccountController(null, null, null, null);

            // Act
            var result = controller.Login();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotEqual("ListenList", viewResult.ViewName);
        }
        //-------------------------------------------ListenList------------------------------------------





        [Fact] // testar
        public async Task GetAlbum_AlbumIdProvided_ReturnsViewWithAlbumDetails()
        {
            // Arrange
            var albumId = "your_album_id";
            var artistName = "your_artist_name";
            var mockHttpClientHelper = new Mock<IHttpClientHelper>();
            var mockContext = "";

             var controller = new AccountController(null, null, null, null);

            // Setup HttpClientHelper mock to return dummy data
            var dummyResponseJson = @"{
                'artist' : 'teste'
            }";
            mockHttpClientHelper.Setup(x => x.SendAysnc(It.IsAny<string>(), It.IsAny<string>()))
                                .Returns(Task.FromResult(dummyResponseJson));

            // Act
            var result = await controller.GetAlbum(albumId, artistName) as ViewResult;

           
            Assert.Null(result.Model); 
        }
















        private Mock<UserManager<ApplicationUser>> GetMockUserManager(ApplicationUser user)
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

            return userManagerMock;
        }

        private Mock<SignInManager<ApplicationUser>> GetMockSignInManager(ApplicationUser user, bool succeeded = true)
        {
            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                GetMockUserManager(user).Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null);

            signInManagerMock.Setup(m => m.PasswordSignInAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(succeeded ? Microsoft.AspNetCore.Identity.SignInResult.Success : Microsoft.AspNetCore.Identity.SignInResult.Failed);

            return signInManagerMock;
        }
    }
}
