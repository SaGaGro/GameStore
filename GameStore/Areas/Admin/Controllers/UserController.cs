using Microsoft.AspNetCore.Mvc;
using GameStore.Models.DB;

namespace GameStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly Csi402dbContext _db;

        public UserController(Csi402dbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var users = _db.Users.OrderBy(u => u.RoleId).ThenBy(u => u.Username).ToList();
            return View(users);
        }

        public IActionResult Edit(string UID)
        {
            var user = _db.Users.FirstOrDefault(u => u.UserId == UID);
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User data)
        {
            var user = _db.Users.FirstOrDefault(u => u.UserId == data.UserId);

            if (user == null)
                return RedirectToAction("Index");

            user.Username    = data.Username;
            user.Email       = data.Email;
            user.DisplayName = data.DisplayName;
            user.Country     = data.Country;
            user.RoleId      = data.RoleId;
            user.UpdatedAt   = DateTime.Now;

            _db.Update(user);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(string UID)
        {
            var users = _db.Users.Where(u => u.UserId == UID).ToList();
            _db.Users.RemoveRange(users);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
