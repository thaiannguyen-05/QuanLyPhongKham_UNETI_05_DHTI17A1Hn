using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Common.Filters;

using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Schema;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Controllers;

[RequireRole]
public class HomeController : Controller
{
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
}
