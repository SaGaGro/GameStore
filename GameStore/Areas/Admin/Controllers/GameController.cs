using Microsoft.AspNetCore.Mvc;
using GameStore.Models.DB;

namespace GameStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GameController : Controller
    {
        private readonly Csi402dbContext _db;

        public GameController(Csi402dbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var games = _db.Games
                .OrderBy(g => g.IsApproved)
                .ThenByDescending(g => g.CreatedAt)
                .ToList();

            return View(games);
        }

        [HttpPost]
        public IActionResult Approve(int id)
        {
            var game = _db.Games.FirstOrDefault(g => g.GameId == id);
            if (game != null)
            {
                game.IsApproved = true;
                game.StatusId   = 1;
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            var game = _db.Games.FirstOrDefault(g => g.GameId == id);
            if (game != null)
            {
                game.IsApproved = false;
                game.StatusId   = 0;
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
