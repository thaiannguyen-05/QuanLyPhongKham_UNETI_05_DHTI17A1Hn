using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Booking.Models;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Booking.Services.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Booking.Models.Mapping;

public static class BookingMapping
{
    public static BookingRegisterDto ToRegisterDto(this BookingRegisterViewModel model)
    {
        return new BookingRegisterDto
        {
            ScheduleId = model.ScheduleId,
            Reason = model.Reason?.Trim()
        };
    }
}
