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
            AdsManager mgr = new AdsManager(_connectionString);
            AdsViewModel vm = new AdsViewModel()
            {
                Ads = mgr.GetAds(),
                UserId = User.Identity.IsAuthenticated ? mgr.GetUserIdFromEmail(User.Identity.Name) : 0,
            };
            return View(vm);
        }

        [Authorize]
        public IActionResult NewAd()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NewAd(Ad ad)
        {
            var mgr = new AdsManager(_connectionString);
            ad.User = new User { Id = mgr.GetUserIdFromEmail(User.Identity.Name) };
            mgr.AddAd(ad);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteAd(int id)
        {
            var mgr = new AdsManager(_connectionString);
            if (mgr.GetUserIdFromEmail(User.Identity.Name) == mgr.GetUserIdForAd(id))
            {
                mgr.DeleteAd(id);
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Message"] = "Error deleting ad";
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        public IActionResult MyAccount()
        {
            AdsManager mgr = new AdsManager(_connectionString);
            AdsViewModel vm = new AdsViewModel()
            {
                Ads = mgr.GetAdsById(mgr.GetUserIdFromEmail(User.Identity.Name)),
            };
            return View(vm); ;
        }
    }
}
