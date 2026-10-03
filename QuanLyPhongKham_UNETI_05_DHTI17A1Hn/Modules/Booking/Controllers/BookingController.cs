using Microsoft.AspNetCore.Mvc;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Common.Filters;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Booking.Models;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Booking.Models.Mapping;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Booking.Services;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Schedule.Services;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Schedule.Services.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Booking.Controllers;

public sealed class BookingController : Controller
{
    private readonly IBookingService _bookingService;
    private readonly IScheduleService _scheduleService;

    public BookingController(IBookingService bookingService, IScheduleService scheduleService)
    {
        _bookingService = bookingService;
        _scheduleService = scheduleService;
    }

    [HttpGet]
    [RequireRole(Role.Patient)]
    public async Task<IActionResult> Register(int scheduleId, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleService.GetByIdAsync(scheduleId, cancellationToken);

        if (!IsRegistrable(schedule))
        {
            TempData["Error"] = "Lịch khám này hiện không đăng ký được.";
            return RedirectToAction("Search", "Schedule");
        }

        return View(ToViewModel(schedule));
    }

    [HttpPost]
    [RequireRole(Role.Patient)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        BookingRegisterViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await RefillAsync(model, cancellationToken);
            return View(model);
        }

        await _bookingService.RegisterAsync(GetCurrentPatientId(), model.ToRegisterDto(), cancellationToken);
        TempData["Success"] = "Đăng ký khám thành công. Phiếu đang chờ quản trị viên chấp nhận.";
        return RedirectToAction("Search", "Schedule");
    }

    private async Task RefillAsync(BookingRegisterViewModel model, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleService.GetByIdAsync(model.ScheduleId, cancellationToken);
        var filled = ToViewModel(schedule);
        model.DoctorName = filled.DoctorName;
        model.SpecialtyName = filled.SpecialtyName;
        model.Date = filled.Date;
        model.StartTime = filled.StartTime;
        model.EndTime = filled.EndTime;
        model.RemainingSlots = filled.RemainingSlots;
    }

    private static BookingRegisterViewModel ToViewModel(ScheduleDto schedule)
    {
        return new BookingRegisterViewModel
        {
            ScheduleId = schedule.Id,
            DoctorName = schedule.DoctorName,
            SpecialtyName = schedule.SpecialtyName,
            Date = schedule.Date,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime,
            RemainingSlots = schedule.Capacity - schedule.BookedCount
        };
    }

    private static bool IsRegistrable(ScheduleDto schedule)
    {
        if (schedule.Status != ScheduleStatus.Open)
        {
            return false;
        }

        var day = schedule.Date.Date;
        if (day < DateTime.Today
            || (day == DateTime.Today && schedule.StartTime <= DateTime.Now.TimeOfDay))
        {
            return false;
        }

        return schedule.BookedCount < schedule.Capacity;
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
