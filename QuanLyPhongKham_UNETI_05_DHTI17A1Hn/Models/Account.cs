using System.ComponentModel.DataAnnotations;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models;

public class Account
{
    [Key]
    [Display(Name = "Mã tài khoản")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên đăng nhập bắt buộc nhập.")]
    [StringLength(50)]
    [Display(Name = "Tên đăng nhập")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu bắt buộc nhập.")]
    [StringLength(100)]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string PasswordHash { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ tên bắt buộc nhập.")]
    [StringLength(100)]
    [Display(Name = "Họ tên")]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [StringLength(100)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Vai trò phải được xác định.")]
    [Display(Name = "Vai trò")]
    public Role Role { get; set; } = Role.Patient;

    [Display(Name = "Trạng thái")]
    public AccountStatus Status { get; set; } = AccountStatus.Active;

    public Patient? Patient { get; set; }
}
