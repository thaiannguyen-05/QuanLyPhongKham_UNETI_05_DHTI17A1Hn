namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Accounts;

public sealed class AccountIndexViewModel
{
    public IReadOnlyList<AccountListViewModel> Accounts { get; set; } = Array.Empty<AccountListViewModel>();
}
