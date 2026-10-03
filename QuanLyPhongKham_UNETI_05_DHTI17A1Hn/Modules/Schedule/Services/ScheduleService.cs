using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Data;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Schedule.Services.Dto;
using ScheduleEntity = QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Schema.Schedule;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Schedule.Services;

public sealed class ScheduleService : IScheduleService
{
    private const string NotFoundMessage = "Không tìm thấy thông tin lịch khám.";
    private const string DoctorNotFoundMessage = "Bác sĩ đã chọn không tồn tại.";
    private const string DoctorNotWorkingMessage = "Bác sĩ hiện không làm việc nên không thể đề xuất lịch mới.";
    private const string OverlapMessage = "Khung giờ bị trùng với lịch khác của bác sĩ.";
    private const string OnlyPendingEditMessage = "Chỉ được sửa lịch đang ở trạng thái Chờ duyệt hoặc Bị từ chối.";
    private const string OnlyPendingDeleteMessage = "Chỉ được xóa lịch đang ở trạng thái Chờ duyệt hoặc Bị từ chối.";
    private const string HasBookingsMessage = "Không được xóa: lịch đã có phiếu đăng ký khám.";
    private const string OnlyPendingApproveMessage = "Chỉ duyệt được lịch đang Chờ duyệt.";
    private const string PastScheduleMessage = "Ngày khám phải từ hôm nay trở đi.";
    private const string WeekWindowMessage = "Chỉ được đề xuất lịch trong 7 ngày tới.";
    private const string FutureTimeMessage = "Lịch phải ở thời điểm tương lai.";
    private const string CapacityReducedMessage = "Không thể giảm số lượng tối đa xuống dưới số đã đăng ký.";

    private readonly AppDbContext _context;

