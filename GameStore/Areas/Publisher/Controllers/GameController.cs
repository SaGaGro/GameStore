using Microsoft.AspNetCore.Mvc;
using GameStore.Models.DB;
using GameStore.ViewModels;

namespace GameStore.Areas.Publisher.Controllers
{
    [Area("Publisher")]
    public class GameController : Controller
    {
        private readonly Csi402dbContext _db;
        private readonly IWebHostEnvironment _env;

        public GameController(Csi402dbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        private async Task<string?> SaveCoverImageAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;
            var folder = Path.Combine(_env.WebRootPath, "uploads", "games");
            Directory.CreateDirectory(folder);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folder, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return $"/uploads/games/{fileName}";
        }

        public IActionResult Index()
        {
            var publisherId = HttpContext.Session.GetString("UserId");
            if (publisherId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var games = _db.Games
                .Where(g => g.PublisherId == publisherId)
                .OrderByDescending(g => g.CreatedAt)
                .ToList();

            return View(games);
        }

        public IActionResult Create()
        {
            return View(new GameViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GameViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var coverPath = await SaveCoverImageAsync(model.CoverImage);

            var game = new Game
            {
                Title       = model.Title,
                Description = model.Description,
                BasePrice   = model.IsFree ? 0 : model.Price,
                Genre       = model.Genre,
                IsFree      = model.IsFree,
                HasTrial    = model.HasTrial,
                TrialHours  = model.TrialHours,
                CoverImage  = coverPath,
                ReleaseDate = model.ReleasedAt,
                PublisherId = HttpContext.Session.GetString("UserId"),
                IsApproved  = false,
                StatusId    = 0,
                CreatedAt   = DateTime.Now
            };

            _db.Games.Add(game);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var publisherId = HttpContext.Session.GetString("UserId");
            if (publisherId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var game = _db.Games.FirstOrDefault(g => g.GameId == id && g.PublisherId == publisherId);
            if (game == null) return NotFound();

            var model = new GameViewModel
            {
                GameId            = game.GameId,
                Title             = game.Title,
                Description       = game.Description ?? "",
                Price             = game.BasePrice,
                Genre             = game.Genre,
                ExistingCoverImage = game.CoverImage,
                IsFree            = game.IsFree ?? false,
                HasTrial          = game.HasTrial ?? false,
                TrialHours        = game.TrialHours,
                ReleasedAt        = game.ReleaseDate ?? DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GameViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var publisherId = HttpContext.Session.GetString("UserId");
            if (publisherId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var game = _db.Games.FirstOrDefault(g => g.GameId == model.GameId && g.PublisherId == publisherId);
            if (game == null) return NotFound();

            var newCover = await SaveCoverImageAsync(model.CoverImage);

            game.Title       = model.Title;
            game.Description = model.Description;
            game.BasePrice   = model.IsFree ? 0 : model.Price;
            game.Genre       = model.Genre;
            game.IsFree      = model.IsFree;
            game.HasTrial    = model.HasTrial;
            game.TrialHours  = model.TrialHours;
            game.ReleaseDate = model.ReleasedAt;
            game.CoverImage  = newCover ?? game.CoverImage;
            game.UpdatedAt   = DateTime.Now;

            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var publisherId = HttpContext.Session.GetString("UserId");
            if (publisherId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var game = _db.Games.FirstOrDefault(g => g.GameId == id && g.PublisherId == publisherId);
            if (game == null) return NotFound();
            return View(game);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var publisherId = HttpContext.Session.GetString("UserId");
            if (publisherId == null) return RedirectToAction("Login", "Account", new { area = "" });

            var game = _db.Games.FirstOrDefault(g => g.GameId == id && g.PublisherId == publisherId);
            if (game != null)
            {
                _db.Games.Remove(game);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
