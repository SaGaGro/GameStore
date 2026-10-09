using Microsoft.AspNetCore.Mvc;
using GameStore.Models.DB;

namespace GameStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class PointController : Controller
    {
        private readonly Csi402dbContext _db;

        public PointController(Csi402dbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var history = _db.Pointhistories
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            ViewBag.TotalEarned  = history.Where(h => h.Type == "Earned").Sum(h => h.Points);
            ViewBag.TotalUsed    = history.Where(h => h.Type == "Used").Sum(h => Math.Abs(h.Points));
            ViewBag.TotalExpired = history.Where(h => h.Type == "Expired").Sum(h => Math.Abs(h.Points));
            ViewBag.Balance      = ViewBag.TotalEarned - ViewBag.TotalUsed - ViewBag.TotalExpired;

            ViewBag.ExpiringPoints = history
                .Where(h => h.Type == "Earned" && h.ExpiresAt != null && h.ExpiresAt <= DateTime.Now.AddDays(30))
                .Sum(h => h.Points);
            ViewBag.ExpiringDate = DateTime.Now.AddDays(30).ToString("dd MMM yyyy");

            return View(history);
        }
    }
}
