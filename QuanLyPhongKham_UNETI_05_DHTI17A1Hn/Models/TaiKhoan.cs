// Shared foundation - ca nhom. SV nhan Module dien Ho ten/MSSV khi sua file nay.
// Noi dung: Entity TaiKhoan theo muc 5.1 de tai 04.
using System.ComponentModel.DataAnnotations;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models;

public class TaiKhoan
{
    [Key]
    [Display(Name = "Mã tài khoản")]
    public int MaTaiKhoan { get; set; }

    [Required(ErrorMessage = "Tên đăng nhập bắt buộc nhập.")]
    [StringLength(50)]
    [Display(Name = "Tên đăng nhập")]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu bắt buộc nhập.")]
    [StringLength(100)]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string MatKhau { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ tên bắt buộc nhập.")]
    [StringLength(100)]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [StringLength(100)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Vai trò phải được xác định.")]
    [Display(Name = "Vai trò")]
    public VaiTro VaiTro { get; set; } = VaiTro.BenhNhan;

    [Display(Name = "Trạng thái")]
    public TrangThaiTaiKhoan TrangThai { get; set; } = TrangThaiTaiKhoan.HoatDong;

    public BenhNhan? BenhNhan { get; set; }
}
