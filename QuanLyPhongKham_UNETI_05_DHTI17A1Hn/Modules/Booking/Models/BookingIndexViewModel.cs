using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Booking.Services.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Booking.Models;

public sealed class BookingIndexViewModel
{
    public IReadOnlyList<BookingDto> Bookings { get; set; } = Array.Empty<BookingDto>();

    public string? SearchTerm { get; set; }
    public BookingStatus? Status { get; set; }

    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalItems { get; set; }
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalItems / PageSize);

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}
