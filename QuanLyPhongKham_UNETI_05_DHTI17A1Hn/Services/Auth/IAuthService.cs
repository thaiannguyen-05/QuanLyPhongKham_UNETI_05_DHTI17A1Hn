using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Services.Auth;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default);
}

public sealed record AuthenticatedAccount(
    int Id,
    string Username,
    string FullName,
    Role Role);

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
