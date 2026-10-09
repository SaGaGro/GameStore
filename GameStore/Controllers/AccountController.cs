using Microsoft.AspNetCore.Mvc;
using GameStore.ViewModels;
using GameStore.Models.DB;


namespace GameStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly Csi402dbContext _db;

        public AccountController(Csi402dbContext db)
        {
            _db = db;
        }


        // GET: /Account/Login
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserId") != null)
                return RedirectByRole(HttpContext.Session.GetInt32("RoleId") ?? 4); // GetInt32 คืน int? จึงต้อง ?? 4
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _db.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null || user.PasswordHash != model.Password)
            {
                ModelState.AddModelError("", "อีเมลหรือรหัสผ่านไม่ถูกต้อง");
                return View(model);
            }
            if (user.IsActive == false)
            {
                ModelState.AddModelError("", "บัญชีนี้ถูกระงับ กรุณาติดต่อผู้ดูแลระบบ");
                return View(model);
            }

            HttpContext.Session.SetString("UserId", user.UserId);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("DisplayName", user.DisplayName ?? user.Username);
            HttpContext.Session.SetInt32("RoleId", user.RoleId);

            return RedirectByRole(user.RoleId);
        }

        private IActionResult RedirectByRole(int roleId) => roleId switch
        {
            1 or 2 => RedirectToAction("Index", "Home", new { area = "Admin" }),
            3      => RedirectToAction("Index", "Home", new { area = "Publisher" }),
            _      => RedirectToAction("Index", "Home", new { area = "Customer" }),
        };

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (_db.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "อีเมลนี้ถูกใช้แล้ว");
                return View(model);
            }
            if (_db.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "ชื่อผู้ใช้นี้ถูกใช้แล้ว");
                return View(model);
            }

            var user = new User
            {
                UserId = Guid.NewGuid().ToString(),
                Username = model.Username,
                Email = model.Email,
                PasswordHash = model.Password,
                DisplayName = model.DisplayName,
                Country = model.Country,
                RoleId = 4,
                IsActive = true,
                WalletPoints = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _db.Add(user);
            _db.SaveChanges();

            return RedirectToAction("Login");
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public IActionResult Lab9User(Lab9UserViewModel data)
        {
            var u = new Labstudent();
            u.StdId = data.UserId;
            u.StdName = data.Name;
            u.StdLastname = data.Lastname;
            u.StdPassword = data.Password;
            _db.Add(u);
            _db.SaveChanges();
            return RedirectToAction("Lab9User", "Account");
        }

        public IActionResult Lab9User()
        {
            var user = (from u in _db.Labstudents
                        select new Lab9UserViewModel
                        {
                            UserId = u.StdId,
                            Password = u.StdPassword,
                            Name = u.StdName,
                            Lastname = u.StdLastname
                        }).ToList();
            return View(user);
        }


        [HttpPost]
        public IActionResult Lab10(Lab9UserViewModel data)
        {
            var user = (from us in _db.Labstudents where us.StdId == data.UserId select us).FirstOrDefault();

            if (user == null)
            {
                var newStudent = new Labstudent
                {
                    StdId = data.UserId,
                    StdName = data.Name,
                    StdLastname = data.Lastname,
                    StdPassword = data.Password
                };
                _db.Add(newStudent);
            }
            else
            {
                user.StdName = data.Name;
                user.StdLastname = data.Lastname;
                user.StdPassword = data.Password;
                _db.Update(user);
            }

            _db.SaveChanges();

            return RedirectToAction("Lab9User", "Account");
        }

        public IActionResult Lab10(string UID)
        {
            var check = (from us in _db.Labstudents
                         where us.StdId == UID
                         select new Lab9UserViewModel
                         {
                             UserId = us.StdId,
                             Name = us.StdName,
                             Lastname = us.StdLastname,
                             Password = us.StdPassword,
                         }).FirstOrDefault();

            return View(check);
        }

        public IActionResult Lab10Remove(string UID)
        {
            var users = (from us in _db.Labstudents where us.StdId == UID select us).ToList();
            _db.RemoveRange(users);
            _db.SaveChanges();

            return RedirectToAction("Lab9User", "Account");
        }

    }
}