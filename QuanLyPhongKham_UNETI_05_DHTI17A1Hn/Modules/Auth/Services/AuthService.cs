using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Data;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Auth.Services;

public sealed class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AuthResult> LoginAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        var trimmedUsername = (username ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(trimmedUsername))
        {
            return AuthResult.Failure("Tên đăng nhập hoặc mật khẩu không đúng.");
        }
        var account = await _context.Accounts
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.Username == trimmedUsername,
                cancellationToken);

        if (account is null)
        {
            return AuthResult.Failure("Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        if (account.Status == AccountStatus.Locked)
        {
            return AuthResult.Failure(
                "Tài khoản đã bị khóa, không được phép đăng nhập.",
                isLocked: true);
        }

        if (!PasswordHasher.Verify(password ?? string.Empty, account.PasswordHash))
        {
            return AuthResult.Failure("Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        return AuthResult.Success(new AuthenticatedAccount(
            account.Id,
            account.Username,
            account.FullName,
            account.Role));
    }
}
