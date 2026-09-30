using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Common.Filters;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Doctor.Models;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Doctor.Models.Mapping;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Doctor.Services;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Doctor.Services.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Doctor.Controllers;

[RequireRole(Role.Admin)]
public sealed class DoctorController : Controller
{
    private readonly IDoctorService _doctorService;

    public DoctorController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        [FromQuery] DoctorFilterDto filter,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _doctorService.GetPagedAsync(filter, cancellationToken);
        var specialties = await _doctorService.GetSpecialtyDropdownAsync(cancellationToken);

        var viewModel = new DoctorIndexViewModel
        {
            Doctors = items,
            SearchTerm = filter.SearchTerm,
            SpecialtyId = filter.SpecialtyId,
            Status = filter.Status,
            MinFee = filter.MinFee,
            MaxFee = filter.MaxFee,
            SortBy = filter.SortBy,
            CurrentPage = filter.PageNumber < 1 ? 1 : filter.PageNumber,
            PageSize = filter.PageSize < 1 ? 5 : filter.PageSize,
            TotalItems = totalCount,
            SpecialtyList = specialties
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(
        int id,
        CancellationToken cancellationToken)
    {
        var doctor = await _doctorService.GetByIdAsync(id, cancellationToken);
        return View(doctor.ToDetailViewModel());
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new DoctorFormViewModel
        {
            SpecialtyList = await _doctorService.GetSpecialtyDropdownAsync(cancellationToken)
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        DoctorFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.SpecialtyList = await _doctorService.GetSpecialtyDropdownAsync(cancellationToken);
            return View(model);
        }

        await _doctorService.CreateAsync(model.ToSaveDto(), cancellationToken);
        TempData["Success"] = "Thêm mới bác sĩ thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken)
    {
        var doctor = await _doctorService.GetByIdAsync(id, cancellationToken);
        var model = doctor.ToFormViewModel();
        model.SpecialtyList = await _doctorService.GetSpecialtyDropdownAsync(cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        DoctorFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (model.Id != id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            model.SpecialtyList = await _doctorService.GetSpecialtyDropdownAsync(cancellationToken);
            return View(model);
        }

        await _doctorService.UpdateAsync(id, model.ToSaveDto(), cancellationToken);
        TempData["Success"] = "Cập nhật thông tin bác sĩ thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var doctor = await _doctorService.GetByIdAsync(id, cancellationToken);
        return View(doctor.ToDeleteViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        DoctorDeleteViewModel model,
        CancellationToken cancellationToken)
    {
        await _doctorService.DeleteAsync(model.Doctor.Id, cancellationToken);
        TempData["Success"] = "Xóa thông tin bác sĩ thành công.";
        return RedirectToAction(nameof(Index));
    }
}
