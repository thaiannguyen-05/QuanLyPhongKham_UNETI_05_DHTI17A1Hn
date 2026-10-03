using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Schedule.Services.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Schedule.Models;

public sealed class ScheduleIndexViewModel
{
    public IReadOnlyList<ScheduleDto> Schedules { get; set; } = Array.Empty<ScheduleDto>();
    public int? DoctorId { get; set; }
    public int? SpecialtyId { get; set; }
    public DateTime? Date { get; set; }
    public ScheduleStatus? Status { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalItems { get; set; }
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
    public IEnumerable<SelectListItem> DoctorList { get; set; } = Enumerable.Empty<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
    public IEnumerable<SelectListItem> SpecialtyList { get; set; } = Enumerable.Empty<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
}
