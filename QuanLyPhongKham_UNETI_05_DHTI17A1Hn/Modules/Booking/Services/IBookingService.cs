using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Booking.Services.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Booking.Services;

public interface IBookingService
{
    Task<int> RegisterAsync(
        int patientId,
        BookingRegisterDto dto,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<BookingDto> Items, int TotalCount)> GetMyAsync(
        int patientId,
        BookingFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<BookingDto> Items, int TotalCount)> GetPagedAsync(
        BookingFilterDto filter,
        CancellationToken cancellationToken = default);
}
