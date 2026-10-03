using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Common.Filters;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Patient.Models;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Patient.Models.Mapping;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Patient.Services;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Patient.Services.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Patient.Controllers;

public sealed class PatientController : Controller
{
    private readonly IPatientService _patientService;

    public PatientController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    [RequireRole(Role.Admin)]
    public async Task<IActionResult> Index(
        [FromQuery] PatientFilterDto filter,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _patientService.GetPagedAsync(filter, cancellationToken);

        var viewModel = new PatientIndexViewModel
        {
            Patients = items,
            SearchTerm = filter.SearchTerm,
            Status = filter.Status,
            SortBy = filter.SortBy,
            CurrentPage = filter.PageNumber < 1 ? 1 : filter.PageNumber,
            PageSize = filter.PageSize < 1 ? 5 : filter.PageSize,
            TotalItems = totalCount
        };

        return View(viewModel);
    }

    [HttpGet]
    [RequireRole(Role.Admin)]
    public async Task<IActionResult> Details(
        int id,
        CancellationToken cancellationToken)
    {
        var patient = await _patientService.GetByIdAsync(id, cancellationToken);
        return View(patient.ToDetailViewModel());
    }

    [HttpGet]
    [RequireRole(Role.Admin)]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new PatientFormViewModel
        {
            AvailableAccounts = await _patientService.GetAvailableAccountsAsync(null, cancellationToken)
        };
        return View(model);
    }

    [HttpPost]
    [RequireRole(Role.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        PatientFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableAccounts = await _patientService.GetAvailableAccountsAsync(null, cancellationToken);
            return View(model);
        }

        await _patientService.CreateAsync(model.ToCreateDto(), cancellationToken);
        TempData["Success"] = "Thêm mới bệnh nhân thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequireRole(Role.Admin)]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken)
    {
        var patient = await _patientService.GetByIdAsync(id, cancellationToken);
        var model = patient.ToFormViewModel();
        model.AvailableAccounts = await _patientService.GetAvailableAccountsAsync(id, cancellationToken);
        return View(model);
    }

    [HttpPost]
    [RequireRole(Role.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        PatientFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (model.Id != id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            model.AvailableAccounts = await _patientService.GetAvailableAccountsAsync(id, cancellationToken);
            return View(model);
        }

        await _patientService.UpdateAsync(id, model.ToSaveDto(), cancellationToken);
        TempData["Success"] = "Cập nhật thông tin bệnh nhân thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequireRole(Role.Admin)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var patient = await _patientService.GetByIdAsync(id, cancellationToken);
        return View(patient.ToDeleteViewModel());
    }

    [HttpPost]
    [RequireRole(Role.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        PatientDeleteViewModel model,
        CancellationToken cancellationToken)
    {
        await _patientService.DeleteAsync(model.Patient.Id, cancellationToken);
        TempData["Success"] = "Xóa thông tin bệnh nhân thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequireRole(Role.Patient)]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var patient = await _patientService.GetByIdAsync(GetCurrentPatientId(), cancellationToken);
        return View(patient.ToOwnViewModel());
    }

    [HttpPost]
    [RequireRole(Role.Patient)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Me(
        PatientOwnViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var patientId = GetCurrentPatientId();
        var current = await _patientService.GetByIdAsync(patientId, cancellationToken);
        await _patientService.UpdateAsync(patientId, current.ToSaveDto(model), cancellationToken);
        TempData["Success"] = "Cập nhật thông tin cá nhân thành công.";

        var refreshed = await _patientService.GetByIdAsync(patientId, cancellationToken);
        return View(refreshed.ToOwnViewModel());
    }

    private int GetCurrentPatientId()
    {
        if (HttpContext.Items.TryGetValue(AuthItems.PatientId, out var value) && value is int patientId)
        {
            return patientId;
        }

        throw new KeyNotFoundException("Chưa có hồ sơ bệnh nhân liên kết với tài khoản này. Vui lòng liên hệ quản trị viên.");
    }
}
