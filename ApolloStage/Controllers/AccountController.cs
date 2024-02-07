using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ApolloStage.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text;
using System.Reflection;

namespace ApolloStageFirst.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

       

        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
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
        public IActionResult LimitAttempts()
        {
            return View("LimitAttempts");
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
        public async Task<IActionResult> VerifyCodeAsync(ApplicationUser model,string UserMail, string code1, string code2, string code3, string code4, string pass = "normal")
        {
            if (pass == "normal" || pass == "pass") { 
            try
            {
                   
                    var user = await _userManager.FindByEmailAsync(UserMail);

                if (user == null)
                {
                    return RedirectToAction("UserNotFound");
                }

                string combinedCode = code1 + code2 + code3 + code4;
                    Console.WriteLine("model.UserMail -- : --" + model.UserMail);

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
                        Console.WriteLine("UpdateAsync falhou: " + string.Join(", ", updateResult.Errors.Select(e => e.Description)));
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
                Console.WriteLine("Exceção durante a verificação do código: " + ex.ToString());
                return RedirectToAction("Error");
            }
        }else
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
                return RedirectToAction("Error");
            }

            var result = await HttpContext.AuthenticateAsync("Identity.External");
            var externalEmail = result?.Principal?.FindFirstValue(ClaimTypes.Email);
            var externalFirstName = result?.Principal?.FindFirstValue(ClaimTypes.GivenName);

            if (externalEmail == null)
            {
                return RedirectToAction("Error");
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
                };


                var creationResult = await _userManager.CreateAsync(newUser);

                if (creationResult.Succeeded)
                {
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Error");
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

                    return View("CodeConfimPassword", existingUser);
                }
            }

            TempData["fail"] = "E-mail não encontrado";
            return View("ForgotPassword", user);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ApplicationUser user)
        {
            Console.WriteLine("ChangePassword " + user.UserMail);

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
                        return RedirectToAction("Index", "Home");
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



        [HttpGet]
        public async Task<IActionResult> ListenList()
        {

            return View("ListenList", "Account");
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
