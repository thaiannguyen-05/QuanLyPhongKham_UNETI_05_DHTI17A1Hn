using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Auth;

public sealed class TaiKhoanListViewModel
{
    public int MaTaiKhoan { get; set; }

    public string TenDangNhap { get; set; } = string.Empty;

    public string HoTen { get; set; } = string.Empty;

    public string? Email { get; set; }

    public VaiTro VaiTro { get; set; }

    public TrangThaiTaiKhoan TrangThai { get; set; }
}
