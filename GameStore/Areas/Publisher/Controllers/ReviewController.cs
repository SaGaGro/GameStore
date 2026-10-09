using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameStore.Models.DB;

namespace GameStore.Areas.Publisher.Controllers
{
    [Area("Publisher")]
    public class ReviewController : Controller
    {
        private readonly Csi402dbContext _db;

        public ReviewController(Csi402dbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var publisherId = HttpContext.Session.GetString("UserId");
            if (publisherId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var reviews = _db.Reviews
                .Include(r => r.User)
                .Include(r => r.Game)
                .Where(r => r.Game.PublisherId == publisherId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            ViewBag.AverageRating = reviews.Count > 0
                ? reviews.Average(r => r.Rating).ToString("F1")
                : "0.0";
            ViewBag.TotalReviews = reviews.Count;
            ViewBag.FiveStars    = reviews.Count(r => r.Rating == 5);
            ViewBag.FourStars    = reviews.Count(r => r.Rating == 4);
            ViewBag.ThreeStars   = reviews.Count(r => r.Rating == 3);

            return View(reviews);
        }
    }
}
