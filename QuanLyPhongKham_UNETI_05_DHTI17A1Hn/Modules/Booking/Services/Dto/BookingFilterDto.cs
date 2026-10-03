using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Booking.Services.Dto;

public sealed class BookingFilterDto
{
    public string? SearchTerm { get; set; }
    public BookingStatus? Status { get; set; }
    public int? DoctorId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int CurrentPage { get => PageNumber; set => PageNumber = value; }
    public int PageSize { get; set; } = 10;
}
