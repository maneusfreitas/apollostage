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
using ApolloStage.Migrations;
using System.Drawing;
using Newtonsoft.Json;
using ApolloStage;

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
            List<Tshirt> tshirts = _context.Tshirt.ToList();
            List<Mug> mugs = _context.Mug.ToList();

            var viewModel = new MarketProductsModel
            {
                Tshirts = tshirts,
                Mugs = mugs
            };
            var userEmail = User.FindFirst(ClaimTypes.Email).Value;
            var admin = await _userManager.FindByEmailAsync(userEmail);
            TempData["meu"] = admin.Admin;
            return View("Index", viewModel);

        }
        [HttpGet]
        public async Task<IActionResult> addProducts()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email).Value;
            var admin = await _userManager.FindByEmailAsync(userEmail);
            TempData["meu"] = admin.Admin;
            return View("adminAddProduct");
        }
        [HttpGet]
        public IActionResult ProductDetails(int id)
        {

            var tshirt = _context.Tshirt.FirstOrDefault(t => t.Id == id);

            if (tshirt == null)
            {
                return RedirectToPage("index");
            }

            return View("ProductDetails",tshirt);
        }

        [HttpPost]
        public async Task<IActionResult> ShoppingCart(string cartContent)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email).Value;
            var OrderIdx = _context.ProductOrder.Max(p => (int?)p.OrderId) ?? 0;
            
                List<Cart> cartItems = JsonConvert.DeserializeObject<List<Cart>>(cartContent);

         
            int oid = OrderIdx + 1;
            foreach (var item in cartItems)
            {
                string nameWithoutHyphens = item.name.Replace("-", " ");

                // Dividir o nome em partes usando espaços como delimitador
                string[] nameParts = nameWithoutHyphens.Split(' ');
                string sizex = "";
                string colorx = "";

                // Verificar se existem pelo menos duas palavras na string
                if (nameParts.Length >= 2)
                {
                    // Guardar os dois últimos nomes em variáveis
                    colorx = nameParts[nameParts.Length - 1];
                    sizex = nameParts[nameParts.Length - 2];

                    // Remover as duas últimas palavras da lista
                    Array.Resize(ref nameParts, nameParts.Length - 2);
                }

                string finalName = string.Join(" ", nameParts);
                if (item.count > 0)
                {
                    var fprice = _context.Tshirt.First(p => p.Title == finalName);
                   
                    var product = new ApolloStage.Models.Product.ProductOrder
                    {
                        OrderId = oid,
                        UserMail = userEmail,
                        TshirtTitle = finalName,
                        TshirtSize = sizex,
                        TshirtColor = colorx,
                        TshirtCount = item.count,
                        TshirtPrice = fprice.Price,
                        state = "inexistente"
                    };

                    _context.ProductOrder.Add(product);
                }
            }
            await _context.SaveChangesAsync();
            return Json(new { id = oid, email = userEmail });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitTShirt(Tshirt model, List<IFormFile> Images)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var admin = await _userManager.FindByEmailAsync(userEmail);

            model.Images1 = Images.Count > 0 ? await SaveImage(Images[0]) : "null";
            model.Images2 = Images.Count > 1 ? await SaveImage(Images[1]) : "null";
            model.Images3 = Images.Count > 2 ? await SaveImage(Images[2]) : "null";
            model.Images4 = Images.Count > 3 ? await SaveImage(Images[3]) : "null";

            _context.Tshirt.Add(model);
            await _context.SaveChangesAsync();
            TempData["meu"] = admin?.Admin;
            return View("adminAddProduct");
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return null; 
            }

            var originalExtension = Path.GetExtension(image.FileName);
            var imageName = GenerateRandomName() + originalExtension;
            var imagePath = Path.Combine("img", imageName);
            var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath);

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return imagePath;
        }



        [HttpPost]
        public async Task<IActionResult> SubmitMug(string mugTitle, string mugColor, decimal mugPrice, string mugDescription, IFormFile mugImage)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email).Value;
            var admin = await _userManager.FindByEmailAsync(userEmail);

            if (mugImage != null && mugImage.Length > 0)
            {
                try
                {
                    var originalExtension = Path.GetExtension(mugImage.FileName);
                    var imageName = GenerateRandomName() + originalExtension;
                    var imagePath = Path.Combine("img", imageName);
                    var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath);

                    using (var stream = new FileStream(physicalPath, FileMode.Create))
                    {
                        await mugImage.CopyToAsync(stream);
                    }

                    var product = new Mug
                    {
                        title = mugTitle,
                        Color = mugColor,
                        Price = mugPrice,
                        Description = mugDescription,
                        Image = imagePath,
                        
                    };

                    _context.Mug.Add(product);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ocorreu um erro ao salvar a imagem: {ex.Message}");
                }
            }
            TempData["meu"] = admin.Admin;
            return View("adminAddProduct");
        }

        [HttpGet]
        public async Task<IActionResult> delivery(string id, string email)
        {
            TempData["id"] = id;
            TempData["email"] = email;
            return View("OrderDelivery");
        }

        [HttpPost]
        public async Task<IActionResult> SubmitDeliveryOrder([FromForm] ApplicationUser model)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email).Value;
            var userx = await _context.TempRegisterData.FirstOrDefaultAsync(u => u.Email == userEmail);

            Console.WriteLine("wrfwrtfwrt");
            Console.WriteLine(model.ConfirmoEnvio);
            Console.WriteLine(model.ConfirmoEnvio);
            Console.WriteLine("wrfwrtfwrt");

            if (model.ConfirmoEnvio)
                    {
                        var destinatario = model.NomeEntrega;
                        var moradaEntrega = model.MoradaEntrega;
                        var cidadeEntrega = model.CidadeEntrega;
                        var codigoPostalEntrega = model.CodigoPostalEntrega;
                        var paisEntrega = model.PaisEntrega;
                        var numeroTelEntrega = model.NumerotelEntrega;
                  //  if(destinatario!=null && destinatario != "" && moradaEntrega != null && moradaEntrega != "" && cidadeEntrega != null && cidadeEntrega != "" && codigoPostalEntrega != null && codigoPostalEntrega != "" && paisEntrega != null && paisEntrega != "" && numeroTelEntrega != null && numeroTelEntrega != "")
                    userx.NomeEntrega = destinatario;
                    userx.MoradaEntrega = moradaEntrega;
                    userx.CidadeEntrega = cidadeEntrega;
                    userx.CodigoPostalEntrega = codigoPostalEntrega;
                    userx.PaisEntrega = paisEntrega;
                    userx.NumerotelEntrega = numeroTelEntrega;
                    userx.ConfirmoEnvio = model.ConfirmoEnvio;

                  
                }

                    var morada = model.Morada;
                    var cidade = model.Cidade;
                    var codigoPostal = model.CodigoPostal;
                    var pais = model.Pais;
                    var numeroTel = model.Numerotel;
                    userx.Morada = morada;
                    userx.Cidade = cidade;
                    userx.CodigoPostal = codigoPostal;
                    userx.Pais = pais;
                    userx.Numerotel = numeroTel;
            var maxOrderId = await _context.ProductOrder.Where(u => u.UserMail == userEmail).MaxAsync(u => (int?)u.OrderId);

            if (maxOrderId.HasValue)
            {
                var ordersToUpdate = await _context.ProductOrder
                    .Where(u => u.UserMail == userEmail && u.OrderId == maxOrderId)
                    .ToListAsync();

                if (ordersToUpdate.Any())
                {
                    foreach (var order in ordersToUpdate)
                    {
                        order.state = "Pendente"; 
                    }

                }
            }

            await Task.Delay(100); 
            await _context.SaveChangesAsync();
            return View("OrderComplete");
                
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
