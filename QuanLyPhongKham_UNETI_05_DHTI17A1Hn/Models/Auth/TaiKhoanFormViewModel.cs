using System.ComponentModel.DataAnnotations;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Auth;

public sealed class TaiKhoanFormViewModel
{
    public int? MaTaiKhoan { get; set; }

    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
    [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự.")]
    [Display(Name = "Tên đăng nhập")]
    public string TenDangNhap { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Mật khẩu không được vượt quá 100 ký tự.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string? MatKhau { get; set; }

    [Required(ErrorMessage = "Họ tên là bắt buộc.")]
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự.")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Vai trò là bắt buộc.")]
    [Display(Name = "Vai trò")]
    public VaiTro? VaiTro { get; set; }

    [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
    [Display(Name = "Trạng thái")]
    public TrangThaiTaiKhoan? TrangThai { get; set; } = TrangThaiTaiKhoan.HoatDong;
}
