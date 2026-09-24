using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Auth;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Auth;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginViewModel model, CancellationToken cancellationToken = default);
}

public sealed record AuthenticatedAccount(
    int MaTaiKhoan,
    string TenDangNhap,
    string HoTen,
    VaiTro VaiTro);

public sealed record AuthResult(
    bool Succeeded,
    AuthenticatedAccount? Account,
    string? ErrorMessage,
    bool IsLocked = false)
{
    public static AuthResult Success(AuthenticatedAccount account)
    {
        return new AuthResult(true, account, null);
    }

    public static AuthResult Failure(string message, bool isLocked = false)
    {
        return new AuthResult(false, null, message, isLocked);
    }
}
