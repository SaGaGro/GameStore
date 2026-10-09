using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameStore.Models.DB;

namespace GameStore.Areas.Publisher.Controllers
{
    [Area("Publisher")]
    public class PromotionController : Controller
    {
        private readonly Csi402dbContext _db;

        public PromotionController(Csi402dbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var publisherId = HttpContext.Session.GetString("UserId");
            if (publisherId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var promotions = _db.Promotions
                .Include(p => p.Game)
                .Where(p => p.PublisherId == publisherId)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(promotions);
        }

        public IActionResult Create()
        {
            var publisherId = HttpContext.Session.GetString("UserId");
            if (publisherId == null) return RedirectToAction("Login", "Account", new { area = "" });

            ViewBag.Games = _db.Games
                .Where(g => g.PublisherId == publisherId)
                .OrderBy(g => g.Title)
                .Select(g => new { g.GameId, g.Title })
                .ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Promotion model)
        {
            var publisherId = HttpContext.Session.GetString("UserId");
            if (publisherId == null) return RedirectToAction("Login", "Account", new { area = "" });

            // ตรวจว่าเกมนี้เป็นของ publisher จริงๆ
            if (!_db.Games.Any(g => g.GameId == model.GameId && g.PublisherId == publisherId))
                return Forbid();

            model.PublisherId = publisherId;
            model.CreatedAt   = DateTime.Now;
            model.IsActive    = true;
            _db.Promotions.Add(model);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var publisherId = HttpContext.Session.GetString("UserId");
            if (publisherId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var promo = _db.Promotions.FirstOrDefault(p => p.PromotionId == id && p.PublisherId == publisherId);
            if (promo != null)
            {
                _db.Promotions.Remove(promo);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
