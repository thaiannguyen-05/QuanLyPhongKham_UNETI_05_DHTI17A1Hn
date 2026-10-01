using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Common.Filters;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Schedule.Models;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Schedule.Models.Mapping;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Schedule.Services;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Schedule.Services.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Schedule.Controllers;

public sealed class ScheduleController : Controller
{
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet]
    [RequireRole(Role.Admin)]
    public async Task<IActionResult> Index([FromQuery] ScheduleFilterDto filter, CancellationToken ct)
    {
        var (items, total) = await _scheduleService.GetPagedAsync(filter, ct);
        return View(BuildIndexVm(filter, items, total, await _scheduleService.GetDoctorDropdownAsync(ct), await _scheduleService.GetSpecialtyDropdownAsync(ct)));
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] ScheduleFilterDto filter, CancellationToken ct)
    {
        var (items, total) = await _scheduleService.GetAvailableAsync(filter, ct);
        return View(BuildIndexVm(filter, items, total, await _scheduleService.GetDoctorDropdownAsync(ct), await _scheduleService.GetSpecialtyDropdownAsync(ct)));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var dto = await _scheduleService.GetByIdAsync(id, ct);
        if (dto.Status is ScheduleStatus.Pending or ScheduleStatus.Rejected)
        {
            var role = HttpContext.Items[AuthItems.Role] as Role?;
            if (role != Role.Admin)
            {
                var doctorId = HttpContext.Items[AuthItems.DoctorId] as int?;
                if (role != Role.Doctor || doctorId != dto.DoctorId)
                {
                    return Forbid();
                }
            }
        }
        return View(new ScheduleDetailViewModel { Schedule = dto });
    }

    [HttpGet]
    [RequireRole(Role.Doctor)]
    public IActionResult Create() => View(new ScheduleFormViewModel());

    [HttpPost]
    [RequireRole(Role.Doctor)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ScheduleFormViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        await _scheduleService.ProposeAsync(GetCurrentDoctorId(), model.ToSaveDto(), ct);
        TempData["Success"] = "Đề xuất lịch thành công, chờ quản trị viên duyệt.";
        return RedirectToAction(nameof(My));
    }

    [HttpGet]
    [RequireRole(Role.Doctor)]
    public async Task<IActionResult> My([FromQuery] ScheduleFilterDto filter, CancellationToken ct)
    {
        var (items, total) = await _scheduleService.GetMyAsync(GetCurrentDoctorId(), filter, ct);
        return View(BuildIndexVm(filter, items, total, await _scheduleService.GetDoctorDropdownAsync(ct), await _scheduleService.GetSpecialtyDropdownAsync(ct)));
    }

    [HttpGet]
    [RequireRole(Role.Doctor)]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var dto = await _scheduleService.GetByIdAsync(id, ct);
        EnsureOwn(dto);
        return View(dto.ToFormViewModel());
    }

    [HttpPost]
    [RequireRole(Role.Doctor)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ScheduleFormViewModel model, CancellationToken ct)
    {
        if (model.Id != id)
        {
            return BadRequest();
        }
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        await _scheduleService.UpdatePendingAsync(GetCurrentDoctorId(), id, model.ToSaveDto(), ct);
        TempData["Success"] = "Cập nhật đề xuất thành công, chuyển về Chờ duyệt.";
        return RedirectToAction(nameof(My));
    }

    [HttpGet]
    [RequireRole(Role.Doctor)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var dto = await _scheduleService.GetByIdAsync(id, ct);
        EnsureOwn(dto);
        return View(new ScheduleDeleteViewModel { Schedule = dto });
    }

    [HttpPost]
    [RequireRole(Role.Doctor)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(ScheduleDeleteViewModel model, CancellationToken ct)
    {
        await _scheduleService.DeletePendingAsync(GetCurrentDoctorId(), model.Schedule.Id, ct);
        TempData["Success"] = "Xóa đề xuất lịch thành công.";
        return RedirectToAction(nameof(My));
    }

    [HttpPost]
    [RequireRole(Role.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id, CancellationToken ct)
    {
        await _scheduleService.ApproveAsync(id, ct);
        TempData["Success"] = "Đã duyệt lịch.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [RequireRole(Role.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id, CancellationToken ct)
    {
        await _scheduleService.RejectAsync(id, ct);
        TempData["Success"] = "Đã từ chối lịch.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [RequireRole(Role.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(int id, CancellationToken ct)
    {
        await _scheduleService.CloseAsync(id, ct);
        TempData["Success"] = "Đã tạm ngừng lịch.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [RequireRole(Role.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reopen(int id, CancellationToken ct)
    {
        await _scheduleService.ReopenAsync(id, ct);
        TempData["Success"] = "Đã mở lại lịch.";
        return RedirectToAction(nameof(Index));
    }

    private int GetCurrentDoctorId()
    {
        if (HttpContext.Items.TryGetValue(AuthItems.DoctorId, out var value) && value is int doctorId && doctorId > 0)
        {
            return doctorId;
        }
        throw new KeyNotFoundException("Chưa có hồ sơ bác sĩ liên kết với tài khoản này. Vui lòng liên hệ quản trị viên.");
    }

    private void EnsureOwn(ScheduleDto dto)
    {
        if (dto.DoctorId != GetCurrentDoctorId())
        {
            throw new KeyNotFoundException("Không tìm thấy thông tin lịch khám.");
        }
    }

    private static ScheduleIndexViewModel BuildIndexVm(ScheduleFilterDto filter,
        IReadOnlyList<ScheduleDto> items, int total,
        IEnumerable<SelectListItem> doctors, IEnumerable<SelectListItem> specialties)
    {
        return new ScheduleIndexViewModel
        {
            Schedules = items,
            DoctorId = filter.DoctorId,
            SpecialtyId = filter.SpecialtyId,
            Date = filter.Date,
            Status = filter.Status,
            CurrentPage = filter.PageNumber < 1 ? 1 : filter.PageNumber,
            PageSize = filter.PageSize < 1 ? 10 : filter.PageSize,
            TotalItems = total,
            DoctorList = doctors,
            SpecialtyList = specialties
        };
    }

}
