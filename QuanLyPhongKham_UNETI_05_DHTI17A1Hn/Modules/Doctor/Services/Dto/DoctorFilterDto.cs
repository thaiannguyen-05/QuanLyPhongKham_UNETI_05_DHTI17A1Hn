using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Doctor.Services.Dto;

public sealed class DoctorFilterDto
{
    public string? SearchTerm { get; set; }
    public int? SpecialtyId { get; set; }
    public DoctorStatus? Status { get; set; }
    public string? Gender { get; set; }
    public decimal? MinFee { get; set; }
    public decimal? MaxFee { get; set; }
    public string? SortBy { get; set; }
    public int PageNumber { get; set; } = 1;
    public int CurrentPage { get => PageNumber; set => PageNumber = value; }
    public int PageSize { get; set; } = 5;
}
