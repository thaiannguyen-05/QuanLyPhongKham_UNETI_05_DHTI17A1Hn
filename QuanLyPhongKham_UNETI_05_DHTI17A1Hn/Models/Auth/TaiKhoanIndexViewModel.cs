namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Auth;

public sealed class TaiKhoanIndexViewModel
{
    public IReadOnlyList<TaiKhoanListViewModel> Accounts { get; set; } = Array.Empty<TaiKhoanListViewModel>();
}
