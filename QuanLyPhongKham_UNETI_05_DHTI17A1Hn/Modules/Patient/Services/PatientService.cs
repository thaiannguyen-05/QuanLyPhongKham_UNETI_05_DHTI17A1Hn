using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Data;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Auth.Services;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Patient.Services.Dto;
using AccountEntity = QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Schema.Account;
using PatientEntity = QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Schema.Patient;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Patient.Services;

public sealed class PatientService : IPatientService
{
    private const string NotFoundMessage = "Không tìm thấy thông tin bệnh nhân.";
    private const string DuplicateUsernameMessage = "Tên đăng nhập đã tồn tại.";
    private const string AccountNotFoundMessage = "Tài khoản liên kết đã chọn không tồn tại.";
    private const string AccountAlreadyLinkedMessage = "Tài khoản này đã liên kết với một bệnh nhân khác.";
    private const string AccountMustBePatientMessage = "Chỉ tài khoản vai trò Bệnh nhân mới được liên kết hồ sơ.";
    private const string HasBookingsMessage = "Không được xóa: bệnh nhân còn phiếu đăng ký khám. Hãy chuyển sang Tạm ngừng.";

    private readonly AppDbContext _context;

    public PatientService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IReadOnlyList<PatientDto> Items, int TotalCount)> GetPagedAsync(
        PatientFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(filter);

        var query = _context.Patients
            .AsNoTracking()
            .Include(p => p.Account)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.Where(p =>
                p.FullName.Contains(term) ||
                (p.Phone != null && p.Phone.Contains(term)) ||
                (p.Email != null && p.Email.Contains(term)));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(p => p.Status == filter.Status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
        var pageSize = filter.PageSize < 1 ? 5 : filter.PageSize;

        query = filter.SortBy switch
        {
            "name_asc" => query.OrderBy(p => p.FullName),
            "name_desc" => query.OrderByDescending(p => p.FullName),
            "oldest" => query.OrderBy(p => p.RegisteredAt),
            _ => query.OrderByDescending(p => p.Id)
        };

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PatientDto
            {
                Id = p.Id,
                FullName = p.FullName,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender,
                Phone = p.Phone,
                Email = p.Email,
                Address = p.Address,
                RegisteredAt = p.RegisteredAt,
                Status = p.Status,
                AccountId = p.AccountId,
                Username = p.Account != null ? p.Account.Username : null,
                BookingCount = p.Bookings.Count()
            })
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<PatientDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var dto = await _context.Patients
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PatientDto
            {
                Id = p.Id,
                FullName = p.FullName,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender,
                Phone = p.Phone,
                Email = p.Email,
                Address = p.Address,
                RegisteredAt = p.RegisteredAt,
                Status = p.Status,
                AccountId = p.AccountId,
                Username = p.Account != null ? p.Account.Username : null,
                BookingCount = p.Bookings.Count()
            })
            .SingleOrDefaultAsync(cancellationToken);

        return dto ?? throw new KeyNotFoundException(NotFoundMessage);
    }

    public async Task CreateAsync(
        PatientCreateDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        NormalizeSaveDto(dto);

        if (string.IsNullOrWhiteSpace(dto.FullName))
        {
            throw new ArgumentException("Họ tên bệnh nhân bắt buộc nhập.", nameof(dto.FullName));
        }

        ValidateDateOfBirth(dto.DateOfBirth, nameof(dto.DateOfBirth));

        if (dto.AccountId.HasValue && dto.CreateAccount)
        {
            throw new InvalidOperationException("Chỉ chọn một trong hai: liên kết tài khoản có sẵn hoặc tạo tài khoản mới.");
        }

        var patient = new PatientEntity
        {
            FullName = dto.FullName.Trim(),
            DateOfBirth = dto.DateOfBirth,
            Gender = string.IsNullOrWhiteSpace(dto.Gender) ? null : dto.Gender,
            Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim(),
            Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim(),
            Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim(),
            Status = dto.Status,
            RegisteredAt = DateTime.Today
        };

        if (dto.AccountId.HasValue)
        {
            await ValidateLinkableAccountAsync(dto.AccountId.Value, null, cancellationToken);
            patient.AccountId = dto.AccountId.Value;
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync(cancellationToken);
            return;
        }

        if (dto.CreateAccount)
        {
            var username = (dto.NewUsername ?? string.Empty).Trim();
            var password = dto.NewPassword ?? string.Empty;

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Tên đăng nhập bắt buộc nhập khi tạo tài khoản.", nameof(dto.NewUsername));
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                throw new ArgumentException("Mật khẩu phải từ 6 đến 100 ký tự.", nameof(dto.NewPassword));
            }

            if (await _context.Accounts.AnyAsync(a => a.Username == username, cancellationToken))
            {
                throw new InvalidOperationException(DuplicateUsernameMessage);
            }

            var account = new AccountEntity
            {
                Username = username,
                PasswordHash = PasswordHasher.Hash(password),
                FullName = patient.FullName,
                Email = patient.Email,
                Role = Role.Patient,
                Status = AccountStatus.Active
            };

            patient.Account = account;
            _context.Patients.Add(patient);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                _context.Entry(patient).State = EntityState.Detached;
                if (await _context.Accounts.AnyAsync(a => a.Username == username, cancellationToken))
                {
                    throw new InvalidOperationException(DuplicateUsernameMessage);
                }

                throw;
            }

            return;
        }

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        int id,
        PatientSaveDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        NormalizeSaveDto(dto);

        if (string.IsNullOrWhiteSpace(dto.FullName))
        {
            throw new ArgumentException("Họ tên bệnh nhân bắt buộc nhập.", nameof(dto.FullName));
        }

        ValidateDateOfBirth(dto.DateOfBirth, nameof(dto.DateOfBirth));

        var patient = await _context.Patients
            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException(NotFoundMessage);

        if (dto.AccountId.HasValue)
        {
            await ValidateLinkableAccountAsync(dto.AccountId.Value, id, cancellationToken);
        }

        patient.FullName = dto.FullName.Trim();
        patient.DateOfBirth = dto.DateOfBirth;
        patient.Gender = string.IsNullOrWhiteSpace(dto.Gender) ? null : dto.Gender;
        patient.Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();
        patient.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
        patient.Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim();
        patient.Status = dto.Status;
        patient.AccountId = dto.AccountId;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var patient = await _context.Patients
            .Include(p => p.Bookings)
            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException(NotFoundMessage);

        if (patient.Bookings.Any())
        {
            throw new InvalidOperationException(HasBookingsMessage);
        }

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<SelectListItem>> GetAvailableAccountsAsync(
        int? excludePatientId = null,
        CancellationToken cancellationToken = default)
    {
        var linkedAccountIds = _context.Patients
            .AsNoTracking()
            .Where(p => p.AccountId.HasValue)
            .Where(p => !excludePatientId.HasValue || p.Id != excludePatientId.Value)
            .Select(p => p.AccountId!.Value);

        return await _context.Accounts
            .AsNoTracking()
            .Where(a => a.Role == Role.Patient && a.Status == AccountStatus.Active)
            .Where(a => !linkedAccountIds.Contains(a.Id))
            .OrderBy(a => a.Username)
            .Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = $"{a.Username} — {a.FullName}"
            })
            .ToListAsync(cancellationToken);
    }

    private static void NormalizeSaveDto(PatientSaveDto dto)
    {
        dto.FullName = dto.FullName?.Trim() ?? string.Empty;
        dto.Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();
        dto.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
        dto.Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim();
        dto.Gender = string.IsNullOrWhiteSpace(dto.Gender) ? null : dto.Gender.Trim();
    }

    private static void ValidateDateOfBirth(DateTime? dateOfBirth, string paramName)
    {
        if (!dateOfBirth.HasValue)
        {
            return;
        }

        if (dateOfBirth.Value.Date > DateTime.Today)
        {
            throw new ArgumentException("Ngày sinh không được lớn hơn ngày hiện tại.", paramName);
        }

        if (dateOfBirth.Value.Year < 1900)
        {
            throw new ArgumentException("Ngày sinh không hợp lệ.", paramName);
        }
    }

    private async Task ValidateLinkableAccountAsync(
        int accountId,
        int? excludePatientId,
        CancellationToken cancellationToken)
    {
        var account = await _context.Accounts
            .AsNoTracking()
            .SingleOrDefaultAsync(a => a.Id == accountId, cancellationToken);

        if (account is null)
        {
            throw new ArgumentException(AccountNotFoundMessage, nameof(PatientSaveDto.AccountId));
        }

        if (account.Role != Role.Patient)
        {
            throw new ArgumentException(AccountMustBePatientMessage, nameof(PatientSaveDto.AccountId));
        }

        var alreadyLinked = await _context.Patients
            .AsNoTracking()
            .AnyAsync(
                p => p.AccountId == accountId && (!excludePatientId.HasValue || p.Id != excludePatientId.Value),
                cancellationToken);

        if (alreadyLinked)
        {
            throw new ArgumentException(AccountAlreadyLinkedMessage, nameof(PatientSaveDto.AccountId));
        }
    }
}
