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
using System.Drawing;
using Newtonsoft.Json;
using ApolloStage;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using Stripe.FinancialConnections;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using SessionService = Stripe.Checkout.SessionService;
using System.Diagnostics.Metrics;
using System.Xml.Linq;

namespace ApolloStageFirst.Controllers
{
    public class MarketController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _stripeSecretKey;
        private readonly IConfiguration _configuration;


        public MarketController(IConfiguration configuration, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }
        [HttpGet]
        public async Task<IActionResult> Success()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email).Value;
            var maxOrderId = await _context.ProductOrder.Where(u => u.UserMail == userEmail).MaxAsync(u => (int?)u.OrderId);

            if (maxOrderId.HasValue)
            {
                var ordersToUpdates = await _context.ProductOrder
                    .Where(u => u.UserMail == userEmail && u.OrderId == maxOrderId)
                    .ToListAsync();

                if (ordersToUpdates.Any())
                {
                    foreach (var order in ordersToUpdates)
                    {
                        order.State = "Pago";
                    }
                }
            }
            await _context.SaveChangesAsync();
            return View("StripeSuccess");
        }

        [HttpGet]
        public async Task<IActionResult> CancelOrder()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email).Value;
            var maxOrderId = await _context.ProductOrder
                .Where(u => u.UserMail == userEmail)
                .MaxAsync(u => (int?)u.OrderId);

            var ordersToUpdates = await _context.ProductOrder
                .Where(u => u.UserMail == userEmail && u.OrderId == maxOrderId)
                .ToListAsync();

            int pointstoreplace = 0;
            foreach (var order in ordersToUpdates)
            {
                pointstoreplace = order.Pointstoapply;
                _context.ProductOrder.Remove(order);
            }
            var use = await _userManager.FindByEmailAsync(userEmail);
            use.points += pointstoreplace;
            _context.TempRegisterData.Update(use);
            await _context.SaveChangesAsync();

            var response = new
            {
                success = true 
            };

            return Json(response);
        }

        [HttpGet]
        public async Task<IActionResult> Cancel()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email).Value;
            var maxOrderId = await _context.ProductOrder.Where(u => u.UserMail == userEmail).MaxAsync(u => (int?)u.OrderId);

            if (maxOrderId.HasValue)
            {
                var ordersToUpdates = await _context.ProductOrder
                    .Where(u => u.UserMail == userEmail && u.OrderId == maxOrderId)
                    .ToListAsync();

                if (ordersToUpdates.Any())
                {
                    foreach (var order in ordersToUpdates)
                    {
                        order.State = "Cancelado";
                    }

                }
            }

            await _context.SaveChangesAsync();
            var ordersToUpdate = await _context.ProductOrder
                    .Where(u => u.UserMail == userEmail && u.OrderId == maxOrderId)
                    .ToListAsync();
            // return View("OrderComplete", ordersToUpdate);
            TempData["cancel"] = "A encomenda nº"+ maxOrderId + " foi cancelada caso pretenda tentar novamente clique em pagar novamente";
            return View("StripeCancel", ordersToUpdate);
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
            var user = await _userManager.FindByEmailAsync(userEmail);
            TempData["points"] = user.points;
            TempData["meu"] = user.Admin;
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
        public async Task<IActionResult> ProductDetails(int id)
        {

            var tshirt = _context.Tshirt.FirstOrDefault(t => t.Id == id);

            if (tshirt == null)
            {
                return RedirectToPage("index");
            }
            var userEmail = User.FindFirst(ClaimTypes.Email).Value;
            var admin = await _userManager.FindByEmailAsync(userEmail);
            var user = await _userManager.FindByEmailAsync(userEmail);
            TempData["points"] = user.points;
            TempData["meu"] = admin.Admin;
            return View("ProductDetails",tshirt);
        }
        [HttpPost]
       /* [HttpPost]
        public IActionResult ShoppingCart([FromBody] List<Cart> products)
        {
            if (products == null || !products.Any())
            {
                return BadRequest("Nenhum produto enviado");
            }

            foreach (var product in products)
            {
                Console.WriteLine($"Nome: {product.Name}, Preço: {product.Price}, Quantidade: {product.Count}, Tamanho: {product.Size}, Cor: {product.Color}, Imagem: {product.Image}, Descrição: {product.Pname}, Total: {product.Total}");
            }

            return NoContent();
        }*/


         [HttpPost]
         public async Task<IActionResult> ShoppingCart([FromBody] List<Cart> cartContent, int points)
         {
             var userEmail = User.FindFirst(ClaimTypes.Email).Value;
             var OrderIdx = _context.ProductOrder.Max(p => (int?)p.OrderId) ?? 0;

             Console.WriteLine("cartContent");
             Console.WriteLine("Cart Content:");
             foreach (var cartItem in cartContent)
             {
                 Console.WriteLine($"Product ID: {cartItem.Name}, Quantity: {cartItem.Count}");
                 // Se houver mais propriedades em Cart, você pode imprimi-las da mesma maneira
             }
             Console.WriteLine("cartContent");
             bool check = true;
             int oid = OrderIdx + 1;
             foreach (var item in cartContent)
             {
                 string nameWithoutHyphens = item.Name.Replace("-", " ");

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
                 if (item.Count > 0)
                 {

                     var fprice = _context.Tshirt.First(p => p.Title == finalName);
                     decimal finalprice = fprice.Price;
                     if (points > 1000 && check)
                     {
                         decimal p = fprice.Price;
                         finalprice= (p - (Math.Floor((decimal)points / 1000) / item.Count));
                         var user = await _userManager.FindByEmailAsync(userEmail);
                         user.points -= points;
                         _context.TempRegisterData.Update(user);
                         check = false;
                     }
                     var product = new ApolloStage.Models.Product.ProductOrder
                     {
                         OrderId = oid,
                         UserMail = userEmail,
                         TshirtTitle = finalName,
                         TshirtSize = sizex,
                         TshirtColor = colorx,
                         TshirtCount = item.Count,
                         TshirtPrice = finalprice,
                         State = "inexistente",
                         Pointstoapply = points
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
            var userEmail = User.FindFirst(ClaimTypes.Email).Value;
            var userx = await _context.TempRegisterData.FirstOrDefaultAsync(u => u.Email == userEmail);
            TempData["id"] = id;
            TempData["email"] = email;
            return View("OrderDelivery",userx);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitDeliveryOrder([FromForm] ApplicationUser model)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email).Value;
            var userx = await _context.TempRegisterData.FirstOrDefaultAsync(u => u.Email == userEmail);


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
                var ordersToUpdates = await _context.ProductOrder
                    .Where(u => u.UserMail == userEmail && u.OrderId == maxOrderId)
                    .ToListAsync();

                if (ordersToUpdates.Any())
                {
                    foreach (var order in ordersToUpdates)
                    {
                        order.State = "Pendente"; 
                    }

                }
            }

            await _context.SaveChangesAsync();
            var ordersToUpdate = await _context.ProductOrder
                    .Where(u => u.UserMail == userEmail && u.OrderId == maxOrderId)
                    .ToListAsync();
            return View("OrderComplete", ordersToUpdate);

        }
        [HttpPost]
        public async Task<IActionResult> ProcessPayment(string orderId, string userEmail, decimal amount)
        {
            try
            {
                var stripePublicKey = _configuration["Stripe:PubKey"];
                StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

                var lineItems = new List<SessionLineItemOptions>();
                var maxOrderId = await _context.ProductOrder.Where(u => u.UserMail == userEmail).MaxAsync(u => (int?)u.OrderId);
                var ordersToUpdates = await _context.ProductOrder
                                    .Where(u => u.UserMail == userEmail && u.OrderId == maxOrderId)
                                    .ToListAsync();
                foreach (var order in ordersToUpdates)
                {
                    var lineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "eur",
                            UnitAmount = (long)(order.TshirtPrice * 100), // Supondo que Price é o preço do produto
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = order.TshirtTitle, 
                                Description = "Tshirt Color: "+order.TshirtColor + " Tshirt Size:" + order.TshirtSize
                            }
                        },
                        Quantity = order.TshirtCount 
                    };

                    lineItems.Add(lineItem);
                }

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = lineItems,
                    Mode = "payment",
                    SuccessUrl = "https://localhost:7164/market/success",
                    CancelUrl = "https://localhost:7164/market/cancel"
                };

                var service = new SessionService();
                var session = service.Create(options);

                Console.WriteLine($"Session created: {session.Id}");

                // Obter o link de redirecionamento da sessão de checkout
                var checkoutUrl = session.Url;

                // Redirecionar o usuário para a página de checkout do Stripe
                return Redirect(checkoutUrl);
            }
            catch (StripeException e)
            {
                // Handle Stripe exceptions
                Console.WriteLine($"StripeException: {e.Message}");
                return BadRequest("Failed to process payment");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred");
            }
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
