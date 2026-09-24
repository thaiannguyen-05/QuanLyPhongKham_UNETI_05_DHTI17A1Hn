using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Filters;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Controllers;

[RequireRole(Role.Admin)]
public sealed class SpecialtyController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
