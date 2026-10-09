using Microsoft.AspNetCore.Mvc;
using GameStore.Models.DB;

namespace GameStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly Csi402dbContext _db;

        public HomeController(Csi402dbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var games = _db.Games
                .Where(g => g.IsApproved == true)
                .OrderByDescending(g => g.SoldCount)
                .Take(8)
                .ToList();

            return View(games);
        }
    }
}
