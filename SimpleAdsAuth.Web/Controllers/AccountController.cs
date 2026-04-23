using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleAdsAuth.Data;
using System.Security.Claims;

namespace SimpleAdsAuth.Web.Controllers
{
    public class AccountController : Controller
    {
        private string _connectionString
             = @"Data Source=.\sqlexpress;Initial Catalog=Ads;Integrated Security=true;Trust Server Certificate=true;";

        public IActionResult Signup()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            return View();
        }

        [HttpPost]
        public IActionResult Signup(User user)
        {
            var repo = new AdsRepository(_connectionString);
            bool userAddedSuccessfully = repo.AddUser(user);
            if (userAddedSuccessfully)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                TempData["Message"] = "Invalid User Credentials";
                return RedirectToAction("Signup", "Account");
            }
        }

        public IActionResult Login()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(User tempUser)
        {
            var repo = new AdsRepository(_connectionString);
            var user = repo.Login(tempUser);
            if (user == null)
            {
                TempData["Message"] = "Invalid Login";
                return RedirectToAction("Login");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                    new ClaimsIdentity(claims, "Cookies", ClaimTypes.Email, "roles"))
                ).Wait();

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult Signout()
        {
            HttpContext.SignOutAsync().Wait();
            return RedirectToAction("Index", "Home");
        }
    }
}
