using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Data;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Booking.Services.Dto;
using BookingEntity = QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Schema.Booking;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Booking.Services;

public sealed class BookingService : IBookingService
{
    private const string PatientNotFoundMessage = "Chưa có hồ sơ bệnh nhân liên kết với tài khoản này. Vui lòng liên hệ quản trị viên.";
    private const string PatientInactiveMessage = "Hồ sơ bệnh nhân đang tạm ngừng, không thể đăng ký khám.";
    private const string ScheduleNotFoundMessage = "Không tìm thấy thông tin lịch khám.";
    private const string ScheduleFullMessage = "Lịch khám đã đủ chỗ.";
    private const string ScheduleClosedMessage = "Lịch khám đang tạm ngừng, không thể đăng ký.";
    private const string ScheduleNotOpenMessage = "Lịch khám chưa mở đăng ký.";
    private const string ScheduleElapsedMessage = "Lịch khám đã diễn ra, không thể đăng ký.";
    private const string DoctorNotWorkingMessage = "Bác sĩ hiện không làm việc, không thể đăng ký lịch này.";
    private const string DuplicateMessage = "Bạn đã đăng ký lịch khám này.";

    private readonly AppDbContext _context;

    public BookingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> RegisterAsync(
        int patientId,
        BookingRegisterDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.ScheduleId < 1)
        {
            throw new ArgumentException("Lịch khám không hợp lệ.", nameof(dto.ScheduleId));
        }

        dto.Reason = string.IsNullOrWhiteSpace(dto.Reason) ? null : dto.Reason.Trim();

        // 1. Bệnh nhân tồn tại + đang hoạt động (đọc tươi mỗi request).
        var patient = await _context.Patients
            .SingleOrDefaultAsync(p => p.Id == patientId, cancellationToken)
            ?? throw new KeyNotFoundException(PatientNotFoundMessage);

        if (patient.Status != PatientStatus.Active)
        {
            throw new InvalidOperationException(PatientInactiveMessage);
        }

        // 2. Lịch khám tồn tại.
        var schedule = await _context.Schedules
            .Include(s => s.Doctor)
            .SingleOrDefaultAsync(s => s.Id == dto.ScheduleId, cancellationToken)
            ?? throw new KeyNotFoundException(ScheduleNotFoundMessage);

        // 3. Lịch đang mở đăng ký.
        if (schedule.Status != ScheduleStatus.Open)
        {
            throw new InvalidOperationException(schedule.Status switch
            {
                ScheduleStatus.Full => ScheduleFullMessage,
                ScheduleStatus.Closed => ScheduleClosedMessage,
                _ => ScheduleNotOpenMessage
            });
        }

        // 4. Lịch chưa diễn ra.
        var day = schedule.Date.Date;
        if (day < DateTime.Today
            || (day == DateTime.Today && schedule.StartTime <= DateTime.Now.TimeOfDay))
        {
            throw new InvalidOperationException(ScheduleElapsedMessage);
        }

        // 5. Bác sĩ đang làm việc.
        if (schedule.Doctor is null || schedule.Doctor.Status != DoctorStatus.Working)
        {
            throw new InvalidOperationException(DoctorNotWorkingMessage);
        }

        // 6. Chưa trùng (cùng bệnh nhân + lịch, phiếu chưa hủy).
        if (await _context.Bookings.AnyAsync(
            b => b.PatientId == patientId
                && b.ScheduleId == dto.ScheduleId
                && b.Status != BookingStatus.Cancelled,
            cancellationToken))
        {
            throw new InvalidOperationException(DuplicateMessage);
        }

        // 7. Còn slot tham khảo (số chính thức chốt ở #14 lúc chấp nhận).
        if (schedule.BookedCount >= schedule.Capacity)
        {
            throw new InvalidOperationException(ScheduleFullMessage);
        }

        // Tạo phiếu Pending, KHÔNG đụng BookedCount/trạng thái lịch.
        var booking = new BookingEntity
        {
            PatientId = patientId,
            ScheduleId = dto.ScheduleId,
            Reason = dto.Reason,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.Now
        };

        _context.Bookings.Add(booking);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            _context.Entry(booking).State = EntityState.Detached;

            if (await _context.Bookings.AnyAsync(
                b => b.PatientId == patientId
                    && b.ScheduleId == dto.ScheduleId
                    && b.Status != BookingStatus.Cancelled,
                cancellationToken))
            {
                throw new InvalidOperationException(DuplicateMessage);
            }

            throw;
        }

        return booking.Id;
    }
}
