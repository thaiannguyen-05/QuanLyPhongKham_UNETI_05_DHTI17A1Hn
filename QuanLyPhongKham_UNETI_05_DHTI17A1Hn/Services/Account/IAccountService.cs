using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Services.Account.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Services.Account;

public interface IAccountService
{
    Task<IReadOnlyList<AccountListDto>> GetListAsync(CancellationToken cancellationToken = default);

    Task<AccountDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task CreateAsync(CreateAccountDto dto, CancellationToken cancellationToken = default);

    Task UpdateAsync(int id, UpdateAccountDto dto, CancellationToken cancellationToken = default);

    Task SetLockAsync(int id, bool isLocked, CancellationToken cancellationToken = default);
}
