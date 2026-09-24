using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Filters;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Auth;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Auth;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Controllers;

[RequireVaiTro(VaiTro.Admin)]
public sealed class TaiKhoanController : Controller
{
    private readonly ITaiKhoanService _taiKhoanService;

    public TaiKhoanController(ITaiKhoanService taiKhoanService)
    {
        _taiKhoanService = taiKhoanService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var accounts = await _taiKhoanService.GetListAsync(cancellationToken);
        return View(new TaiKhoanIndexViewModel { Accounts = accounts });
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new TaiKhoanFormViewModel
        {
            VaiTro = VaiTro.BenhNhan,
            TrangThai = TrangThaiTaiKhoan.HoatDong
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        TaiKhoanFormViewModel model,
        CancellationToken cancellationToken)
    {
        AddPasswordRequiredError(model);

        if (ModelState.IsValid)
        {
            var result = await _taiKhoanService.CreateAsync(model, cancellationToken);
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
        var model = await _taiKhoanService.GetForEditAsync(id, cancellationToken);
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
        TaiKhoanFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (model.MaTaiKhoan != id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            var result = await _taiKhoanService.UpdateAsync(id, model, cancellationToken);
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
        var currentAccountId = HttpContext.Session.GetInt32(SessionKeys.MaTaiKhoan);
        if (currentAccountId == id)
        {
            TempData["Error"] = "Không thể khóa tài khoản đang đăng nhập.";
            return RedirectToAction(nameof(Index));
        }

        var updated = await _taiKhoanService.SetLockAsync(id, isLocked, cancellationToken);
        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = isLocked
            ? "Đã khóa tài khoản."
            : "Đã mở khóa tài khoản.";
        return RedirectToAction(nameof(Index));
    }

    private void AddPasswordRequiredError(TaiKhoanFormViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.MatKhau))
        {
            ModelState.AddModelError(
                nameof(model.MatKhau),
                "Mật khẩu là bắt buộc.");
        }
    }

    private void AddServiceError(TaiKhoanSaveResult result)
    {
        if (!string.IsNullOrWhiteSpace(result.ErrorField))
        {
            ModelState.AddModelError(result.ErrorField, result.ErrorMessage ?? "Không thể lưu tài khoản.");
            return;
        }

        ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Không thể lưu tài khoản.");
    }
}
