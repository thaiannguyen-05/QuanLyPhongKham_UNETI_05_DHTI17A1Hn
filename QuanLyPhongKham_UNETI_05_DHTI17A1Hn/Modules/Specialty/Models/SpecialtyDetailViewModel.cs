using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Specialty.Models;

public sealed class SpecialtyDetailViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public SpecialtyStatus Status { get; set; }

    public int DoctorCount { get; set; }
}
