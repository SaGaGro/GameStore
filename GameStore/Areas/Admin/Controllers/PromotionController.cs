using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameStore.Models.DB;

namespace GameStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PromotionController : Controller
    {
        private readonly Csi402dbContext _db;

        public PromotionController(Csi402dbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var promotions = _db.Promotions
                .Include(p => p.Game)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(promotions);
        }

        public IActionResult Create()
        {
            ViewBag.Games = _db.Games
                .Where(g => g.IsApproved == true)
                .OrderBy(g => g.Title)
                .Select(g => new { g.GameId, g.Title })
                .ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Promotion model)
        {
            model.CreatedAt = DateTime.Now;
            model.IsActive  = true;
            _db.Promotions.Add(model);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var promo = _db.Promotions.FirstOrDefault(p => p.PromotionId == id);
            if (promo == null) return RedirectToAction("Index");

            ViewBag.Games = _db.Games
                .Where(g => g.IsApproved == true)
                .OrderBy(g => g.Title)
                .Select(g => new { g.GameId, g.Title })
                .ToList();
            return View(promo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Promotion model)
        {
            var promo = _db.Promotions.FirstOrDefault(p => p.PromotionId == model.PromotionId);
            if (promo == null) return RedirectToAction("Index");

            promo.Title           = model.Title;
            promo.Type            = model.Type;
            promo.GameId          = model.GameId;
            promo.DiscountPercent = model.DiscountPercent;
            promo.StartDate       = model.StartDate;
            promo.EndDate         = model.EndDate;
            promo.UpdatedAt       = DateTime.Now;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var promo = _db.Promotions.FirstOrDefault(p => p.PromotionId == id);
            if (promo != null)
            {
                _db.Promotions.Remove(promo);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
