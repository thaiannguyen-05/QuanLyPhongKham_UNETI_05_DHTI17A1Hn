using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Schema;

public class Booking
{
    [Key]
    [Display(Name = "Mã phiếu")]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Bệnh nhân")]
    public int PatientId { get; set; }

    [ForeignKey(nameof(PatientId))]
    public Patient? Patient { get; set; }

    [Required]
    [Display(Name = "Lịch khám")]
    public int ScheduleId { get; set; }

    [ForeignKey(nameof(ScheduleId))]
    public Schedule? Schedule { get; set; }

    [Display(Name = "Ngày đăng ký")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [DataType(DataType.MultilineText)]
    [Display(Name = "Lý do khám")]
    public string? Reason { get; set; }

    [Display(Name = "Trạng thái")]
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
}
