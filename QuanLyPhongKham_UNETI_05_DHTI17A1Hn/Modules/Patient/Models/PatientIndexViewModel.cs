using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Patient.Services.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Patient.Models;

public sealed class PatientIndexViewModel
{
    public IReadOnlyList<PatientDto> Patients { get; set; } = Array.Empty<PatientDto>();

    public string? SearchTerm { get; set; }
    public PatientStatus? Status { get; set; }
    public string? SortBy { get; set; }

    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public int TotalItems { get; set; }
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalItems / PageSize);

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}
