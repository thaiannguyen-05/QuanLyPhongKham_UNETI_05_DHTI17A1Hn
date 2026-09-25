using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Schema;

public class Schedule
{
    [Key]
    [Display(Name = "Mã lịch khám")]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Bác sĩ")]
    public int DoctorId { get; set; }

    [ForeignKey(nameof(DoctorId))]
    public Doctor? Doctor { get; set; }

    [Required(ErrorMessage = "Ngày khám bắt buộc nhập.")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày khám")]
    public DateTime Date { get; set; }

    [Required]
    [Display(Name = "Giờ bắt đầu")]
    public TimeSpan StartTime { get; set; }

    [Required]
    [Display(Name = "Giờ kết thúc")]
    public TimeSpan EndTime { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Số lượng tối đa phải > 0.")]
    [Display(Name = "Số lượng tối đa")]
    public int Capacity { get; set; }

    [Range(0, int.MaxValue)]
    [Display(Name = "Số lượng đã đăng ký")]
    public int BookedCount { get; set; } = 0;

    [Display(Name = "Trạng thái")]
    public ScheduleStatus Status { get; set; } = ScheduleStatus.Open;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
