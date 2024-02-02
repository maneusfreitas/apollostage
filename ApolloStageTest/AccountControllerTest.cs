using ApolloStage.Controllers;
using ApolloStage.Data;
using ApolloStage.Models;
using ApolloStageFirst.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace ApolloStageTest
{
    public class AccountControllerTest
    {
        [Fact]
        public async Task ResetPassword_ExistingUser_ReturnsResetPasswordConfirmationView()
        {
            // Configuração do contexto do banco de dados em memória
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;

            // Criação do contexto do banco de dados
            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureCreated();

                // Configuração do UserManager e SignInManager
                var userManager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(context),
                    null, null, null, null, null, null, null, null
                );

                var signInManager = new SignInManager<ApplicationUser>(
                    userManager,
                    new HttpContextAccessor(),
                    new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                    null,
                    null,
                    null,
                    null);

                // Adiciona um usuário de exemplo ao banco de dados em memória
                var user = new ApplicationUser
                {
                    Name = "testuser",
                    UserMail = "existing@example.com",
                    Email = "existing@example.com",
                    Password = "Password123@",
                    ConfirmPassword = "Password123@",
                    Code = "0",
                    ConfirmedEmail = false,
                };

                var result = await userManager.CreateAsync(user, "Password123@");

                Assert.True(result.Succeeded); // Verifica se o usuário foi criado com sucesso

                // Criação de uma instância do AccountController
                var controller = new AccountController(userManager, signInManager);

                // Chamada ao método ResetPassword no controlador
                var resetResult = await controller.ResetPassword(user);

                // Verificação se a View retornada é a esperada
                Assert.IsType<ViewResult>(resetResult);
                Assert.Equal("ForgotPasswordConfirmation", (resetResult as ViewResult).ViewName);
            }
        }



        [Fact]
        public async Task ResetPassword_NonExistingUser_ReturnsRedirectToAction()
        {
            // Arrange
            var user = new ApplicationUser { UserMail = "nonexisting@example.com" };
            var userManagerMock = GetMockUserManager(null);

            var controller = new AccountController(userManagerMock.Object, null);

            // Act
            var result = await controller.ResetPassword(user);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
        }

        private Mock<UserManager<ApplicationUser>> GetMockUserManager(ApplicationUser user)
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

            // Configure other necessary setup for ResetPasswordAsync, GeneratePasswordResetTokenAsync, etc.

            return userManagerMock;
        }
    }
}
