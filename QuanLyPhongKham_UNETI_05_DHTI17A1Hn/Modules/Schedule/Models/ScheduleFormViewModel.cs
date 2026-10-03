using System.ComponentModel.DataAnnotations;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Schedule.Models;

public sealed class ScheduleFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Ngày khám bắt buộc nhập.")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày khám")]
    public DateTime Date { get; set; } = DateTime.Today.AddDays(1);

    [Required(ErrorMessage = "Giờ bắt đầu bắt buộc nhập.")]
    [Display(Name = "Giờ bắt đầu")]
    public TimeSpan StartTime { get; set; } = new TimeSpan(8, 0, 0);

    [Required(ErrorMessage = "Giờ kết thúc bắt buộc nhập.")]
    [Display(Name = "Giờ kết thúc")]
    public TimeSpan EndTime { get; set; } = new TimeSpan(9, 0, 0);

    [Required(ErrorMessage = "Số lượng tối đa bắt buộc nhập.")]
    [Range(1, 1000, ErrorMessage = "Số lượng tối đa phải > 0.")]
    [Display(Name = "Số lượng tối đa")]
    public int Capacity { get; set; } = 10;
}
