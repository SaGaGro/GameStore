using Microsoft.AspNetCore.Mvc;

namespace GameStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}