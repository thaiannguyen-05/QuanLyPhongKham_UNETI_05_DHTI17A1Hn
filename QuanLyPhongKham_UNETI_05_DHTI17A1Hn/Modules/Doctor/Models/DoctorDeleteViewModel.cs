using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Doctor.Services.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Doctor.Models;

public sealed class DoctorDeleteViewModel
{
    public DoctorDto Doctor { get; set; } = new();
    public bool CanDelete => Doctor.ScheduleCount == 0;
}
