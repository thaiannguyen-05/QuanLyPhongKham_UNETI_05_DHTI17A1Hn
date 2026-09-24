using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Filters;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Auth;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Auth;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Controllers;

public sealed class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        var currentAccount = HttpContext.Items[RequireVaiTroAttribute.CurrentAccountItemKey]
            as TaiKhoan;

        if (currentAccount is not null)
        {
            return RedirectForRole(currentAccount.VaiTro);
        }

        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.MatKhau = string.Empty;
            return View(model);
        }

        var result = await _authService.LoginAsync(model);
        if (!result.Succeeded || result.Account is null)
        {
            ModelState.AddModelError(
                string.Empty,
                result.ErrorMessage ?? "Tên đăng nhập hoặc mật khẩu không đúng.");
            model.MatKhau = string.Empty;
            return View(model);
        }

        HttpContext.Session.Clear();
        HttpContext.Session.SetInt32(
            SessionKeys.MaTaiKhoan,
            result.Account.MaTaiKhoan);

        return RedirectForRole(result.Account.VaiTro);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    private IActionResult RedirectForRole(VaiTro role)
    {
        return role == VaiTro.Admin
            ? RedirectToAction("Index", "Specialty")
            : RedirectToAction("Index", "Home");
    }
}
