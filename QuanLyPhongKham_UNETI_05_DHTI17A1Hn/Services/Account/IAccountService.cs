using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Accounts;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Services.Account;

public interface IAccountService
{
    Task<IReadOnlyList<AccountListViewModel>> GetListAsync(CancellationToken cancellationToken = default);

    Task<AccountFormViewModel?> GetForEditAsync(int id, CancellationToken cancellationToken = default);

    Task<AccountSaveResult> CreateAsync(
        AccountFormViewModel model,
        CancellationToken cancellationToken = default);

    Task<AccountSaveResult> UpdateAsync(
        int id,
        AccountFormViewModel model,
        CancellationToken cancellationToken = default);

    Task<bool> SetLockAsync(
        int id,
        bool isLocked,
        CancellationToken cancellationToken = default);
}

public sealed record AccountSaveResult(
    bool Succeeded,
    string? ErrorField = null,
    string? ErrorMessage = null)
{
    public static AccountSaveResult Success()
    {
        return new AccountSaveResult(true);
    }

    public static AccountSaveResult Failure(string field, string message)
    {
        return new AccountSaveResult(false, field, message);
    }
}
