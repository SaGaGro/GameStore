using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GameStore.Models;
using GameStore.Models.DB;
using GameStore.ViewModels;

namespace GameStore.Controllers;

public class HomeController : Controller
{

    private readonly Csi402dbContext _db;

    public HomeController(Csi402dbContext db)
    {
        _db = db;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Lab8()
    {
        var user = (from u in _db.Users select u).ToList();
        return View(user);
    }

}
