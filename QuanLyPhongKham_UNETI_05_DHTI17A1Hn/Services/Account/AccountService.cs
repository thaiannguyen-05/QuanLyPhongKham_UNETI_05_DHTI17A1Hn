using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Data;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Accounts;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Services.Auth;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Services.Account;

public sealed class AccountService : IAccountService
{
    private const string DuplicateUsernameMessage = "Tên đăng nhập đã tồn tại.";

    private readonly AppDbContext _context;

    public AccountService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<AccountListViewModel>> GetListAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .AsNoTracking()
            .OrderBy(account => account.Username)
            .Select(account => new AccountListViewModel
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

    public async Task<AccountFormViewModel?> GetForEditAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Accounts
            .AsNoTracking()
            .Where(account => account.Id == id)
            .Select(account => new AccountFormViewModel
            {
                Id = account.Id,
                Username = account.Username,
                FullName = account.FullName,
                Email = account.Email,
                Role = account.Role,
                Status = account.Status
            })
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<AccountSaveResult> CreateAsync(
        AccountFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        var username = ValidateUsername(model.Username);
        if (username is null)
        {
            return AccountSaveResult.Failure(
                nameof(model.Username),
                "Tên đăng nhập là bắt buộc.");
        }

        if (string.IsNullOrWhiteSpace(model.Password))
        {
            return AccountSaveResult.Failure(
                nameof(model.Password),
                "Mật khẩu là bắt buộc.");
        }

        var roleError = ValidateRole(model.Role);
        if (roleError is not null)
        {
            return roleError;
        }

        if (await IsDuplicateAsync(username, null, cancellationToken))
        {
            return AccountSaveResult.Failure(
                nameof(model.Username),
                DuplicateUsernameMessage);
        }

        var account = new Models.Account
        {
            Username = username,
            PasswordHash = PasswordHasher.Hash(model.Password),
            Role = model.Role!.Value,
        };
        ApplyCommon(account, model, username);

        _context.Accounts.Add(account);
        return await SaveWithDuplicateHandlingAsync(account, username, cancellationToken);
    }

    public async Task<AccountSaveResult> UpdateAsync(
        int id,
        AccountFormViewModel model,
        CancellationToken cancellationToken = default)
    {
        var account = await _context.Accounts
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (account is null)
        {
            return AccountSaveResult.Failure(
                nameof(model.Id),
                "Không tìm thấy tài khoản.");
        }

        var username = ValidateUsername(model.Username);
        if (username is null)
        {
            return AccountSaveResult.Failure(
                nameof(model.Username),
                "Tên đăng nhập là bắt buộc.");
        }

        var roleError = ValidateRole(model.Role);
        if (roleError is not null)
        {
            return roleError;
        }

        if (await IsDuplicateAsync(username, id, cancellationToken))
        {
            return AccountSaveResult.Failure(
                nameof(model.Username),
                DuplicateUsernameMessage);
        }

        account.Username = username;
        ApplyCommon(account, model, username);

        if (!string.IsNullOrWhiteSpace(model.Password))
        {
            account.PasswordHash = PasswordHasher.Hash(model.Password);
        }

        return await SaveWithDuplicateHandlingAsync(account, username, cancellationToken);
    }

    public async Task<bool> SetLockAsync(
        int id,
        bool isLocked,
        CancellationToken cancellationToken = default)
    {
        var account = await _context.Accounts
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (account is null)
        {
            return false;
        }

        account.Status = isLocked
            ? AccountStatus.Locked
            : AccountStatus.Active;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static string? ValidateUsername(string? username)
    {
        var trimmed = username?.Trim() ?? string.Empty;
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }

    private static AccountSaveResult? ValidateRole(Role? role)
    {
        return role.HasValue
            ? null
            : AccountSaveResult.Failure(
                nameof(AccountFormViewModel.Role),
                "Vai trò là bắt buộc.");
    }

    private static void ApplyCommon(
        Models.Account account,
        AccountFormViewModel model,
        string username)
    {
        account.Username = username;
        account.FullName = model.FullName.Trim();
        account.Email = NormalizeEmail(model.Email);
        account.Role = model.Role!.Value;
        account.Status = model.Status ?? AccountStatus.Active;
    }

    private async Task<bool> IsDuplicateAsync(
        string username,
        int? id,
        CancellationToken cancellationToken)
    {
        return id.HasValue
            ? await _context.Accounts.AnyAsync(
                account => account.Username == username
                    && account.Id != id.Value,
                cancellationToken)
            : await _context.Accounts.AnyAsync(
                account => account.Username == username,
                cancellationToken);
    }

    private async Task<AccountSaveResult> SaveWithDuplicateHandlingAsync(
        Models.Account account,
        string username,
        CancellationToken cancellationToken)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return AccountSaveResult.Success();
        }
        catch (DbUpdateException)
        {
            _context.Entry(account).State = EntityState.Detached;

            if (await _context.Accounts.AnyAsync(
                    item => item.Username == username,
                    cancellationToken))
            {
                return AccountSaveResult.Failure(
                    nameof(AccountFormViewModel.Username),
                    DuplicateUsernameMessage);
            }

            throw;
        }
    }

    private static string? NormalizeEmail(string? email)
    {
        return string.IsNullOrWhiteSpace(email) ? null : email.Trim();
    }
}
