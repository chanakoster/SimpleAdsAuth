using Microsoft.AspNetCore.Authentication;
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
            var mgr = new AdsManager(_connectionString);
            bool userAddedSuccessfully = mgr.AddUser(user);
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
            var mgr = new AdsManager(_connectionString);
            var user = mgr.Login(tempUser);
            if (user == null)
            {
                TempData["Message"] = "Invalid Login";
                return RedirectToAction("Login");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email) //whatever we set here, is what will be available in User.Identity.Name
            };

            //this code does the actual login - it tells MVC to set the auth cookie, and the 
            //users email address will be "baked" into that cookie

            HttpContext.SignInAsync(new ClaimsPrincipal(
                    new ClaimsIdentity(claims, "Cookies", ClaimTypes.Email, "roles"))
                ).Wait();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Signout()
        {
            HttpContext.SignOutAsync().Wait();
            return RedirectToAction("Index", "Home");
        }
    }
}
