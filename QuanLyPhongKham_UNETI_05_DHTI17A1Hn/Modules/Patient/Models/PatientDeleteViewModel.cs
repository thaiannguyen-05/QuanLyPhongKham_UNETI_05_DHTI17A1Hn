using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Patient.Services.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Patient.Models;

public sealed class PatientDeleteViewModel
{
    public PatientDto Patient { get; set; } = new();
    public bool CanDelete => Patient.BookingCount == 0;
}
