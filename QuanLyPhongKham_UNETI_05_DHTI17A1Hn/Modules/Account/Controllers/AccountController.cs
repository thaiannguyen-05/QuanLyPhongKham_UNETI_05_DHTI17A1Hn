using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Common.Filters;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Account.Models;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Account.Models.Mapping;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Account.Services;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Auth.Services;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Account.Controllers;

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
        var dtos = await _accountService.GetListAsync(cancellationToken);
        return View(new AccountIndexViewModel
        {
            Accounts = dtos.Select(AccountMapping.ToListViewModel).ToList()
        });
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
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _accountService.CreateAsync(AccountMapping.ToCreateDto(model), cancellationToken);
        TempData["Success"] = "Tạo tài khoản thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var dto = await _accountService.GetByIdAsync(id, cancellationToken);
        return View(AccountMapping.ToFormViewModel(dto));
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

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _accountService.UpdateAsync(id, AccountMapping.ToUpdateDto(model), cancellationToken);
        TempData["Success"] = "Cập nhật tài khoản thành công.";
        return RedirectToAction(nameof(Index));
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

        await _accountService.SetLockAsync(id, isLocked, cancellationToken);
        TempData["Success"] = isLocked
            ? "Đã khóa tài khoản."
            : "Đã mở khóa tài khoản.";
        return RedirectToAction(nameof(Index));
    }
}
