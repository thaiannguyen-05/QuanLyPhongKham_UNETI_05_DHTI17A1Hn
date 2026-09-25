using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Filters;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Controllers;

[RequireVaiTro]
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
