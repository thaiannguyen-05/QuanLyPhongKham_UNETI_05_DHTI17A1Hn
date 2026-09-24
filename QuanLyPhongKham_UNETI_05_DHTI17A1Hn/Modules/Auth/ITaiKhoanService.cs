using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Auth;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Auth;

public interface ITaiKhoanService
{
    Task<IReadOnlyList<TaiKhoanListViewModel>> GetListAsync(CancellationToken cancellationToken = default);

    Task<TaiKhoanFormViewModel?> GetForEditAsync(int id, CancellationToken cancellationToken = default);

    Task<TaiKhoanSaveResult> CreateAsync(
        TaiKhoanFormViewModel model,
        CancellationToken cancellationToken = default);

    Task<TaiKhoanSaveResult> UpdateAsync(
        int id,
        TaiKhoanFormViewModel model,
        CancellationToken cancellationToken = default);

    Task<bool> SetLockAsync(
        int id,
        bool isLocked,
        CancellationToken cancellationToken = default);
}

public sealed record TaiKhoanSaveResult(
    bool Succeeded,
    string? ErrorField = null,
    string? ErrorMessage = null)
{
    public static TaiKhoanSaveResult Success()
    {
        return new TaiKhoanSaveResult(true);
    }

    public static TaiKhoanSaveResult Failure(string field, string message)
    {
        return new TaiKhoanSaveResult(false, field, message);
    }
}
