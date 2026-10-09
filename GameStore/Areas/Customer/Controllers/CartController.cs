using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameStore.Models.DB;
using GameStore.ViewModels;
using System.Text.Json;

namespace GameStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly Csi402dbContext _db;
        private const string CartKey = "Cart";
        private const decimal PointsPerBaht = 0.1m;   // ซื้อ 1 บาท = 0.1 แต้ม
        private const decimal PointValueInBaht = 0.5m; // 1 แต้ม = 0.5 บาท
        private const decimal SeriesDiscountPct = 30m; // SeriesDiscount ลด 30%

        public CartController(Csi402dbContext db)
        {
            _db = db;
        }

        // ---------- helpers ----------

        private List<int> GetCartIds()
        {
            var json = HttpContext.Session.GetString(CartKey);
            return json == null ? new List<int>() : JsonSerializer.Deserialize<List<int>>(json)!;
        }

        private void SaveCartIds(List<int> ids)
        {
            HttpContext.Session.SetString(CartKey, JsonSerializer.Serialize(ids));
        }

        /// <summary>คำนวณ CartItems พร้อม discount ทุกประเภท</summary>
        private List<CartItem> BuildCartItems(List<Game> games, string userId)
        {
            var now = DateTime.Now;

            // โปรโมชั่น FestivalSale ที่ active อยู่ตอนนี้
            var activePromos = _db.Promotions
                .Where(p => p.IsActive == true
                         && p.Type == "FestivalSale"
                         && p.StartDate <= now
                         && p.EndDate >= now)
                .ToList();

            // Library ของ user (ใช้เช็ค SeriesDiscount)
            var libraryGameIds = _db.Userlibraries
                .Where(ul => ul.UserId == userId)
                .Select(ul => ul.GameId)
                .ToHashSet();

            var items = new List<CartItem>();

            foreach (var game in games)
            {
                decimal effectivePrice = game.BasePrice;
                decimal discountAmount = 0;
                string? discountNote = null;

                // 1. Early Bird (ReleaseDate อนาคต + DiscountPercent)
                if (game.ReleaseDate > now && game.DiscountPercent > 0)
                {
                    discountAmount = game.BasePrice * game.DiscountPercent.Value / 100;
                    effectivePrice = game.BasePrice - discountAmount;
                    discountNote = $"⚡ Early Bird -{game.DiscountPercent}%";
                }
                // 2. FestivalSale — ถ้ามีโปรที่ครอบเกมนี้
                else if (activePromos.Any(p => p.GameId == game.GameId))
                {
                    var promo = activePromos.First(p => p.GameId == game.GameId);
                    discountAmount = game.BasePrice * promo.DiscountPercent / 100;
                    effectivePrice = game.BasePrice - discountAmount;
                    discountNote = $"🏷️ {promo.Title} -{promo.DiscountPercent}%";
                }
                // 3. DiscountPercent ทั่วไป (ไม่ใช่ Early Bird)
                else if (game.DiscountPercent > 0)
                {
                    discountAmount = game.BasePrice * game.DiscountPercent.Value / 100;
                    effectivePrice = game.BasePrice - discountAmount;
                    discountNote = $"💰 ส่วนลด -{game.DiscountPercent}%";
                }

                // 4. SeriesDiscount — ถ้ามีเกมในซีรีส์เดียวกันใน Library แล้ว
                if (game.SeriesId != null && discountAmount == 0)
                {
                    var hasSeriesGame = _db.Games
                        .Any(g => g.SeriesId == game.SeriesId
                               && g.GameId != game.GameId
                               && libraryGameIds.Contains(g.GameId));

                    if (hasSeriesGame)
                    {
                        discountAmount = game.BasePrice * SeriesDiscountPct / 100;
                        effectivePrice = game.BasePrice - discountAmount;
                        discountNote = $"🎮 ซื้อซีรีส์ -{SeriesDiscountPct}%";
                    }
                }

                items.Add(new CartItem
                {
                    Game           = game,
                    EffectivePrice = Math.Max(effectivePrice, 0),
                    DiscountAmount = discountAmount,
                    DiscountNote   = discountNote
                });
            }

            return items;
        }

        // ---------- actions ----------

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var ids = GetCartIds();
            var games = ids.Count > 0
                ? _db.Games.Where(g => ids.Contains(g.GameId)).ToList()
                : new List<Game>();

            var user = _db.Users.FirstOrDefault(u => u.UserId == userId);
            var pointsAvailable = (int)(user?.WalletPoints ?? 0);

            var items           = BuildCartItems(games, userId);
            var subtotal        = items.Sum(i => i.Game.BasePrice);
            var promoDiscount   = items.Sum(i => i.DiscountAmount);
            var afterPromo      = subtotal - promoDiscount;

            // แต้มที่ใช้ได้สูงสุด = ไม่เกิน afterPromo / PointValueInBaht และไม่เกินที่มี
            var maxRedeemable   = (int)Math.Floor(afterPromo / PointValueInBaht);
            var pointsRedeemable = Math.Min(pointsAvailable, maxRedeemable);

            var cart = new CartViewModel
            {
                Items                = items,
                Subtotal             = subtotal,
                DiscountFromPromotion = promoDiscount,
                PointsAvailable      = pointsAvailable,
                PointsRedeemable     = pointsRedeemable,
                PointsUsed           = 0,
                PointsDiscount       = 0,
                Total                = afterPromo,
                PointsEarned         = (int)(afterPromo * PointsPerBaht)
            };

            return View(cart);
        }

        [HttpPost]
        public IActionResult Add(int gameId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            if (_db.Userlibraries.Any(ul => ul.UserId == userId && ul.GameId == gameId))
                return RedirectToAction("Index");

            var ids = GetCartIds();
            if (!ids.Contains(gameId))
            {
                ids.Add(gameId);
                SaveCartIds(ids);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Remove(int gameId)
        {
            var ids = GetCartIds();
            ids.Remove(gameId);
            SaveCartIds(ids);
            return RedirectToAction("Index");
        }

        // GET: หน้า mock payment — รับ pointsToUse จาก query
        public IActionResult Checkout(int pointsToUse = 0)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var ids = GetCartIds();
            if (ids.Count == 0) return RedirectToAction("Index");

            var games           = _db.Games.Where(g => ids.Contains(g.GameId)).ToList();
            var items           = BuildCartItems(games, userId);
            var subtotal        = items.Sum(i => i.Game.BasePrice);
            var promoDiscount   = items.Sum(i => i.DiscountAmount);
            var afterPromo      = subtotal - promoDiscount;

            var user            = _db.Users.FirstOrDefault(u => u.UserId == userId);
            var pointsAvailable = (int)(user?.WalletPoints ?? 0);
            var maxRedeemable   = (int)Math.Floor(afterPromo / PointValueInBaht);
            pointsToUse         = Math.Min(pointsToUse, Math.Min(pointsAvailable, maxRedeemable));

            var pointsDiscount  = pointsToUse * PointValueInBaht;
            var finalTotal      = afterPromo - pointsDiscount;

            ViewBag.Total        = finalTotal;
            ViewBag.PointsUsed   = pointsToUse;
            ViewBag.PointsEarned = (int)(finalTotal * PointsPerBaht);

            return View();
        }

        // POST: บันทึก Order จริง
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PlaceOrder(int pointsToUse = 0)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var ids = GetCartIds();
            if (ids.Count == 0) return RedirectToAction("Index");

            var games           = _db.Games.Where(g => ids.Contains(g.GameId)).ToList();
            var items           = BuildCartItems(games, userId);
            var subtotal        = items.Sum(i => i.Game.BasePrice);
            var promoDiscount   = items.Sum(i => i.DiscountAmount);
            var afterPromo      = subtotal - promoDiscount;

            var user            = _db.Users.FirstOrDefault(u => u.UserId == userId);
            var pointsAvailable = (int)(user?.WalletPoints ?? 0);
            var maxRedeemable   = (int)Math.Floor(afterPromo / PointValueInBaht);
            pointsToUse         = Math.Min(pointsToUse, Math.Min(pointsAvailable, maxRedeemable));

            var pointsDiscount  = pointsToUse * PointValueInBaht;
            var finalTotal      = afterPromo - pointsDiscount;
            var pointsEarned    = (int)(finalTotal * PointsPerBaht);

            // สร้าง Order
            var order = new Order
            {
                CustomerId   = userId,
                TotalAmount  = subtotal,
                PointsUsed   = pointsToUse,
                FinalAmount  = finalTotal,
                PointsEarned = pointsEarned,
                OrderStatus  = "Completed",
                CreatedAt    = DateTime.Now,
                UpdatedAt    = DateTime.Now
            };
            _db.Orders.Add(order);
            _db.SaveChanges();

            // OrderItems + Library + SoldCount
            foreach (var item in items)
            {
                _db.Orderitems.Add(new Orderitem
                {
                    OrderId        = order.OrderId,
                    GameId         = item.Game.GameId,
                    UnitPrice      = item.Game.BasePrice,
                    DiscountAmount = item.DiscountAmount,
                    NetPrice       = item.EffectivePrice,
                    DiscountNote   = item.DiscountNote,
                    CreatedAt      = DateTime.Now
                });

                if (!_db.Userlibraries.Any(ul => ul.UserId == userId && ul.GameId == item.Game.GameId))
                {
                    _db.Userlibraries.Add(new Userlibrary
                    {
                        UserId    = userId,
                        GameId    = item.Game.GameId,
                        CreatedAt = DateTime.Now
                    });
                }

                item.Game.SoldCount = (item.Game.SoldCount ?? 0) + 1;
            }

            // จัดการแต้ม
            if (user != null)
            {
                // หักแต้มที่ใช้
                if (pointsToUse > 0)
                {
                    user.WalletPoints = (user.WalletPoints ?? 0) - pointsToUse;
                    _db.Pointhistories.Add(new Pointhistory
                    {
                        UserId      = userId,
                        Points      = -pointsToUse,
                        Type        = "Used",
                        Description = $"ใช้แต้มซื้อเกม (Order #{order.OrderId})",
                        CreatedAt   = DateTime.Now
                    });
                }

                // เพิ่มแต้มที่ได้
                if (pointsEarned > 0)
                {
                    user.WalletPoints = (user.WalletPoints ?? 0) + pointsEarned;
                    _db.Pointhistories.Add(new Pointhistory
                    {
                        UserId      = userId,
                        Points      = pointsEarned,
                        Type        = "Earned",
                        Description = $"ซื้อเกม {items.Count} เกม (Order #{order.OrderId})",
                        CreatedAt   = DateTime.Now
                    });
                }
            }

            _db.SaveChanges();

            // FreeGame: claim เกมฟรีที่ active อยู่ใน library
            var now = DateTime.Now;
            var freeGames = _db.Promotions
                .Include(p => p.Game)
                .Where(p => p.IsActive == true
                         && p.Type == "FreeGame"
                         && p.StartDate <= now
                         && p.EndDate >= now)
                .ToList();

            foreach (var fp in freeGames)
            {
                if (!_db.Userlibraries.Any(ul => ul.UserId == userId && ul.GameId == fp.GameId))
                {
                    _db.Userlibraries.Add(new Userlibrary
                    {
                        UserId    = userId,
                        GameId    = fp.GameId,
                        CreatedAt = DateTime.Now
                    });
                }
            }

            _db.SaveChanges();
            SaveCartIds(new List<int>());

            TempData["PointsEarned"] = pointsEarned;
            TempData["PointsUsed"]   = pointsToUse;
            TempData["OrderId"]      = order.OrderId;
            TempData["FreeGames"]    = freeGames.Count;

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }

        // GET: claim เกมฟรี (standalone)
        [HttpPost]
        public IActionResult ClaimFreeGame(int gameId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var now = DateTime.Now;
            var promo = _db.Promotions.FirstOrDefault(p =>
                p.GameId == gameId
                && p.Type == "FreeGame"
                && p.IsActive == true
                && p.StartDate <= now
                && p.EndDate >= now);

            if (promo != null && !_db.Userlibraries.Any(ul => ul.UserId == userId && ul.GameId == gameId))
            {
                _db.Userlibraries.Add(new Userlibrary
                {
                    UserId    = userId,
                    GameId    = gameId,
                    CreatedAt = DateTime.Now
                });
                _db.SaveChanges();
            }

            return RedirectToAction("Index", "Library");
        }
    }
}