    public ScheduleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IReadOnlyList<ScheduleDto> Items, int TotalCount)> GetPagedAsync(
        ScheduleFilterDto filter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);
        var query = BaseQuery();
        query = ApplyFilter(query, filter);
        query = query.OrderBy(s => s.Date).ThenBy(s => s.StartTime);
        return await ToPagedAsync(query, filter, cancellationToken);
    }

    public async Task<(IReadOnlyList<ScheduleDto> Items, int TotalCount)> GetMyAsync(
        int doctorId, ScheduleFilterDto filter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);
        var query = BaseQuery().Where(s => s.DoctorId == doctorId);
        query = ApplyFilter(query, filter, skipDoctor: true);
        query = query.OrderBy(s => s.Date).ThenBy(s => s.StartTime);
        return await ToPagedAsync(query, filter, cancellationToken);
    }

    public async Task<(IReadOnlyList<ScheduleDto> Items, int TotalCount)> GetAvailableAsync(
        ScheduleFilterDto filter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);
        var now = DateTime.Now;
        var today = DateTime.Today;
        var query = BaseQuery().Where(s => s.Status == ScheduleStatus.Open && s.BookedCount < s.Capacity);
        query = query.Where(s => s.Date > today || (s.Date == today && s.StartTime > now.TimeOfDay));
        query = ApplyFilter(query, filter, onlyOpen: true);
        query = query.OrderBy(s => s.Date).ThenBy(s => s.StartTime);
        return await ToPagedAsync(query, filter, cancellationToken);
    }

    public async Task<ScheduleDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var dto = await BaseQuery().Where(s => s.Id == id).Select(ToDto()).SingleOrDefaultAsync(cancellationToken);
        return dto ?? throw new KeyNotFoundException(NotFoundMessage);
    }

    public async Task ProposeAsync(int doctorId, ScheduleSaveDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var day = dto.Date.Date;
        ValidateTimeRange(dto, day);
        var doctor = await _context.Doctors.SingleOrDefaultAsync(d => d.Id == doctorId, cancellationToken)
            ?? throw new KeyNotFoundException(DoctorNotFoundMessage);
        if (doctor.Status != DoctorStatus.Working)
        {
            throw new InvalidOperationException(DoctorNotWorkingMessage);
        }
        await EnsureNoOverlapAsync(doctorId, day, dto.StartTime, dto.EndTime, null, cancellationToken);
        var entity = new ScheduleEntity
        {
            DoctorId = doctorId,
            Date = day,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Capacity = dto.Capacity,
            BookedCount = 0,
            Status = ScheduleStatus.Pending
        };
        _context.Schedules.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePendingAsync(int doctorId, int id, ScheduleSaveDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var entity = await _context.Schedules.SingleOrDefaultAsync(s => s.Id == id && s.DoctorId == doctorId, cancellationToken)
            ?? throw new KeyNotFoundException(NotFoundMessage);
        if (entity.Status is not (ScheduleStatus.Pending or ScheduleStatus.Rejected))
        {
            throw new InvalidOperationException(OnlyPendingEditMessage);
        }
        if (await HasActiveBookingsAsync(id, cancellationToken))
        {
            throw new InvalidOperationException(HasBookingsMessage);
        }
        var day = dto.Date.Date;
        ValidateTimeRange(dto, day);
        await EnsureNoOverlapAsync(doctorId, day, dto.StartTime, dto.EndTime, id, cancellationToken);
        if (dto.Capacity < entity.BookedCount)
        {
            throw new ArgumentException(CapacityReducedMessage, nameof(dto.Capacity));
        }
        entity.Date = day;
        entity.StartTime = dto.StartTime;
        entity.EndTime = dto.EndTime;
        entity.Capacity = dto.Capacity;
        entity.Status = ScheduleStatus.Pending;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeletePendingAsync(int doctorId, int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Schedules.SingleOrDefaultAsync(s => s.Id == id && s.DoctorId == doctorId, cancellationToken)
            ?? throw new KeyNotFoundException(NotFoundMessage);
        if (entity.Status is not (ScheduleStatus.Pending or ScheduleStatus.Rejected))
        {
            throw new InvalidOperationException(OnlyPendingDeleteMessage);
        }
        if (await HasActiveBookingsAsync(id, cancellationToken))
        {
            throw new InvalidOperationException(HasBookingsMessage);
        }
        _context.Schedules.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ApproveAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Schedules.SingleOrDefaultAsync(s => s.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException(NotFoundMessage);
        if (entity.Status != ScheduleStatus.Pending)
        {
            throw new InvalidOperationException(OnlyPendingApproveMessage);
        }
        var doctor = await _context.Doctors.SingleOrDefaultAsync(d => d.Id == entity.DoctorId, cancellationToken)
            ?? throw new ArgumentException(DoctorNotFoundMessage, nameof(entity.DoctorId));
        if (doctor.Status != DoctorStatus.Working)
        {
            throw new InvalidOperationException(DoctorNotWorkingMessage);
        }
        ValidateFuture(entity.Date, entity.StartTime, entity.EndTime, entity.Capacity);
        await EnsureNoOverlapAsync(entity.DoctorId, entity.Date.Date, entity.StartTime, entity.EndTime, id, cancellationToken);
        entity.Status = ScheduleStatus.Open;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Schedules.SingleOrDefaultAsync(s => s.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException(NotFoundMessage);
        if (entity.Status != ScheduleStatus.Pending)
        {
            throw new InvalidOperationException(OnlyPendingApproveMessage);
        }
        entity.Status = ScheduleStatus.Rejected;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task CloseAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Schedules.SingleOrDefaultAsync(s => s.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException(NotFoundMessage);
        if (entity.Status != ScheduleStatus.Open)
        {
            throw new InvalidOperationException("Chỉ tạm ngừng được lịch đang Còn nhận đăng ký.");
        }
        entity.Status = ScheduleStatus.Closed;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ReopenAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Schedules.SingleOrDefaultAsync(s => s.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException(NotFoundMessage);
        if (entity.Status != ScheduleStatus.Closed)
        {
            throw new InvalidOperationException("Chỉ mở lại được lịch đang Tạm ngừng.");
        }
        var doctor = await _context.Doctors.SingleOrDefaultAsync(d => d.Id == entity.DoctorId, cancellationToken)
            ?? throw new ArgumentException(DoctorNotFoundMessage, nameof(entity.DoctorId));
        if (doctor.Status != DoctorStatus.Working)
        {
            throw new InvalidOperationException(DoctorNotWorkingMessage);
        }
        ValidateFuture(entity.Date, entity.StartTime, entity.EndTime, entity.Capacity);
        await EnsureNoOverlapAsync(entity.DoctorId, entity.Date.Date, entity.StartTime, entity.EndTime, id, cancellationToken);
        entity.Status = ScheduleStatus.Open;
        await _context.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<ScheduleEntity> BaseQuery()
    {
        return _context.Schedules.AsNoTracking().Include(s => s.Doctor).ThenInclude(d => d!.Specialty);
    }

    private static IQueryable<ScheduleEntity> ApplyFilter(IQueryable<ScheduleEntity> query, ScheduleFilterDto filter, bool skipDoctor = false, bool onlyOpen = false)
    {
        if (!skipDoctor && filter.DoctorId.HasValue && filter.DoctorId.Value > 0)
        {
            query = query.Where(s => s.DoctorId == filter.DoctorId.Value);
        }
        if (filter.SpecialtyId.HasValue && filter.SpecialtyId.Value > 0)
        {
            query = query.Where(s => s.Doctor != null && s.Doctor.SpecialtyId == filter.SpecialtyId.Value);
        }
        if (filter.Date.HasValue)
        {
            var day = filter.Date.Value.Date;
            var next = day.AddDays(1);
            query = query.Where(s => s.Date >= day && s.Date < next);
        }
        if (!onlyOpen && filter.Status.HasValue)
        {
            query = query.Where(s => s.Status == filter.Status.Value);
        }
        return query;
    }

    private async Task<(IReadOnlyList<ScheduleDto> Items, int TotalCount)> ToPagedAsync(
        IQueryable<ScheduleEntity> query, ScheduleFilterDto filter, CancellationToken cancellationToken)
    {
        var total = await query.CountAsync(cancellationToken);
        var page = filter.PageNumber < 1 ? 1 : filter.PageNumber;
        var size = filter.PageSize < 1 ? 10 : filter.PageSize;
        var items = await query.Skip((page - 1) * size).Take(size).Select(ToDto()).ToListAsync(cancellationToken);
        return (items, total);
    }

    private static System.Linq.Expressions.Expression<Func<ScheduleEntity, ScheduleDto>> ToDto()
    {
        return s => new ScheduleDto
        {
            Id = s.Id,
            DoctorId = s.DoctorId,
            DoctorName = s.Doctor != null ? s.Doctor.FullName : string.Empty,
            SpecialtyId = s.Doctor != null ? s.Doctor.SpecialtyId : 0,
            SpecialtyName = s.Doctor != null && s.Doctor.Specialty != null ? s.Doctor.Specialty.Name : string.Empty,
            Date = s.Date,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            Capacity = s.Capacity,
            BookedCount = s.BookedCount,
            Status = s.Status
        };
    }

    private static void ValidateTimeRange(ScheduleSaveDto dto, DateTime day)
    {
        if (day < DateTime.Today)
        {
            throw new ArgumentException(PastScheduleMessage, nameof(dto.Date));
        }
        if (day > DateTime.Today.AddDays(7))
        {
            throw new ArgumentException(WeekWindowMessage, nameof(dto.Date));
        }
        ValidateFuture(day, dto.StartTime, dto.EndTime, dto.Capacity);
    }

    private static void ValidateFuture(DateTime day, TimeSpan start, TimeSpan end, int capacity)
    {
        if (day.Date == DateTime.Today && start <= DateTime.Now.TimeOfDay)
        {
            throw new ArgumentException(FutureTimeMessage, nameof(start));
        }
        if (start >= end)
        {
            throw new ArgumentException("Giờ bắt đầu phải trước giờ kết thúc.", nameof(start));
        }
        if (capacity <= 0)
        {
            throw new ArgumentException("Số lượng tối đa phải > 0.", nameof(capacity));
        }
    }

    private async Task EnsureNoOverlapAsync(int doctorId, DateTime day, TimeSpan start, TimeSpan end, int? excludeId, CancellationToken ct)
    {
        var next = day.AddDays(1);
        var overlap = await _context.Schedules.AnyAsync(s =>
            s.DoctorId == doctorId && s.Date >= day && s.Date < next &&
            s.StartTime < end && s.EndTime > start &&
            s.Status != ScheduleStatus.Closed && s.Status != ScheduleStatus.Rejected &&
            (!excludeId.HasValue || s.Id != excludeId.Value), ct);
        if (overlap)
        {
            throw new InvalidOperationException(OverlapMessage);
        }
    }

    private async Task<bool> HasActiveBookingsAsync(int scheduleId, CancellationToken ct)
    {
        return await _context.Bookings.AnyAsync(b =>
            b.ScheduleId == scheduleId && b.Status != BookingStatus.Cancelled, ct);
    }

    public async Task<IEnumerable<SelectListItem>> GetDoctorDropdownAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Doctors
            .AsNoTracking()
            .OrderBy(d => d.FullName)
            .Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.FullName
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SelectListItem>> GetSpecialtyDropdownAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Specialties
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            })
            .ToListAsync(cancellationToken);
}
}
