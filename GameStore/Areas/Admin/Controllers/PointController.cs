using Microsoft.AspNetCore.Mvc;
using GameStore.Models.DB;

namespace GameStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PointController : Controller
    {
        private readonly Csi402dbContext _db;

        public PointController(Csi402dbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            // ค่าตั้งค่าระบบแต้ม (คงที่ตามที่ออกแบบไว้)
            ViewBag.EarnRate       = 5;
            ViewBag.EarnPer        = 100;
            ViewBag.PointValue     = 1;
            ViewBag.MaxUsagePercent = 20;
            ViewBag.ExpiryDays     = 180;

            // สถิติจาก DB จริง
            ViewBag.TotalPointsIssued  = _db.Pointhistories.Where(p => p.Type == "Earned").Sum(p => (int?)p.Points) ?? 0;
            ViewBag.TotalPointsUsed    = _db.Pointhistories.Where(p => p.Type == "Used").Sum(p => (int?)Math.Abs(p.Points)) ?? 0;
            ViewBag.TotalPointsExpired = _db.Pointhistories.Where(p => p.Type == "Expired").Sum(p => (int?)Math.Abs(p.Points)) ?? 0;

            return View();
        }
    }
}
