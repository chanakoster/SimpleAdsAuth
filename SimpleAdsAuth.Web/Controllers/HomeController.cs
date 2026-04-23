using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleAdsAuth.Data;
using SimpleAdsAuth.Web.Models;
using System;
using System.Diagnostics;

namespace SimpleAdsAuth.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString
            = @"Data Source=.\sqlexpress;Initial Catalog=Ads;Integrated Security=true;Trust Server Certificate=true;";

        public IActionResult Index()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            var repo = new AdsRepository(_connectionString);
            var vm = new AdsViewModel()
            {
                Ads = repo.GetAds(),
                UserId = User.Identity.IsAuthenticated ? repo.GetUserIdFromEmail(User.Identity.Name) : 0,
            };
            return View(vm);
        }

        [Authorize]
        public IActionResult NewAd()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult NewAd(Ad ad)
        {
            var repo = new AdsRepository(_connectionString);
            ad.User = new User { Id = repo.GetUserIdFromEmail(User.Identity.Name) };
            repo.AddAd(ad);
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public IActionResult DeleteAd(int id)
        {
            var repo = new AdsRepository(_connectionString);
            int UserId = repo.GetUserIdFromEmail(User.Identity.Name);
            if (UserId == repo.GetUserIdForAd(id))
            {
                repo.DeleteAd(id);
            }

            else
            {
                repo.ReportUser(UserId, id);
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult MyAccount()
        {
            var repo = new AdsRepository(_connectionString);
            AdsViewModel vm = new AdsViewModel()
            {
                Ads = repo.GetAdsById(repo.GetUserIdFromEmail(User.Identity.Name)),
            };
            return View(vm); ;
        }
    }
}
