using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using ApolloStage.Controllers;
using ApolloStage.Data;
using ApolloStage.Models;
using ApolloStageFirst.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ApolloStageTest
{
    public class MarketControllerTest
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private IWebHostEnvironment _webHostEnvironment;
        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new ApplicationDbContext(options);

            // Configure IConfiguration
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Stripe:PubKey", "pk_test_51Ouw5g040plG9RotFtnzZX5ZuylkUmaepSH7HlJ8r2Xxydpnbe6m02zEeROTabGZqhTJNQJNRZ7VsaubUJggK9Nq00r6ZARMiq" },
                    { "Stripe:SecretKey", "sk_test_51Ouw5g040plG9RotKa3ephwgHk0fG7rTIR3qNBSX0ULowLuYcpeCXRdF9ruE2XkNZg8EcG3GqNf2e4nJMR5RxzyB00hbCeP95c" }
                })
                .Build();

            // Mock UserManager
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object,
                null, null, null, null, null, null, null, null);

            // Mock SignInManager
            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object, null, null, null, null, null, null);

            _webHostEnvironment = new Mock<IWebHostEnvironment>().Object;
            _userManager = userManagerMock.Object;
            _signInManager = signInManagerMock.Object;
            _configuration = configuration;

            // Register a test user
            var user = new ApplicationUser
            {
                UserMail = "lisboabot@gmail.com",
                Email = "lisboabot@gmail.com",
                Name = "lisboabot",
                UserName = "lisboabot",
                Password = "Password123",
                ConfirmPassword = "Password123",
                DateOfBirth = DateTime.Now.AddYears(-30),
                Country = "Portugal",
                Gender = "Masculino",
                Code = "0",
                ConfirmedEmail = false,
                Admin = false,
                points = 0
            };

            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        }

        [Test]
        public async Task ProcessPayment_ReturnsRedirect()
        {
            // Arrange
            var controller = new MarketController(
                _configuration, _userManager, _signInManager, _context, _webHostEnvironment
            );

            // Act
            var result = await controller.ProcessPayment("1", "lisboabot@gmail.com", 10.99m) as RedirectResult;

            // Assert
            NUnit.Framework.Assert.IsNotNull(result);
            NUnit.Framework.Assert.AreEqual("https://apollostage20240303150613.azurewebsites.net/market/success", result.Url);

            // Example database access
            var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == "lisboabot@gmail.com");
            // Make assertions based on the database result, if necessary
        }
    }
}
