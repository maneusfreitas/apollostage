using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApolloStage.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text;
using ApolloStage;
using ApolloStage.Services;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using ApolloStage.Data;
using Newtonsoft.Json.Linq;
using ApolloStage.Models.Extra;

namespace ApolloStageFirst.Controllers
{
    public class AccountController : Controller
    {
        private readonly string baseUrl = "https://api.spotify.com/v1";
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly HttpCLientHelper _httpClientHelper;
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, HttpCLientHelper httpClientHelper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpClientHelper = httpClientHelper;
            _context = context;
        }



        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpGet]
        public async Task<IActionResult> profile()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email);
            var existingUser = _userManager.FindByEmailAsync(userEmail.Value).Result;

            // var getuseralbums = _context.AlbumRatings.Where(t => t.userEmail == userEmail.Value).ToList();
            var top50Albums = _context.AlbumRatings.Where(t => t.userEmail == userEmail.Value)
                  .Take(50)
                  .Select(t => t.albumId)
                  .ToList();
            // Agrupar em lotes de 10
            var albumGroups = top50Albums
                .Select((value, index) => new { value, index })
                .GroupBy(x => x.index / 10)
                .Select(group => group.Select(x => x.value).ToList())
                .ToList();

            // Consultar a API do Spotify para cada lote de 10
            var albums = new List<Album>();
            foreach (var group in albumGroups)
            {
                // Concatenar os IdAlbum em uma única string separada por vírgulas
                string rAlbums = string.Join(",", group);

                // Consultar a API do Spotify
                string urlnew = "https://api.spotify.com/v1/albums?ids=" + rAlbums;
                Console.WriteLine("token: " + SpotifyService.AccessToken);
                Console.WriteLine("albuns: " + rAlbums);
                var responseJsonw = await _httpClientHelper.SendAysnc(urlnew, SpotifyService.AccessToken);

                var responseJObjectw = JObject.Parse(responseJsonw);
                var albumsInGroup = responseJObjectw["albums"].ToObject<List<Album>>();

                albums.AddRange(albumsInGroup);
            }


            var albumList = albums.Select(albumItem => new Album
            {
                album_type = albumItem.album_type,
                artists = albumItem.artists,
                available_markets = albumItem.available_markets,
                external_urls = albumItem.external_urls,
                href = albumItem.href,
                id = albumItem.id,
                images = albumItem.images,
                name = albumItem.name,
                release_date = albumItem.release_date,
                release_date_precision = albumItem.release_date_precision,
                type = albumItem.type,
                uri = albumItem.uri,
                label = albumItem.label,
                popularity = albumItem.popularity,
            }).Reverse().ToList();

            var allAlbums = _context.ListenList
                     .Where(a => a.UserMail == userEmail.Value)
                     .Select(a => a.AlbumId)
                     .ToList();
            List<Album> albumsWithDetails = new List<Album>();
            Album albumDetails; // Mova a declaração para fora do loop

            foreach (var favoriteAlbum in allAlbums)
            {
                var albumDetailsUrl = $"https://api.spotify.com/v1/albums/{favoriteAlbum}";
                var responseJson = await _httpClientHelper.SendAysnc(albumDetailsUrl, SpotifyService.AccessToken);
                albumDetails = JsonConvert.DeserializeObject<Album>(responseJson);

                // Aqui você pode adicionar albumDetails a uma lista, se necessário
                albumsWithDetails.Add(albumDetails);
            }
            var reviewUser = _context.AlbumReview
                    .Where(a => a.UserMail == userEmail.Value).Count();
            var reviewTotal = _context.AlbumReview.Count();

            var recommendedAlbum = _context.AlbumReview
                    .Where(a => a.UserMail == userEmail.Value && a.reviewRecommendation == "Recomendo").Count();
            var notRecommendedAlbum = _context.AlbumReview
                    .Where(a => a.UserMail == userEmail.Value && a.reviewRecommendation != "Recomendo").Count();

            var nratingUser = _context.AlbumRatings
                 .Where(a => a.userEmail == userEmail.Value).Count();
            var nratingTotal = _context.AlbumRatings.Count();

            var genderChart = _context.AlbumRatings.Where(a=> a.userEmail == userEmail.Value)
             .GroupBy(a => a.gender)
             .ToDictionary(g => g.Key, g => g.Count());

            var viewModel = new ProfileViewModel
            {
                User = existingUser,
                Album = albumList,
                Listenslist = albumsWithDetails,
                NReviwesUser = reviewUser,
                NReviwesTotal = reviewTotal,
                NRecommendedalbum= recommendedAlbum,
                NNotRecommendedalbum = notRecommendedAlbum,
                NTotalRatings= nratingTotal,
                NUserRatings = nratingUser,
                GenderChart = genderChart,

            };
            TempData["limite"] = albums.Count();

            return View("Profile", viewModel);
        }


        [HttpGet]
        public IActionResult LimitAttempts()
        {
            return View("LimitAttempts");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("FirstRegister");
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }
        [HttpGet]
        public async Task<IActionResult> dashboard()
        {
            var userEmailClaim = User.FindFirst(ClaimTypes.Email);
            var admin = await _userManager.FindByEmailAsync(userEmailClaim.Value);
            TempData["meu"] = admin.Admin;
            TempData["name"] = admin.Name;
            var prodcount = _context.ProductOrder
                     .Where(item => item.State == "Pago")
                     .Sum(item =>  item.TshirtCount);
            var prodRating = _context.AlbumRatings.Count();
            var prodproduct = _context.Tshirt.Count();
            var prodemail = _context.TempRegisterData.Count();
            var prodrev = _context.AlbumReview.Count();
            var prodlistnlist = _context.ListenList.Count();
            var somaCountMails = _context.TempRegisterData.Sum(item => item.CountMails);
            var reports = _context.ReviewReports.Where(a => a.revchecked == false).OrderByDescending(a => a.count).ToList();
            var orderbystatus = _context.ProductOrder
       .Where(a => a.State != "inexistente")
       .GroupBy(a => a.State)
       .Select(g => new
       {
           State = g.Key,
           Count = g.Sum(item => item.TshirtCount)
       }).ToList<dynamic>();


            var orders = _context.ProductOrder
                     .Where(item => item.State == "Pago")
                     .ToList();

            var balance = orders.Sum(item => item.TshirtPrice * item.TshirtCount);


            DateTime today = DateTime.Today;
            DayOfWeek currentDayOfWeek = today.DayOfWeek;
            DateTime startOfWeek = today.AddDays(-(int)currentDayOfWeek + (int)DayOfWeek.Monday);
            DateTime endOfWeek = startOfWeek.AddDays(6);


            var allDaysOfWeek = new List<DayOfWeek>
                {
                    DayOfWeek.Monday,
                    DayOfWeek.Tuesday,
                    DayOfWeek.Wednesday,
                    DayOfWeek.Thursday,
                    DayOfWeek.Friday,
                    DayOfWeek.Saturday,
                    DayOfWeek.Sunday
                };

            var weeksales = _context.ProductOrder
          .Where(a => a.data >= startOfWeek && a.data <= endOfWeek && a.State == "Pago")
          .GroupBy(a => a.data.DayOfWeek)
          .Select(g => new
          {
              DayOfWeek = g.Key,
              TotalQuantity = g.Sum(item => (double)item.TshirtCount),
              TotalTshirtPrice = g.Sum(item => (double)item.TshirtCount * (double)item.TshirtPrice)  
          })
          .ToList();

            var salesByDayOfWeek = weeksales.ToDictionary(ws => ws.DayOfWeek, ws => ws);


            foreach (var missingDay in allDaysOfWeek.Except(salesByDayOfWeek.Keys))
            {
                salesByDayOfWeek.Add(missingDay, new
                {
                    DayOfWeek = missingDay,
                    TotalQuantity = 0.0,
                    TotalTshirtPrice = 0.0
                });
            }

            var orderedWeekSales = salesByDayOfWeek
                .OrderBy(pair => pair.Key)
                .Select(pair => new
                {
                    DayOfWeek = $"{GetDayAbbreviation(pair.Key)}({startOfWeek.AddDays((int)pair.Key).Day})", 
                    pair.Value.TotalQuantity,
                    pair.Value.TotalTshirtPrice
                })
                .ToList<dynamic>();



            foreach (var sale in orderedWeekSales)
            {
                Console.WriteLine($"DayOfWeek: {sale.DayOfWeek}, Total Quantity: {sale.TotalQuantity}, Total Tshirt Price: {sale.TotalTshirtPrice}");
            }


            var existingRatings = _context.AlbumRatings.Select(a => a.starRating).Distinct().ToList();
            var allRatings = Enumerable.Range(1, 10).ToList();
            var avg = allRatings.Select(rating =>
            {
                var ratingExists = existingRatings.Contains(rating);
                var count = ratingExists ? _context.AlbumRatings.Count(a => a.starRating == rating) : 0;
                return new { StarRating = rating, Count = count };
            }).ToList<dynamic>();


            foreach (var group in avg)
            {
                Console.WriteLine($"Star Rating: {group.StarRating}, Count: {group.Count}");
            }


            var genderChart = _context.AlbumRatings
               .GroupBy(a => a.gender)
               .ToDictionary(g => g.Key, g => g.Count());


            var viewModel = new Dashboard
            {
               TotalEmail= somaCountMails,
               TotalListenList = prodlistnlist,
               TotalProducts= prodproduct,
               TotalProductSales = prodcount,
               TotalRatings= prodRating,
               TotalReviews= prodrev,
               TotalUsers= prodemail,
               Reports = reports,
               balance = balance,
               ordercoutstatus = orderbystatus,
               orders = orderedWeekSales,
               averageRatings = avg,
                GenderChart = genderChart

            };
            return View("Dashboard", viewModel);
        }

        private static string GetDayAbbreviation(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return "Mon";
                case DayOfWeek.Tuesday:
                    return "Tue";
                case DayOfWeek.Wednesday:
                    return "Wed";
                case DayOfWeek.Thursday:
                    return "Thu";
                case DayOfWeek.Friday:
                    return "Fri";
                case DayOfWeek.Saturday:
                    return "Sat";
                case DayOfWeek.Sunday:
                    return "Sun";
                default:
                    return string.Empty; // Retornar vazio para valores inválidos
            }
        }

        [HttpGet]
        public async Task<IActionResult> removeReview(int reviewId)
        {
            var userEmailClaim = User.FindFirst(ClaimTypes.Email);
            var admin = await _userManager.FindByEmailAsync(userEmailClaim.Value);
            if (admin.Admin)
            {
                var reviewReportToRemove = await _context.ReviewReports.FindAsync(reviewId);
                if (reviewReportToRemove != null)
                {
                    _context.ReviewReports.Remove(reviewReportToRemove);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }

        [HttpGet]
        public async Task<IActionResult> aproveReview(int reviewId)
        {
            var userEmailClaim = User.FindFirst(ClaimTypes.Email);
            var admin = await _userManager.FindByEmailAsync(userEmailClaim.Value);
            if (admin.Admin)
            {
                var reviewReport = await _context.ReviewReports.FindAsync(reviewId);
                if (reviewReport != null)
                {
                    reviewReport.revchecked = true;
                    _context.ReviewReports.Update(reviewReport);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> Login(ApplicationUser model)
        {
            var user = await _userManager.FindByEmailAsync(model.UserMail);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> FirstRegister(ApplicationUser model)
        {
            try
            {
                var tempRegisterData = new ApplicationUser
                {
                    UserMail = model.UserMail,
                    Name = "",
                    Password = "",
                    ConfirmPassword = "",
                    DateOfBirth = DateTime.Now,
                    Country = "",
                    Gender = "",
                    Code = "0",
                    ConfirmedEmail = false,
                    Admin = false,
                    points = 0,
                    CountMails=0,
                };
                return View("SecondRegister", tempRegisterData);
            }

            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocorreu um erro ao salvar os dados: " + ex.Message);
                return View(model);
            }

        }


        [HttpPost]
        public async Task<IActionResult> SecondRegister(ApplicationUser model)
        {

            try
            {

                TempData["Password"] = model.Password;
                TempData["ConfirmPassword"] = model.ConfirmPassword;
                TempData["Name"] = model.Name;

                var tempRegisterData = new ApplicationUser
                {
                    UserMail = model.UserMail,
                    Name = model.Name,
                    DateOfBirth = DateTime.Now,
                    Country = "",
                    Gender = "",
                    Code = "0",
                    ConfirmedEmail = false,
                    Admin = false,
                    points=0,
                    CountMails = 0,
                };

                return View("ThirdRegister", tempRegisterData);
            }

            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocorreu um erro ao salvar os dados: " + ex.Message);
                return View(model);
            }

        }



        [HttpPost]
        public async Task<IActionResult> ThirdRegister(ApplicationUser model)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(model.UserMail);

                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Este email já está em uso. Por favor, escolha outro.");
                    return View(model);
                }
                TempData["Email"] = model.UserMail;
                var applicationUser = new ApplicationUser
                {
                    UserMail = model.UserMail,
                    Email = model.UserMail,
                    Name = model.Name,
                    UserName = model.Name,
                    Password = model.Password,
                    ConfirmPassword = model.ConfirmPassword,
                    DateOfBirth = model.DateOfBirth,
                    Country = model.Country,
                    Gender = model.Gender,
                    Code = "0",
                    ConfirmedEmail = false,
                    Admin= false,
                    points=0,
                    CountMails = 0,
                };

                // Gerar um código aleatório de 4 numeros
                var random = new Random();
                var code = new string(Enumerable.Repeat("0123456789", 4)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                applicationUser.Code = code;
                var result = await _userManager.CreateAsync(applicationUser, model.Password);

                if (result.Succeeded)
                {

                    EmailService emailService = new EmailService();
                    await emailService.SendCodeByEmailAsync(model.UserMail, code);
                    applicationUser.CountMails += 1;
                    _context.TempRegisterData.Update(applicationUser);
                    _context.SaveChanges();
                    return View("CodeConfim", applicationUser);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocorreu um erro ao salvar os dados: " + ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> VerifyCodeAsync(ApplicationUser model, string UserMail, string code1, string code2, string code3, string code4, string pass = "normal")
        {
            if (pass == "normal" || pass == "pass")
            {
                try
                {

                    var user = await _userManager.FindByEmailAsync(UserMail);

                    if (user == null)
                    {
                        return RedirectToAction("UserNotFound");
                    }

                    string combinedCode = code1 + code2 + code3 + code4;

                    if (user.Code == combinedCode)
                    {
                        user.ConfirmedEmail = true;
                        user.EmailConfirmed = true;

                        var updateResult = await _userManager.UpdateAsync(user);

                        if (updateResult.Succeeded)
                        {
                            if (pass == "normal")
                            {
                                TempData["successAccount"] = "Account created successfully, try to login NOW";
                                return RedirectToAction("Login", "Account");
                            }
                            else if (pass == "pass")
                                return View("PasswordChangeCompleted", model);
                            else return RedirectToAction("Error", "Home");
                        }
                        else
                        {
                            return RedirectToAction("Error");
                        }
                    }
                    else
                    {
                        var deleteResult = await _userManager.DeleteAsync(user);
                        if (deleteResult.Succeeded)
                        {
                            return View("InvalidCode");
                        }
                        else
                        {
                            return BadRequest("Erro ao excluir o usuário");
                        }

                    }
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error");
                }
            }
            else
                return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ApplicationUser user)
        {

            if (user != null)
            {
                // Obtém o usuário do banco de dados com base no e-mail
                var existingUser = await _userManager.FindByEmailAsync(user.UserMail);

                if (existingUser != null)
                {
                    // Gera o token de redefinição de senha
                    var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);

                    // Tenta redefinir a senha
                    var result = await _userManager.ResetPasswordAsync(existingUser, token, user.Password);

                    if (result.Succeeded)
                    {
                        TempData["successPassword"] = "Password updated successfully, try logging in NOW";
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        // Se houver falha, você pode lidar com os erros aqui
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    // Usuário não encontrado
                    ModelState.AddModelError(string.Empty, "Usuário não encontrado");
                }
            }

            // Redireciona para uma página de erro em caso de falha
            return RedirectToAction("Error", "Home");
        }




        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {

            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }


        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                return View("Error");
            }

            var result = await HttpContext.AuthenticateAsync("Identity.External");
            var externalEmail = result?.Principal?.FindFirstValue(ClaimTypes.Email);
            var externalFirstName = result?.Principal?.FindFirstValue(ClaimTypes.GivenName);

            if (externalEmail == null)
            {
                return View("Error");
            }

            var user = await _userManager.FindByEmailAsync(externalEmail);

            if (user == null)
            {
                var newUser = new ApplicationUser
                {
                    UserMail = externalEmail,
                    Email = externalEmail,
                    UserName = externalFirstName,
                    Name = externalFirstName,
                    Code = "0",
                    ConfirmedEmail = true,
                    Admin = false,
                    points = 0,
                    CountMails = 0,
                };


                var creationResult = await _userManager.CreateAsync(newUser);

                if (creationResult.Succeeded)
                {
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View("Error");
                }
            }
            else
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ApplicationUser user)
        {
            var existingUser = await _userManager.FindByEmailAsync(user.UserMail);

            if (existingUser != null)
            {
                var random = new Random();
                var code = new string(Enumerable.Repeat("0123456789", 4)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                existingUser.Code = code;
                var updateResult = await _userManager.UpdateAsync(existingUser);

                if (updateResult != null && updateResult.Succeeded)
                {
                    EmailService emailService = new EmailService();
                    await emailService.SendCodeByEmailAsync(existingUser.UserMail, code);
                    existingUser.CountMails += 1;
                    _context.TempRegisterData.Update(existingUser);
                    _context.SaveChanges();
                    return View("CodeConfimPassword", existingUser);
                }
            }

            TempData["fail"] = "E-mail não encontrado";
            return View("ForgotPassword", user);
        }

        [HttpPost]
        public async Task<IActionResult> ResendVerificationEmailAsync(string userEmail, int n)
        {
            n++;
            var random = new Random();
            var code = new string(Enumerable.Repeat("0123456789", 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            var existingUser = await _userManager.FindByEmailAsync(userEmail);

            if (existingUser != null)
            {
                existingUser.Code = code;
                var updateResult = await _userManager.UpdateAsync(existingUser);

                if (updateResult.Succeeded)
                {
                    EmailService emailService = new EmailService();
                    await emailService.SendCodeByEmailAsync(userEmail, code);
                    existingUser.CountMails += 1;
                    _context.TempRegisterData.Update(existingUser);
                    _context.SaveChanges();
                    if (n <= 0)
                    {
                        var deleteResult = await _userManager.DeleteAsync(existingUser);
                        if (deleteResult.Succeeded)
                        {
                            return Ok("Usuário excluído com sucesso");
                        }
                        else
                        {
                            return BadRequest("Erro ao excluir o usuário");
                        }
                    }
                    return Ok();
                }
                else
                {
                    return BadRequest("Erro ao atualizar o código do usuário");
                }
            }

            return NotFound("Usuário não encontrado");
        }




        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }



        [HttpPost]
        public async Task<IActionResult> AdicionarAlistenList(string albumId, string albumName)
        {
            var userEmailClaim = User.FindFirst(ClaimTypes.Email);

            if (userEmailClaim == null)
            {
                return RedirectToAction("Login");
            }

            var userEmail = userEmailClaim.Value;

            var existingUser = await _userManager.FindByEmailAsync(userEmail);

            if (existingUser == null)
            {
                // Lidar com o caso em que o usuário não é encontrado
                return RedirectToAction("Login");
            }

            var albumExistente = _context.ListenList.FirstOrDefault(a => a.UserMail == userEmail && a.AlbumId == albumId);
           
            var user = _context.TempRegisterData.FirstOrDefault(r => r.UserMail == userEmail);
            if (user != null)
            {
                user.points++;
                _context.TempRegisterData.Update(user);
                _context.SaveChanges();

            }

            if (albumExistente == null)
            {
                existingUser.FavoriteAlbum.Add(new ListenList { UserMail = userEmail, AlbumId = albumId });

                await _userManager.UpdateAsync(existingUser);
                var registroExistente = _context.Top50.FirstOrDefault(t => t.IdAlbum == albumId);

                if (registroExistente != null)
                {
                    registroExistente.count++;
                }
                else
                {
                    var novoRegistro = new Top50 { IdAlbum = albumId, count = 1 };
                    _context.Top50.Add(novoRegistro);
                }

                _context.SaveChanges();
                return Json(new { success = true, errorMessage = "O álbum " + albumName + " foi adicionado à sua ListenList." });
            }
            else
            {
               

                _context.SaveChanges();
                return Json(new { success = false, errorMessage = "O álbum " + albumName + " foi removido da sua ListenList." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoverDosFavoritos(string albumId)
        {
            var userEmailClaim = User.FindFirst(ClaimTypes.Email);
            var admin = await _userManager.FindByEmailAsync(userEmailClaim.Value);

            string username = "";
            if (userEmailClaim == null)
            {
                return RedirectToAction("Login");
            }

            var userEmail = userEmailClaim.Value;


            var user = _context.TempRegisterData.FirstOrDefault(r => r.UserMail == userEmail);
            if (user != null)
            {
                if (user.points > 0)
                {
                    user.points -= 1;
                    _context.TempRegisterData.Update(user);
                    _context.SaveChanges();
                }
            }
        
            var existingUser = await _userManager.FindByEmailAsync(userEmail);

            if (existingUser == null)
            {

                return RedirectToAction("Login");
            }
            username = existingUser.Name;
            // Verifica se o álbum existe na lista de favoritos do usuário
            var albumExistente = _context.ListenList.FirstOrDefault(a => a.AlbumId == albumId);

            if (albumExistente != null)
            {
                var registroExistente = _context.Top50.FirstOrDefault(t => t.IdAlbum == albumId);

                if (registroExistente != null)
                {
                    registroExistente.count--;
                    _context.Top50.Update(registroExistente);

                }
                _context.ListenList.Remove(albumExistente);
                await _context.SaveChangesAsync();

            }
            else
            {
                 var registroExistente = _context.Top50.FirstOrDefault(t => t.IdAlbum == albumId);

                if (registroExistente != null)
                {
                    registroExistente.count++;
                }
                else
                {
                    var novoRegistro = new Top50 { IdAlbum = albumId, count = 1 };
                    _context.Top50.Add(novoRegistro);
                }
            }

            var allAlbums = _context.ListenList
                    .Where(a => a.UserMail == userEmail)
                    .Select(a => a.AlbumId)
                    .ToList();
            List<Album> albumsWithDetails = new List<Album>();
            Album albumDetails; // Mova a declaração para fora do loop

            foreach (var favoriteAlbum in allAlbums)
            {
                var albumDetailsUrl = $"https://api.spotify.com/v1/albums/{favoriteAlbum}";
                var responseJson = await _httpClientHelper.SendAysnc(albumDetailsUrl, SpotifyService.AccessToken);
                albumDetails = JsonConvert.DeserializeObject<Album>(responseJson);

                // Aqui você pode adicionar albumDetails a uma lista, se necessário
                albumsWithDetails.Add(albumDetails);
            }
            string formattedUsername = char.ToUpper(username[0]) + username.Substring(1).ToLower().Replace(" ", "");
            TempData["username"] = formattedUsername;



            TempData["meu"] = admin.Admin;
            return View("ListenList", albumsWithDetails);
        }



        [HttpPost]
        public async Task<IActionResult> RemoverDosFavoritosInAlbum(string albumId)
        {
            var userEmailClaim = User.FindFirst(ClaimTypes.Email);
            string username = "";

            if (userEmailClaim == null)
            {
                return Json(new { success = false });
            }

            var userEmail = userEmailClaim.Value;

            var existingUser = await _userManager.FindByEmailAsync(userEmail);

            if (existingUser == null)
            {

                return Json(new { success = false });
            }
            username = existingUser.Name;
            // Verifica se o álbum existe na lista de favoritos do usuário
            var albumExistente = _context.ListenList.FirstOrDefault(a => a.AlbumId == albumId);

            if (albumExistente != null)
            {
                

                _context.ListenList.Remove(albumExistente);
                await _context.SaveChangesAsync();

            }
            else
            {
                // Adicione logs para depuração
                Console.WriteLine($"Álbum não encontrado na lista de favoritos: {albumId}");
                return Json(new { success = false });
            }

            var allAlbums = _context.ListenList
                    .Where(a => a.UserMail == userEmail)
                    .Select(a => a.AlbumId)
                    .ToList();
            List<Album> albumsWithDetails = new List<Album>();
            Album albumDetails; // Mova a declaração para fora do loop

            foreach (var favoriteAlbum in allAlbums)
            {
                var albumDetailsUrl = $"https://api.spotify.com/v1/albums/{favoriteAlbum}";
                var responseJson = await _httpClientHelper.SendAysnc(albumDetailsUrl, SpotifyService.AccessToken);
                albumDetails = JsonConvert.DeserializeObject<Album>(responseJson);

                // Aqui você pode adicionar albumDetails a uma lista, se necessário
                albumsWithDetails.Add(albumDetails);
            }
            string formattedUsername = char.ToUpper(username[0]) + username.Substring(1).ToLower().Replace(" ", "");
            TempData["username"] = formattedUsername;

            return Json(new { success = true });
        }


        [HttpGet]
        public async Task<IActionResult> ListenList()
        {
            var userEmailClaim = User.FindFirst(ClaimTypes.Email);
            string username = "";
            if (userEmailClaim == null)
            {
                return RedirectToAction("Login");
            }

            var userEmail = userEmailClaim.Value;
            var existingUser = _userManager.FindByEmailAsync(userEmail).Result;

            if (existingUser == null)
            {
                return RedirectToAction("Login");
            }
            username = existingUser.Name;

            var allAlbums = _context.ListenList
                     .Where(a => a.UserMail == userEmail)
                     .Select(a => a.AlbumId)
                     .ToList();
            List<Album> albumsWithDetails = new List<Album>();
            Album albumDetails; // Mova a declaração para fora do loop
            string durationSeconds = "";
            double totalDuration = 0;
            int i = 0;
            foreach (var favoriteAlbum in allAlbums)
            {
                var albumDetailsUrl = $"https://api.spotify.com/v1/albums/{favoriteAlbum}";
                var responseJson = await _httpClientHelper.SendAysnc(albumDetailsUrl, SpotifyService.AccessToken);
                var albumDetailss = JsonConvert.DeserializeObject<Album>(responseJson);
               
                // Aqui você pode adicionar albumDetails a uma lista, se necessário
                albumsWithDetails.Add(albumDetailss);

                // Obtém as faixas do álbum
                var albumTracksUrl = $"https://api.spotify.com/v1/albums/{favoriteAlbum}/tracks";
                var responseTracksJson = await _httpClientHelper.SendAysnc(albumTracksUrl, SpotifyService.AccessToken);
                var albumTracks = JsonConvert.DeserializeObject<AlbumTracks>(responseTracksJson);

                var artist = $"https://api.spotify.com/v1/artists/" + albumTracks.items[0].artists[0].id;
                var responseJsonartist = await _httpClientHelper.SendAysnc(artist, SpotifyService.AccessToken);
                dynamic responseJsonArtist = JsonConvert.DeserializeObject(responseJsonartist);

                string name = responseJsonArtist.name;
                string artistimage = responseJsonArtist.images[0].url;
                var genresList = new List<string>();
                var genresArray = (JArray)responseJsonArtist.genres;
                foreach (var genre in genresArray)
                {
                    var genreString = (string)genre;
                    albumsWithDetails[i].Genres.Add(genreString);
                    Console.WriteLine("...ll...");
                    Console.WriteLine(genreString);
                    Console.WriteLine("...ll...");
                }
                foreach (var track in albumTracks.items)
                {
                    totalDuration += track.duration_ms; // Convertendo para minutos
              
                }
                i++;
                TimeSpan t = TimeSpan.FromMilliseconds(totalDuration);
                durationSeconds = string.Format("{0:D2}h:{1:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds);
                albumDetailss.duration_ms_album = durationSeconds;
            }

           


            foreach (var album in albumsWithDetails)
            {
               var rating = _context.AlbumRatings.Where(r => r.albumId == album.id)
                                                            .GroupBy(r => r.albumId)
                                                            .Select(g => new
                                                            {
                                                                AlbumId = g.Key,
                                                                AverageRating = g.Average(r => r.starRating)
                                                            })
                                                            .FirstOrDefault();
           
                if (rating != null)
                {
                    album.classificacaoEspecifica = (int)rating.AverageRating;
                }
                else
                {
                    album.classificacaoEspecifica = 0; 
                }
            }

           

            string formattedUsername = char.ToUpper(username[0]) + username.Substring(1).ToLower().Replace(" ", "");
            TempData["username"] = formattedUsername;
            if(albumsWithDetails.Count >=1)
            TempData["artistName"] = albumsWithDetails[0].artists[0].name;
            return View("ListenList", albumsWithDetails);
        }
        [HttpGet]
        public async Task<IActionResult> SearchArtistsByGenre(string selectedGenre)
        {
            var artists = await SearchArtistsByGenreAsync(selectedGenre, 6);

            return View("ArtistsByGenre", artists);
        }

        private async Task<List<Album>> SearchArtistsByGenreAsync(string genre, int limit)
        {
            string country = "US"; // Substitua pelo país desejado
                                   // incompleto ????
            string urlnew = $"https://api.spotify.com/v1/browse/new-releases?country={country}&limit={limit}&genre=rock";

            var responseNewReleases = await _httpClientHelper.SendAysnc(urlnew, SpotifyService.AccessToken);
            var jsonNewReleases = responseNewReleases;

            var jsonObjectNewReleases = JObject.Parse(jsonNewReleases);
            var itemsNewReleases = jsonObjectNewReleases["albums"]["items"];

            var newReleases = itemsNewReleases
                .Select(item => new Album
                {
                    album_type = (string)item["album_type"],
                    artists = item["artists"].ToObject<List<Artist>>(),
                    available_markets = item["available_markets"].ToObject<List<string>>(),
                    external_urls = new ExternalUrls { spotify = (string)item["external_urls"]["spotify"] },
                    href = (string)item["href"],
                    id = (string)item["id"],
                    images = ((JArray)item["images"]).ToObject<List<ApolloStage.DTO.Image>>(),
                    name = (string)item["name"],
                    release_date = (string)item["release_date"],
                    release_date_precision = (string)item["release_date_precision"],
                    type = (string)item["type"],
                    uri = (string)item["uri"],
                    classificacaoEspecifica = 0,
                    label = (string)item["label"],
                    popularity = (string)item["popularity"],
                })
                .ToList();

            return newReleases;
        }

        [HttpPost]
        public async Task<IActionResult> SaveAlbumRating(Classification classification)
        {
            if (classification.userEmail != "" && classification.albumId != "")
            {
                var userEmail = User.FindFirst(ClaimTypes.Email).Value;

                var existingRating = _context.AlbumRatings
                                          .FirstOrDefault(r => r.userEmail == userEmail && r.albumId == classification.albumId);

                if (existingRating != null)
                {
                    existingRating.starRating = classification.starRating;
                     _context.SaveChanges();

                    return Json(new { success = true, message = "Classificação do album Atualizada" });
                }

                Console.WriteLine(classification.gender);
                Console.WriteLine(classification.gender); Console.WriteLine(classification.gender);
                var albumRating = new Classification
                {
                    userEmail = userEmail,
                    albumId = classification.albumId,
                    starRating = classification.starRating,
                    gender = classification.gender
                };
               
                _context.AlbumRatings.Add(albumRating);
                _context.SaveChanges();

             

                return Json(new { success = true }); // Resposta JSON indicando sucesso
            }
            else
                return Json(new { success = false }); // Resposta JSON indicando sucesso
        }



        [HttpPost]
        public async Task<IActionResult> RemoveAlbumRating(string albumId, string gender)
        {
            if (albumId != "" && albumId != "")
            {
                var userEmail = User.FindFirst(ClaimTypes.Email).Value;

                var existingRating = _context.AlbumRatings
                                          .FirstOrDefault(r => r.userEmail == userEmail && r.albumId == albumId);

                if (existingRating != null)
                {
                  
                    _context.Remove(existingRating);
                    _context.SaveChanges();

                    return Json(new { success = true, message = "Classificação Removida" });
                }

                return Json(new { success = false });
            }
            else
                return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult reportComment(string reviewId, string userEmail, string motivo, string descricao)
        {
            if (reviewId != "" && reviewId != null && motivo != "" && descricao != "")
            {
                var rev = _context.ReviewReports.FirstOrDefault(r => r.IdReview == reviewId);
                if (rev == null)
                {
                    Console.WriteLine(motivo);
                    var reviewReports = new ReviewReports
                    {
                        IdReview = reviewId,
                        IdUserMail = userEmail,
                        causa = motivo,
                        descricao = descricao,
                        count = 0,
                    };
                    _context.ReviewReports.Add(reviewReports);
                    _context.SaveChanges();
                    return Json(new { success = true });
                }
                else
                {
                    rev.count++; 
                    _context.ReviewReports.Update(rev);
                    _context.SaveChanges();
                    return Json(new { success = false, msg = "já foi reportado por um utilizador" });
                }
            }
            else
                return Json(new { success = false });
        }




        [HttpGet]
        public async Task<IActionResult> GetAlbum(string albumId, string artistname)
        {
            if (!string.IsNullOrEmpty(albumId))
            {
                var userEmail = User.FindFirst(ClaimTypes.Email).Value;
                var admin = await _userManager.FindByEmailAsync(userEmail);

                var rating = _context.AlbumRatings.FirstOrDefault(r => r.userEmail == userEmail && r.albumId == albumId);
                int userRating = 0;
                double allRatingvalue = 0;
                if (rating != null)
                {
                    userRating = rating.starRating;
                }


                var allrating = _context.AlbumRatings.Where(r => r.albumId == albumId)
                                                            .GroupBy(r => r.albumId)
                                                            .Select(g => new
                                                            {
                                                                AlbumId = g.Key,
                                                                AverageRating = g.Average(r => r.starRating)
                                                            })
                                                            .FirstOrDefault();

                if (allrating != null)
                {
                    // Acesse a propriedade AverageRating
                    allRatingvalue = allrating.AverageRating;
                }

                var albumDetailsUrl = $"https://api.spotify.com/v1/albums/{albumId}/tracks";
                var albumInfoUrl = $"https://api.spotify.com/v1/albums?ids={albumId}";


                var responseJson = await _httpClientHelper.SendAysnc(albumDetailsUrl, SpotifyService.AccessToken);
                var responseJsonInfo = await _httpClientHelper.SendAysnc(albumInfoUrl, SpotifyService.AccessToken);

                var albumDetails = new AlbumDetails
                {
                    AlbumId = albumId,
                    Tracks = new List<Track>(),
                    AlbumsInfo = new List<Album>(),
                    AlbumReviews = new List<AlbumReview>()
                };
                double totalDuration = 0;
                // Processar as faixas do álbum
                var albumTracks = JsonConvert.DeserializeObject<AlbumTracks>(responseJson);
                foreach (var item in albumTracks.items)
                {
                    double durationSecondsw = item.duration_ms;
                    totalDuration += item.duration_ms;
                    TimeSpan t2 = TimeSpan.FromMilliseconds(durationSecondsw);
                    string durationSeconds2 = string.Format("{0:D2}m:{1:D2}s", t2.Minutes, t2.Seconds);

                    var track = new Track
                    {
                        TrackId = item.id,
                        TrackName = item.name,
                        AlbumId = albumId,
                        TrackNumber = item.track_number,
                        DiscNumber = item.disc_number,
                        Href = item.href,
                        IsLocal = item.is_local,
                        Type = item.type,
                        Uri = item.uri,
                        DurationMinutes = durationSeconds2,
                    };
                    albumDetails.Tracks.Add(track);
                }
                var artist = $"https://api.spotify.com/v1/artists/" + albumTracks.items[0].artists[0].id;
                var responseJsonartist = await _httpClientHelper.SendAysnc(artist, SpotifyService.AccessToken);
                dynamic responseJsonArtist = JsonConvert.DeserializeObject(responseJsonartist);

                string name = responseJsonArtist.name;
                string artistimage = responseJsonArtist.images[0].url;
                var genresList = new List<string>();
                var genresArray = (JArray)responseJsonArtist.genres;
                foreach (var genre in genresArray)
                {
                    var genreString = (string)genre;
                    genresList.Add(genreString);
                }

                var albumInfoJson = JObject.Parse(responseJsonInfo);
                var albumsInfo = albumInfoJson["albums"].ToObject<List<Album>>();
                albumDetails.AlbumsInfo.AddRange(albumsInfo);
                string popularity = albumDetails.AlbumsInfo[0].popularity;

                string label = albumDetails.AlbumsInfo[0].label;
                TempData["myrating"] = userRating;
                TempData["allrating"] = allRatingvalue;
                TempData["popularity"] = popularity;
                TempData["label"] = label;
                TempData["artistname"] = name;
                TempData["artistgenres"] = genresList;
                TempData["artistimage"] = artistimage;
                if (admin != null)
                    TempData["meu"] = admin.Admin;
                else
                    TempData["meu"] = false;
                var ratingd = _context.AlbumRatings
                                         .FirstOrDefault(f => f.userEmail == userEmail && f.albumId == albumId);
                int rate = 0;
                if (ratingd != null)
                {
                    rate = rating.starRating;
                }
                else
                {
                    rate = 0; // Ou defina um valor padrão, se necessário
                }


                string listaounao = "false";
                bool albumExists = _context.ListenList.Any(f => f.UserMail == userEmail && f.AlbumId == albumId);

                if (albumExists)
                {
                    listaounao = "true";

                }
                else if (!albumExists)
                {
                    listaounao = "false";

                }

                List<AlbumReview> albumReviews = _context.AlbumReview
                                                .Where(r => r.AlbumId == albumId)
                                                .ToList();
                foreach (var album in albumReviews)
                {
                    if (album.UserMail == userEmail)
                        album.itsMine = true;
                }

                albumDetails.AlbumReviews.AddRange(albumReviews);


                TimeSpan t = TimeSpan.FromMilliseconds(totalDuration);
                string durationSeconds = string.Format("{0:D2}h:{1:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds);
                TempData["rate"] = rate;
                TempData["rated"] = listaounao;
                TempData["durationSeconds"] = durationSeconds;
                return View("AlbumDetails", albumDetails);
            }
            else
            {
                return View("AlbumDetails");
            }
        }


        
        [HttpPost]
        public IActionResult SaveReview(AlbumReview album)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email).Value;
            if (userEmail != "" && userEmail != null)
            {
                if(album.reviewTitle.Length < 30) {

                    if (album.reviewDescription.Length < 300)
                    {

                        if (album.reviewRecommendation == "recomendo" || album.reviewRecommendation == "nao-recomendo")
                        {
                            var albumRating = new AlbumReview
                            {
                                AlbumId = album.AlbumId,
                                UserMail = userEmail,
                                reviewTitle = album.reviewTitle,
                                reviewDescription = album.reviewDescription,
                                reviewRecommendation = album.reviewRecommendation,
                                itsMine = false,
                            };
                            if (album.reviewRecommendation == "recomendo")
                                albumRating.reviewRecommendation = "Recomendo";
                            if (album.reviewRecommendation == "nao-recomendo")
                                albumRating.reviewRecommendation = "Não Recomendo";
                                _context.AlbumReview.Add(albumRating);
                            _context.SaveChanges();

                            return Json(new { success = true });
                        }
                        else { return Json(new { success = false, title = true, description = true, recommendation = false }); }
                    }else
                    {
                        return Json(new { success = false, title = true, description = false });
                    }
                }
                else { return Json(new { success = false, title = false }); }

               
            }
            else
                return Json(new { success = false }); 
        }


        [HttpPost]
        public IActionResult RemoveReview(AlbumReview albumReview)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email).Value;
            if (!string.IsNullOrEmpty(userEmail))
            {
                var reviewExists = _context.AlbumReview.FirstOrDefault(f => f.UserMail == userEmail && f.AlbumId == albumReview.AlbumId);

                if (reviewExists != null)
                {
                    var album = _context.AlbumReview.FirstOrDefault(a => a.AlbumId == reviewExists.AlbumId);
                    if (album != null)
                    {
                        _context.AlbumReview.Remove(reviewExists);
                        _context.SaveChanges();
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Álbum não encontrado" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Revisão não encontrada para este utilizador" });
                }
            }
            else
            {
                return Json(new { success = false, message = "E-mail do utilizador não disponível" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser([FromForm]  ApplicationUser user)
        {
           

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (userEmail == null)
            {
                return BadRequest("User email not found");
            }

            var existingUser = await _userManager.FindByEmailAsync(userEmail);
            if (existingUser == null)
            {
                return NotFound("User not found");
            }

            existingUser.Morada = user.Morada;
            existingUser.Cidade = user.Cidade;
            existingUser.CodigoPostal = user.CodigoPostal;
            existingUser.Pais = user.Pais;
            existingUser.NomeEntrega = user.NomeEntrega;
            existingUser.MoradaEntrega = user.MoradaEntrega;
            existingUser.CidadeEntrega = user.CidadeEntrega;
            existingUser.CodigoPostalEntrega = user.CodigoPostalEntrega;
            existingUser.PaisEntrega = user.PaisEntrega;
            existingUser.NumerotelEntrega = user.NumerotelEntrega;

            try
            {
                var result = await _userManager.UpdateAsync(existingUser);
                if (!result.Succeeded)
                {
                    // Lidar com falhas na atualização do usuário
                    return BadRequest("Failed to update user");
                }
            }
            catch (Exception ex)
            {
                // Lidar com exceções não tratadas
                return StatusCode(500, "An error occurred while updating user");
            }
            //  return Json(new {update = true});
            return View("Profile", existingUser);
        }



        private string GenerateRandomPassword()
        {
            const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string digitChars = "0123456789";
            const string specialChars = "!@#$%&*;:,.";

            var random = new Random();
            var password = new StringBuilder();

            // Adiciona pelo menos um caractere de cada conjunto na senha
            password.Append(uppercaseChars[random.Next(uppercaseChars.Length)]);
            password.Append(lowercaseChars[random.Next(lowercaseChars.Length)]);
            password.Append(digitChars[random.Next(digitChars.Length)]);
            password.Append(specialChars[random.Next(specialChars.Length)]);

            const string allChars = uppercaseChars + lowercaseChars + digitChars + specialChars;

            // Gera o restante da senha aleatoriamente
            for (int i = 4; i < 12; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            var passwordArray = password.ToString().ToCharArray();
            var shuffledPassword = new string(passwordArray.OrderBy(x => random.Next()).ToArray());

            return shuffledPassword;
        }


    }
}
