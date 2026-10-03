using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Patient.Services.Dto;

public sealed class PatientDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime RegisteredAt { get; set; }
    public PatientStatus Status { get; set; }
    public int? AccountId { get; set; }
    public string? Username { get; set; }
    public int BookingCount { get; set; }
}
