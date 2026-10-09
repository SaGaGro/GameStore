using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameStore.Models.DB;

namespace GameStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PublisherController : Controller
    {
        private readonly Csi402dbContext _db;

        public PublisherController(Csi402dbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var requests = _db.Publisherrequests
                .Include(r => r.User)
                .OrderByDescending(r => r.RequestedAt)
                .ToList();

            return View(requests);
        }

        [HttpPost]
        public IActionResult Approve(int id)
        {
            var req = _db.Publisherrequests.Include(r => r.User).FirstOrDefault(r => r.RequestId == id);
            if (req != null)
            {
                req.Status       = "Approved";
                req.ReviewedAt   = DateTime.Now;
                req.User.RoleId  = 3;
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            var req = _db.Publisherrequests.FirstOrDefault(r => r.RequestId == id);
            if (req != null)
            {
                req.Status     = "Rejected";
                req.ReviewedAt = DateTime.Now;
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
