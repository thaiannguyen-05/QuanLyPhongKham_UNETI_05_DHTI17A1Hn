using System.ComponentModel.DataAnnotations;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Booking.Models;

public sealed class BookingRegisterViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Lịch khám không hợp lệ.")]
    public int ScheduleId { get; set; }

    [StringLength(500, ErrorMessage = "Lý do khám không được vượt quá 500 ký tự.")]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Lý do khám")]
    public string? Reason { get; set; }

    public string DoctorName { get; set; } = string.Empty;
    public string SpecialtyName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int RemainingSlots { get; set; }
}
