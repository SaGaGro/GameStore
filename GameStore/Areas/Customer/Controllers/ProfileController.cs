using Microsoft.AspNetCore.Mvc;
using GameStore.ViewModels;
using GameStore.Models.DB;

namespace GameStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProfileController : Controller
    {
        private readonly Csi402dbContext _db;

        public ProfileController(Csi402dbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var user = _db.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "" });

            var profile = new ProfileViewModel
            {
                Username      = user.Username,
                Email         = user.Email ?? "",
                DisplayName   = user.DisplayName ?? user.Username,
                Phone         = user.Phone,
                Country       = user.Country,
                ProfileImage  = user.ProfileImage,
                RoleId        = user.RoleId,
                CreatedAt     = user.CreatedAt ?? DateTime.Now,
                GamesOwned    = _db.Userlibraries.Count(ul => ul.UserId == userId),
                WishlistCount = _db.Wishlists.Count(w => w.UserId == userId),
                ReviewCount   = _db.Reviews.Count(r => r.UserId == userId),
                PointsBalance = (int)(user.WalletPoints ?? 0)
            };

            return View(profile);
        }

        public IActionResult Edit()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var user = _db.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "" });

            var profile = new ProfileViewModel
            {
                Username     = user.Username,
                Email        = user.Email ?? "",
                DisplayName  = user.DisplayName ?? user.Username,
                Phone        = user.Phone,
                Country      = user.Country,
                ProfileImage = user.ProfileImage
            };

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var user = _db.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "" });

            user.DisplayName = model.DisplayName;
            user.Phone       = model.Phone;
            user.Country     = model.Country;
            user.UpdatedAt   = DateTime.Now;
            _db.SaveChanges();

            HttpContext.Session.SetString("DisplayName", model.DisplayName);

            return RedirectToAction("Index");
        }

        public IActionResult ApplyPublisher()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApplyPublisher(PublisherRequestViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var request = new Publisherrequest
            {
                UserId      = userId,
                CompanyName = model.CompanyName,
                Description = model.Description,
                ContactInfo = model.ContactInfo,
                Status      = "Pending",
                RequestedAt = DateTime.Now
            };
            _db.Publisherrequests.Add(request);
            _db.SaveChanges();

            return RedirectToAction("ApplySuccess");
        }

        public IActionResult ApplySuccess()
        {
            return View();
        }
    }
}