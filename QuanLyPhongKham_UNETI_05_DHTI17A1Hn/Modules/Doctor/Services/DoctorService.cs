using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Data;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Doctor.Services.Dto;
using DoctorEntity = QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Schema.Doctor;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Doctor.Services;

public sealed class DoctorService : IDoctorService
{
    private const string NotFoundMessage = "Không tìm thấy thông tin bác sĩ.";
    private const string SpecialtyNotFoundMessage = "Chuyên khoa đã chọn không tồn tại.";
    private const string HasSchedulesMessage = "Không thể xóa bác sĩ đã có lịch khám trong hệ thống. Vui lòng chuyển trạng thái sang 'Ngừng làm việc'.";

    private readonly AppDbContext _context;

    public DoctorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IReadOnlyList<DoctorDto> Items, int TotalCount)> GetPagedAsync(
        DoctorFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        var query = _context.Doctors
            .AsNoTracking()
            .Include(d => d.Specialty)
            .AsQueryable();

        // 1. Tìm kiếm theo Từ khóa (Họ tên, SĐT, Trình độ)
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.Where(d =>
                d.FullName.Contains(term) ||
                (d.Phone != null && d.Phone.Contains(term)) ||
                (d.Qualification != null && d.Qualification.Contains(term)));
        }

        // 2. Lọc theo Chuyên khoa
        if (filter.SpecialtyId.HasValue && filter.SpecialtyId.Value > 0)
        {
            query = query.Where(d => d.SpecialtyId == filter.SpecialtyId.Value);
        }

        // 3. Lọc theo Trạng thái
        if (filter.Status.HasValue)
        {
            query = query.Where(d => d.Status == filter.Status.Value);
        }

        // 4. Lọc theo Phí khám (Khoảng giá)
        if (filter.MinFee.HasValue)
        {
            query = query.Where(d => d.ConsultationFee >= filter.MinFee.Value);
        }
        if (filter.MaxFee.HasValue)
        {
            query = query.Where(d => d.ConsultationFee <= filter.MaxFee.Value);
        }

        // Đếm tổng số bản ghi sau khi tìm kiếm/lọc
        var totalCount = await query.CountAsync(cancellationToken);

        // 5. Sắp xếp
        query = filter.SortBy switch
        {
            "name_desc" => query.OrderByDescending(d => d.FullName),
            "name_asc" => query.OrderBy(d => d.FullName),
            "fee_desc" => query.OrderByDescending(d => d.ConsultationFee),
            "fee_asc" => query.OrderBy(d => d.ConsultationFee),
            "exp_desc" => query.OrderByDescending(d => d.YearsOfExperience),
            "exp_asc" => query.OrderBy(d => d.YearsOfExperience),
            _ => query.OrderByDescending(d => d.Id)
        };

        // 6. Phân trang bằng Skip và Take trên Database
        var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
        var pageSize = filter.PageSize < 1 ? 5 : filter.PageSize;

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new DoctorDto
            {
                Id = d.Id,
                FullName = d.FullName,
                SpecialtyId = d.SpecialtyId,
                SpecialtyName = d.Specialty != null ? d.Specialty.Name : string.Empty,
                DateOfBirth = d.DateOfBirth,
                Gender = d.Gender,
                Phone = d.Phone,
                Email = d.Email,
                Qualification = d.Qualification,
                YearsOfExperience = d.YearsOfExperience,
                ConsultationFee = d.ConsultationFee,
                Status = d.Status,
                ScheduleCount = d.Schedules.Count()
            })
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<DoctorDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var doctor = await _context.Doctors
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => new DoctorDto
            {
                Id = d.Id,
                FullName = d.FullName,
                SpecialtyId = d.SpecialtyId,
                SpecialtyName = d.Specialty != null ? d.Specialty.Name : string.Empty,
                DateOfBirth = d.DateOfBirth,
                Gender = d.Gender,
                Phone = d.Phone,
                Email = d.Email,
                Qualification = d.Qualification,
                YearsOfExperience = d.YearsOfExperience,
                ConsultationFee = d.ConsultationFee,
                Status = d.Status,
                ScheduleCount = d.Schedules.Count()
            })
            .SingleOrDefaultAsync(cancellationToken);

        return doctor ?? throw new KeyNotFoundException(NotFoundMessage);
    }

    public async Task CreateAsync(
        DoctorSaveDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (!await _context.Specialties.AnyAsync(s => s.Id == dto.SpecialtyId, cancellationToken))
        {
            throw new ArgumentException(SpecialtyNotFoundMessage, nameof(dto.SpecialtyId));
        }

        var doctor = new DoctorEntity
        {
            FullName = dto.FullName.Trim(),
            SpecialtyId = dto.SpecialtyId,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim(),
            Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
            Qualification = string.IsNullOrWhiteSpace(dto.Qualification) ? null : dto.Qualification.Trim(),
            YearsOfExperience = dto.YearsOfExperience,
            ConsultationFee = dto.ConsultationFee,
            Status = dto.Status
        };

        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        int id,
        DoctorSaveDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var doctor = await _context.Doctors
            .SingleOrDefaultAsync(d => d.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException(NotFoundMessage);

        if (!await _context.Specialties.AnyAsync(s => s.Id == dto.SpecialtyId, cancellationToken))
        {
            throw new ArgumentException(SpecialtyNotFoundMessage, nameof(dto.SpecialtyId));
        }

        doctor.FullName = dto.FullName.Trim();
        doctor.SpecialtyId = dto.SpecialtyId;
        doctor.DateOfBirth = dto.DateOfBirth;
        doctor.Gender = dto.Gender;
        doctor.Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();
        doctor.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
        doctor.Qualification = string.IsNullOrWhiteSpace(dto.Qualification) ? null : dto.Qualification.Trim();
        doctor.YearsOfExperience = dto.YearsOfExperience;
        doctor.ConsultationFee = dto.ConsultationFee;
        doctor.Status = dto.Status;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var doctor = await _context.Doctors
            .Include(d => d.Schedules)
            .SingleOrDefaultAsync(d => d.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException(NotFoundMessage);

        if (doctor.Schedules.Any())
        {
            throw new InvalidOperationException(HasSchedulesMessage);
        }

        _context.Doctors.Remove(doctor);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<SelectListItem>> GetSpecialtyDropdownAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Specialties
            .AsNoTracking()
            .Where(s => s.Status == SpecialtyStatus.Active)
            .OrderBy(s => s.Name)
            .Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            })
            .ToListAsync(cancellationToken);
    }
}
