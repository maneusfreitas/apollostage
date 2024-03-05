using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApolloStage.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text;
using System.Reflection;
using ApolloStage.Models.Product;
using Microsoft.EntityFrameworkCore;
using ApolloStage.Data;
using System;
using ApolloStage.Migrations;

namespace ApolloStageFirst.Controllers
{
    public class MarketController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MarketController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;

        }

        [HttpGet]
        public async Task<IActionResult> market()
        {
            List<Product> products = _context.Product.ToList();
            return View("Index", products);

        }
        [HttpGet]
        public async Task<IActionResult> addProducts()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email).Value;
            var admin = await _userManager.FindByEmailAsync(userEmail);
            TempData["meu"] = admin.Admin;
            return View("adminAddProduct", "Market");
        }

        [HttpPost]
        public async Task<IActionResult> SubmitTShirt(string title, string color, string size, decimal price, string description, IFormFile image)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email).Value;
            var admin = await _userManager.FindByEmailAsync(userEmail);

            if (image != null && image.Length > 0)
            {
                try
                {
                    var originalExtension = Path.GetExtension(image.FileName);
                    var imageName = GenerateRandomName() + originalExtension;
                    var imagePath = Path.Combine("img", imageName); 
                    var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath); 

                    using (var stream = new FileStream(physicalPath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var product = new Product
                    {
                        title = title,
                        Color = color,
                        Size = size,
                        Price = price,
                        Description = description,
                        Image = imagePath
                    };

                    _context.Product.Add(product);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ocorreu um erro ao salvar a imagem: {ex.Message}");
                }
            }
            TempData["meu"] = admin.Admin;
            return View("adminAddProduct", "Market"); 
        }


        private string GenerateRandomName()
        {
            const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string digitChars = "0123456789";

            var random = new Random();
            var password = new StringBuilder();

           
            password.Append(uppercaseChars[random.Next(uppercaseChars.Length)]);
            password.Append(lowercaseChars[random.Next(lowercaseChars.Length)]);
            password.Append(digitChars[random.Next(digitChars.Length)]);

            const string allChars = uppercaseChars + lowercaseChars + digitChars;

           
            for (int i = 4; i < 34; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            var passwordArray = password.ToString().ToCharArray();
            var shuffledPassword = new string(passwordArray.OrderBy(x => random.Next()).ToArray());

            return shuffledPassword;
        }
    }
}
