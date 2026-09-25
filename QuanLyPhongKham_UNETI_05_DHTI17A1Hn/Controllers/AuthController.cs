using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Common.Filters;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Schema;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Auth;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Services.Auth;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Controllers;

[RequireRole]
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
        if (HttpContext.Items[AuthItems.Role] is Role currentRole)
        {
            return RedirectForRole(currentRole);
        }

        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Password = string.Empty;
            return View(model);
        }

        AuthResult result;
        try
        {
            result = await _authService.LoginAsync(model.Username, model.Password);
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "Không thể đăng nhập lúc này, vui lòng thử lại.");
            model.Password = string.Empty;
            return View(model);
        }

        if (!result.Succeeded || result.Account is null)
        {
            ModelState.AddModelError(
                string.Empty,
                result.ErrorMessage ?? "Tên đăng nhập hoặc mật khẩu không đúng.");
            model.Password = string.Empty;
            return View(model);
        }

        HttpContext.Session.Clear();
        HttpContext.Session.SetInt32(
            SessionKeys.AccountId,
            result.Account.Id);

        return RedirectForRole(result.Account.Role);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    private IActionResult RedirectForRole(Role role)
    {
        return role == Role.Admin
            ? RedirectToAction("Index", "Specialty")
            : RedirectToAction("Index", "Home");
    }
}
