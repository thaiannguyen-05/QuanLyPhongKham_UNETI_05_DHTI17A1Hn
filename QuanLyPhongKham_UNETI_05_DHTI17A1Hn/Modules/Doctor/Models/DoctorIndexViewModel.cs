using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Doctor.Services.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Doctor.Models;

public sealed class DoctorIndexViewModel
{
    public IReadOnlyList<DoctorDto> Doctors { get; set; } = Array.Empty<DoctorDto>();

    public string? SearchTerm { get; set; }
    public int? SpecialtyId { get; set; }
    public DoctorStatus? Status { get; set; }
    public decimal? MinFee { get; set; }
    public decimal? MaxFee { get; set; }
    public string? SortBy { get; set; }

    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

    public IEnumerable<SelectListItem> SpecialtyList { get; set; } = Enumerable.Empty<SelectListItem>();
}
