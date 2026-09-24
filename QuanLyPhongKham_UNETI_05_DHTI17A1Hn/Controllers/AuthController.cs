using Microsoft.AspNetCore.Mvc;
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
    public IActionResult Login(string? returnUrl = null)
    {
        if (HttpContext.Session.GetInt32(SessionKeys.AccountId).HasValue)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.LoginAsync(model);
        if (!result.Succeeded || result.Account is null)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Đăng nhập không thành công.");
            return View(model);
        }

        HttpContext.Session.SetInt32(SessionKeys.AccountId, result.Account.MaTaiKhoan);
        HttpContext.Session.SetString(SessionKeys.Username, result.Account.TenDangNhap);
        HttpContext.Session.SetString(SessionKeys.FullName, result.Account.HoTen);
        HttpContext.Session.SetString(SessionKeys.Role, result.Account.VaiTro.ToString());

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return LocalRedirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }
}
