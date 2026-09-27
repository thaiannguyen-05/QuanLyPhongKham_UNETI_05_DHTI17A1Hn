using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Common.Filters;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Specialty.Models;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Specialty.Models.Mapping;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Specialty.Services;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Specialty.Controllers;

[RequireRole(Role.Admin)]
public sealed class SpecialtyController : Controller
{
    private readonly ISpecialtyService _specialtyService;

    public SpecialtyController(ISpecialtyService specialtyService)
    {
        _specialtyService = specialtyService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var specialties = await _specialtyService.ListAsync(cancellationToken);
        return View(new SpecialtyIndexViewModel
        {
            Specialties = specialties
                .Select(SpecialtyMapping.ToListViewModel)
                .ToList()
        });
    }

    [HttpGet]
    public async Task<IActionResult> Details(
        int id,
        CancellationToken cancellationToken)
    {
        var specialty = await _specialtyService.GetByIdAsync(id, cancellationToken);
        return View(SpecialtyMapping.ToDetailViewModel(specialty));
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new SpecialtyFormViewModel
        {
            Status = SpecialtyStatus.Active
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        SpecialtyFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _specialtyService.CreateAsync(
            SpecialtyMapping.ToSaveDto(model),
            cancellationToken);
        TempData["Success"] = "Tạo chuyên khoa thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        int id,
        CancellationToken cancellationToken)
    {
        var specialty = await _specialtyService.GetByIdAsync(id, cancellationToken);
        return View(SpecialtyMapping.ToFormViewModel(specialty));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        SpecialtyFormViewModel model,
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

        await _specialtyService.UpdateAsync(
            id,
            SpecialtyMapping.ToSaveDto(model),
            cancellationToken);
        TempData["Success"] = "Cập nhật chuyên khoa thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var specialty = await _specialtyService.GetByIdAsync(id, cancellationToken);
        return View(SpecialtyMapping.ToDeleteViewModel(specialty));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(
        SpecialtyDeleteViewModel model,
        CancellationToken cancellationToken)
    {
        await _specialtyService.DeleteAsync(model.Id, cancellationToken);
        TempData["Success"] = "Xóa chuyên khoa thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetStatus(
        int id,
        SpecialtyStatus status,
        CancellationToken cancellationToken)
    {
        await _specialtyService.SetStatusAsync(id, status, cancellationToken);
        TempData["Success"] = "Cập nhật trạng thái chuyên khoa thành công.";
        return RedirectToAction(nameof(Index));
    }
}
