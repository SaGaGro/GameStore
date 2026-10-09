using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameStore.Models.DB;

namespace GameStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class GameController : Controller
    {
        private readonly Csi402dbContext _db;

        public GameController(Csi402dbContext db)
        {
            _db = db;
        }

        public IActionResult Index(string? search, string? genre, string? sort, string? priceRange)
        {
            var query = _db.Games.Where(g => g.IsApproved == true).AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(g => g.Title.Contains(search));

            if (!string.IsNullOrEmpty(genre) && genre != "all")
                query = query.Where(g => g.Genre == genre);

            if (!string.IsNullOrEmpty(priceRange))
            {
                query = priceRange switch
                {
                    "free"      => query.Where(g => g.IsFree == true),
                    "under500"  => query.Where(g => g.IsFree != true && g.BasePrice <= 500),
                    "500to1000" => query.Where(g => g.BasePrice > 500 && g.BasePrice <= 1000),
                    "over1000"  => query.Where(g => g.BasePrice > 1000),
                    _           => query
                };
            }

            var games = sort switch
            {
                "price_asc"  => query.OrderBy(g => g.BasePrice).ToList(),
                "price_desc" => query.OrderByDescending(g => g.BasePrice).ToList(),
                "rating"     => query.OrderByDescending(g => g.Rating).ToList(),
                "bestseller" => query.OrderByDescending(g => g.SoldCount).ToList(),
                "newest"     => query.OrderByDescending(g => g.CreatedAt).ToList(),
                _            => query.OrderByDescending(g => g.SoldCount).ToList()
            };

            ViewBag.Search     = search;
            ViewBag.Genre      = genre;
            ViewBag.Sort       = sort;
            ViewBag.PriceRange = priceRange;
            ViewBag.Genres     = _db.Games.Where(g => g.IsApproved == true && g.Genre != null)
                                          .Select(g => g.Genre).Distinct().OrderBy(g => g).ToList();

            return View(games);
        }

        public IActionResult Detail(int id)
        {
            var game = _db.Games
                .Include(g => g.Reviews).ThenInclude(r => r.User)
                .Include(g => g.Publisher)
                .FirstOrDefault(g => g.GameId == id && g.IsApproved == true);
            if (game == null) return NotFound();

            var now = DateTime.Now;
            var freePromo = _db.Promotions.FirstOrDefault(p =>
                p.GameId == id
                && p.Type == "FreeGame"
                && p.IsActive == true
                && p.StartDate <= now
                && p.EndDate >= now);

            ViewBag.FreePromo = freePromo;

            var userId = HttpContext.Session.GetString("UserId");
            ViewBag.IsOwned     = userId != null && _db.Userlibraries.Any(ul => ul.UserId == userId && ul.GameId == id);
            ViewBag.IsWishlisted = userId != null && _db.Wishlists.Any(w => w.UserId == userId && w.GameId == id);
            var cartJson = HttpContext.Session.GetString("Cart");
            ViewBag.InCart = userId != null && cartJson != null &&
                             System.Text.Json.JsonSerializer.Deserialize<List<int>>(cartJson)!.Contains(id);

            return View(game);
        }
    }
}
