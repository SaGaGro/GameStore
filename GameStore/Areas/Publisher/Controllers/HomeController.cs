using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameStore.Models.DB;

namespace GameStore.Areas.Publisher.Controllers
{
    [Area("Publisher")]
    public class HomeController : Controller
    {
        private readonly Csi402dbContext _db;

        public HomeController(Csi402dbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var publisherId = HttpContext.Session.GetString("UserId");
            if (publisherId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var games = _db.Games.Where(g => g.PublisherId == publisherId).ToList();

            ViewBag.TotalGames       = games.Count;
            ViewBag.TotalSold        = games.Sum(g => g.SoldCount ?? 0);
            ViewBag.TotalRevenue     = _db.Orderitems
                                          .Where(oi => oi.Game.PublisherId == publisherId)
                                          .Sum(oi => (decimal?)oi.NetPrice) ?? 0;
            ViewBag.AverageRating    = games.Where(g => g.Rating > 0).Any()
                                          ? games.Where(g => g.Rating > 0).Average(g => (double)g.Rating!).ToString("F1")
                                          : "0.0";
            ViewBag.PendingGames     = games.Count(g => g.IsApproved != true);
            ViewBag.ActivePromotions = _db.Promotions
                                          .Count(p => p.PublisherId == publisherId
                                                   && p.IsActive == true
                                                   && p.StartDate <= DateTime.Now
                                                   && p.EndDate >= DateTime.Now);

            var topGames = games
                .Where(g => g.IsApproved == true)
                .OrderByDescending(g => g.SoldCount ?? 0)
                .Take(5)
                .ToList();
            ViewBag.TopGames = topGames;

            var recentOrders = _db.Orderitems
                .Where(oi => oi.Game.PublisherId == publisherId)
                .Include(oi => oi.Order).ThenInclude(o => o.Customer)
                .Include(oi => oi.Game)
                .OrderByDescending(oi => oi.Order.CreatedAt)
                .Take(5)
                .ToList();
            ViewBag.RecentOrders = recentOrders;

            return View();
        }
    }
}
