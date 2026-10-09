using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameStore.Models.DB;
using System.Text.Json;

namespace GameStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class WishlistController : Controller
    {
        private readonly Csi402dbContext _db;

        public WishlistController(Csi402dbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var games = _db.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Game)
                .Select(w => w.Game)
                .ToList();

            return View(games);
        }

        [HttpPost]
        public IActionResult Add(int gameId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            if (!_db.Wishlists.Any(w => w.UserId == userId && w.GameId == gameId))
            {
                _db.Wishlists.Add(new Wishlist
                {
                    UserId    = userId,
                    GameId    = gameId,
                    CreatedAt = DateTime.Now
                });
                _db.SaveChanges();
            }

            return RedirectBack();
        }

        [HttpPost]
        public IActionResult Remove(int gameId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var item = _db.Wishlists.FirstOrDefault(w => w.UserId == userId && w.GameId == gameId);
            if (item != null)
            {
                _db.Wishlists.Remove(item);
                _db.SaveChanges();
            }

            return RedirectBack();
        }

        [HttpPost]
        public IActionResult AddAllToCart(string gameIds)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var ids = JsonSerializer.Deserialize<List<int>>(gameIds) ?? new List<int>();
            var cartJson = HttpContext.Session.GetString("Cart");
            var cart = cartJson != null ? JsonSerializer.Deserialize<List<int>>(cartJson)! : new List<int>();

            foreach (var id in ids)
            {
                if (!cart.Contains(id) && !_db.Userlibraries.Any(ul => ul.UserId == userId && ul.GameId == id))
                    cart.Add(id);
            }

            HttpContext.Session.SetString("Cart", JsonSerializer.Serialize(cart));
            return RedirectToAction("Index", "Cart");
        }

        private IActionResult RedirectBack()
        {
            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer)) return Redirect(referer);
            return RedirectToAction("Index");
        }
    }
}
