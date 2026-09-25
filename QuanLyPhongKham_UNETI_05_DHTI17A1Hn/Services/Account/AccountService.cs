using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Data;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Services.Account.Dto;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Services.Auth;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Services.Account;

public sealed class AccountService : IAccountService
{
    private const string DuplicateUsernameMessage = "Tên đăng nhập đã tồn tại.";
    private const string NotFoundMessage = "Không tìm thấy tài khoản.";

    private readonly AppDbContext _context;

    public AccountService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<AccountListDto>> GetListAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .AsNoTracking()
            .OrderBy(account => account.Username)
            .Select(account => new AccountListDto
            {
                Id = account.Id,
                Username = account.Username,
                FullName = account.FullName,
                Email = account.Email,
                Role = account.Role,
                Status = account.Status
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<AccountDetailDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var dto = await _context.Accounts
            .AsNoTracking()
            .Where(account => account.Id == id)
            .Select(account => new AccountDetailDto
            {
                Id = account.Id,
                Username = account.Username,
                FullName = account.FullName,
                Email = account.Email,
                Role = account.Role,
                Status = account.Status
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (dto is null)
        {
            throw new KeyNotFoundException(NotFoundMessage);
        }

        return dto;
    }

    public async Task CreateAsync(
        CreateAccountDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        dto.Username = dto.Username?.Trim() ?? string.Empty;
        dto.FullName = dto.FullName?.Trim() ?? string.Empty;
        dto.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            dto.Password = string.Empty;
        }

        var username = dto.Username;

        var account = new Models.Schema.Account
        {
            Username = username,
            PasswordHash = PasswordHasher.Hash(dto.Password),
            FullName = dto.FullName,
            Email = dto.Email,
            Role = dto.Role!.Value,
            Status = dto.Status ?? AccountStatus.Active
        };

        _context.Accounts.Add(account);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            _context.Entry(account).State = EntityState.Detached;

            if (await _context.Accounts.AnyAsync(item => item.Username == username, cancellationToken))
            {
                throw new InvalidOperationException(DuplicateUsernameMessage);
            }

            throw;
        }
    }

    public async Task UpdateAsync(
        int id,
        UpdateAccountDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);
        dto.Username = dto.Username?.Trim() ?? string.Empty;
        dto.FullName = dto.FullName?.Trim() ?? string.Empty;
        dto.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            dto.Password = null;
        }

        var account = await _context.Accounts
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (account is null)
        {
            throw new KeyNotFoundException(NotFoundMessage);
        }

        var username = dto.Username;

        account.Username = username;
        account.FullName = dto.FullName;
        account.Email = dto.Email;
        account.Role = dto.Role!.Value;
        account.Status = dto.Status ?? AccountStatus.Active;

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            account.PasswordHash = PasswordHasher.Hash(dto.Password);
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            if (await _context.Accounts.AnyAsync(
                item => item.Username == username && item.Id != id,
                cancellationToken))
            {
                throw new InvalidOperationException(DuplicateUsernameMessage);
            }

            throw;
        }
    }

    public async Task SetLockAsync(
        int id,
        bool isLocked,
        CancellationToken cancellationToken = default)
    {
        var account = await _context.Accounts
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (account is null)
        {
            throw new KeyNotFoundException(NotFoundMessage);
        }

        account.Status = isLocked
            ? AccountStatus.Locked
            : AccountStatus.Active;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
