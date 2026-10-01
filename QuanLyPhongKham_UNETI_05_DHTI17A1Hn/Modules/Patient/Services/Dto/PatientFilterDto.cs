using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Patient.Services.Dto;

public sealed class PatientFilterDto
{
    public string? SearchTerm { get; set; }
    public PatientStatus? Status { get; set; }
    public string? SortBy { get; set; }
    public int PageNumber { get; set; } = 1;
    public int CurrentPage { get => PageNumber; set => PageNumber = value; }
    public int PageSize { get; set; } = 5;
}
