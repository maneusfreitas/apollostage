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
    public class MarketController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public MarketController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
       
        [HttpGet]
        public async Task<IActionResult> market()
        {

            return View("index", "Market");
        }


    }
}
