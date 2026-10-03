using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Schedule.Services.Dto;

public sealed class ScheduleFilterDto
{
    public int? DoctorId { get; set; }
    public int? SpecialtyId { get; set; }
    public DateTime? Date { get; set; }
    public ScheduleStatus? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
