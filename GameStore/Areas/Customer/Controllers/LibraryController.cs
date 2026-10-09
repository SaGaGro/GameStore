using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameStore.Models.DB;

namespace GameStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class LibraryController : Controller
    {
        private readonly Csi402dbContext _db;

        public LibraryController(Csi402dbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var games = _db.Userlibraries
                .Where(ul => ul.UserId == userId)
                .Include(ul => ul.Game)
                .Select(ul => ul.Game)
                .ToList();

            return View(games);
        }
    }
}
