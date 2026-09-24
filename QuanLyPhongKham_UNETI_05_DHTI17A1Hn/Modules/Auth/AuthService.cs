using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Data;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Auth;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Auth;

public sealed class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AuthResult> LoginAsync(
        LoginViewModel model,
        CancellationToken cancellationToken = default)
    {
        var username = model.TenDangNhap.Trim();
        var account = await _context.TaiKhoans
            .AsNoTracking()
            .SingleOrDefaultAsync(
                item => item.TenDangNhap == username,
                cancellationToken);

        if (account is null)
        {
            return AuthResult.Failure("Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        if (AuthHelper.IsLocked(account))
        {
            return AuthResult.Failure(
                "Tài khoản đã bị khóa, không được phép đăng nhập.",
                isLocked: true);
        }

        if (!PasswordHasher.Verify(model.MatKhau, account.MatKhau))
        {
            return AuthResult.Failure("Tên đăng nhập hoặc mật khẩu không đúng.");
        }

        return AuthResult.Success(new AuthenticatedAccount(
            account.MaTaiKhoan,
            account.TenDangNhap,
            account.HoTen,
            account.VaiTro));
    }
}
