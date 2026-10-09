using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameStore.Models.DB;

namespace GameStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly Csi402dbContext _db;

        public HomeController(Csi402dbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            ViewBag.TotalUsers      = _db.Users.Count(u => u.RoleId == 4);
            ViewBag.TotalPublishers = _db.Users.Count(u => u.RoleId == 3);
            ViewBag.TotalGames      = _db.Games.Count();
            ViewBag.TotalRevenue    = _db.Orders.Where(o => o.OrderStatus == "Completed").Sum(o => (decimal?)o.FinalAmount) ?? 0;
            ViewBag.PendingGames      = _db.Games.Count(g => g.IsApproved == false);
            ViewBag.PendingPublishers = _db.Publisherrequests.Count(r => r.Status == "Pending");
            ViewBag.ActivePromotions  = _db.Promotions.Count(p => p.IsActive == true && p.StartDate <= DateTime.Now && p.EndDate >= DateTime.Now);

            return View();
        }
    }
}
