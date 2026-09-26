using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Data;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Specialty.Services.Dto;
using SpecialtyEntity = QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Schema.Specialty;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Specialty.Services;

public sealed class SpecialtyService : ISpecialtyService
{
    private const string NotFoundMessage = "Không tìm thấy chuyên khoa.";
    private const string DuplicateNameMessage = "Tên chuyên khoa đã tồn tại.";
    private const string HasDoctorsMessage =
        "Không được xóa: còn bác sĩ thuộc chuyên khoa. Hãy chuyển Tạm ngừng.";

    private readonly AppDbContext _context;

    public SpecialtyService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<SpecialtyDto>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Specialties
            .AsNoTracking()
            .OrderBy(specialty => specialty.Name)
            .Select(specialty => new SpecialtyDto
            {
                Id = specialty.Id,
                Name = specialty.Name,
                Description = specialty.Description,
                Status = specialty.Status,
                DoctorCount = specialty.Doctors.Count()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<SpecialtyDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var specialty = await _context.Specialties
            .AsNoTracking()
            .Where(item => item.Id == id)
            .Select(item => new SpecialtyDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Status = item.Status,
                DoctorCount = item.Doctors.Count()
            })
            .SingleOrDefaultAsync(cancellationToken);

        return specialty ?? throw new KeyNotFoundException(NotFoundMessage);
    }

    public async Task CreateAsync(
        SpecialtySaveDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var name = NormalizeName(dto.Name);
        ValidateName(name);

        if (await _context.Specialties.AnyAsync(
                item => item.Name == name,
                cancellationToken))
        {
            throw new ArgumentException(DuplicateNameMessage, nameof(dto.Name));
        }

        var specialty = new SpecialtyEntity
        {
            Name = name,
            Description = NormalizeDescription(dto.Description),
            Status = dto.Status
        };

        _context.Specialties.Add(specialty);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            _context.Entry(specialty).State = EntityState.Detached;
            if (await _context.Specialties.AnyAsync(
                    item => item.Name == name,
                    cancellationToken))
            {
                throw new ArgumentException(DuplicateNameMessage, nameof(dto.Name));
            }

            throw;
        }
    }

    public async Task UpdateAsync(
        int id,
        SpecialtySaveDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var specialty = await _context.Specialties
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException(NotFoundMessage);
        var name = NormalizeName(dto.Name);
        ValidateName(name);

        if (await _context.Specialties.AnyAsync(
                item => item.Name == name && item.Id != id,
                cancellationToken))
        {
            throw new ArgumentException(DuplicateNameMessage, nameof(dto.Name));
        }

        specialty.Name = name;
        specialty.Description = NormalizeDescription(dto.Description);
        specialty.Status = dto.Status;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            if (await _context.Specialties.AnyAsync(
                    item => item.Name == name && item.Id != id,
                    cancellationToken))
            {
                throw new ArgumentException(DuplicateNameMessage, nameof(dto.Name));
            }

            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var specialty = await _context.Specialties
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException(NotFoundMessage);

        if (await _context.Doctors.AnyAsync(
                doctor => doctor.SpecialtyId == id,
                cancellationToken))
        {
            throw new InvalidOperationException(HasDoctorsMessage);
        }

        _context.Specialties.Remove(specialty);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task SetStatusAsync(
        int id,
        SpecialtyStatus status,
        CancellationToken cancellationToken = default)
    {
        var specialty = await _context.Specialties
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException(NotFoundMessage);

        specialty.Status = status;
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static string NormalizeName(string? name)
    {
        return name?.Trim() ?? string.Empty;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Tên chuyên khoa là bắt buộc.",
                "Name");
        }
    }

    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}
