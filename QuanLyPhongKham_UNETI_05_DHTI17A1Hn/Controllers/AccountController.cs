using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Filters;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Accounts;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Services.Account;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Services.Auth;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Controllers;

[RequireRole(Role.Admin)]
public sealed class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var accounts = await _accountService.GetListAsync(cancellationToken);
        return View(new AccountIndexViewModel { Accounts = accounts });
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new AccountFormViewModel
        {
            Role = Role.Patient,
            Status = AccountStatus.Active
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        AccountFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var result = await _accountService.CreateAsync(model, cancellationToken);
            if (result.Succeeded)
            {
                TempData["Success"] = "Tạo tài khoản thành công.";
                return RedirectToAction(nameof(Index));
            }

            AddServiceError(result);
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var model = await _accountService.GetForEditAsync(id, cancellationToken);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        AccountFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (model.Id != id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            var result = await _accountService.UpdateAsync(id, model, cancellationToken);
            if (result.Succeeded)
            {
                TempData["Success"] = "Cập nhật tài khoản thành công.";
                return RedirectToAction(nameof(Index));
            }

            AddServiceError(result);
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetLock(
        int id,
        bool isLocked,
        CancellationToken cancellationToken)
    {
        var currentAccountId = HttpContext.Session.GetInt32(SessionKeys.AccountId);
        if (currentAccountId == id)
        {
            TempData["Error"] = "Không thể khóa tài khoản đang đăng nhập.";
            return RedirectToAction(nameof(Index));
        }

        var updated = await _accountService.SetLockAsync(id, isLocked, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = isLocked
            ? "Đã khóa tài khoản."
            : "Đã mở khóa tài khoản.";
        return RedirectToAction(nameof(Index));
    }

    private void AddServiceError(AccountSaveResult result)
    {
        if (!string.IsNullOrWhiteSpace(result.ErrorField))
        {
            ModelState.AddModelError(result.ErrorField, result.ErrorMessage ?? "Không thể lưu tài khoản.");
            return;
        }

        ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Không thể lưu tài khoản.");
    }
}
