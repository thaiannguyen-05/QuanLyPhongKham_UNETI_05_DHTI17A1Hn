using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Patient.Models;

public sealed class PatientFormViewModel : IValidatableObject
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Họ tên bệnh nhân bắt buộc nhập.")]
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
    [Display(Name = "Họ tên")]
    public string FullName { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Ngày sinh")]
    public DateTime? DateOfBirth { get; set; }

    [Display(Name = "Giới tính")]
    public string? Gender { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự.")]
    [Display(Name = "Số điện thoại")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự.")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự.")]
    [Display(Name = "Địa chỉ")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
    [Display(Name = "Trạng thái")]
    public PatientStatus Status { get; set; } = PatientStatus.Active;

    [Display(Name = "Tài khoản liên kết")]
    public int? AccountId { get; set; }

    [Display(Name = "Tạo tài khoản đăng nhập kèm")]
    public bool CreateAccount { get; set; }

    [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự.")]
    [Display(Name = "Tên đăng nhập mới")]
    public string? NewUsername { get; set; }

    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 100 ký tự.")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu mới")]
    public string? NewPassword { get; set; }

    public IEnumerable<SelectListItem> AvailableAccounts { get; set; } = Enumerable.Empty<SelectListItem>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(FullName))
        {
            yield return new ValidationResult("Họ tên bệnh nhân không được để trống hoặc chỉ chứa khoảng trắng.", new[] { nameof(FullName) });
        }

        if (DateOfBirth.HasValue && DateOfBirth.Value.Date > DateTime.Today)
        {
            yield return new ValidationResult("Ngày sinh không được lớn hơn ngày hiện tại.", new[] { nameof(DateOfBirth) });
        }

        if (!string.IsNullOrWhiteSpace(Phone))
        {
            var phone = Phone.Trim();
            if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^(0[3|5|7|8|9])[0-9]{8}$"))
            {
                yield return new ValidationResult("Số điện thoại không hợp lệ (gồm 10 số, bắt đầu bằng 03, 05, 07, 08, 09).", new[] { nameof(Phone) });
            }
        }

        if (AccountId.HasValue && CreateAccount)
        {
            yield return new ValidationResult("Chỉ chọn một trong hai: liên kết tài khoản có sẵn hoặc tạo tài khoản mới.", new[] { nameof(AccountId) });
        }

        if (CreateAccount)
        {
            if (string.IsNullOrWhiteSpace(NewUsername))
            {
                yield return new ValidationResult("Tên đăng nhập bắt buộc nhập khi tạo tài khoản.", new[] { nameof(NewUsername) });
            }

            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                yield return new ValidationResult("Mật khẩu bắt buộc nhập khi tạo tài khoản.", new[] { nameof(NewPassword) });
            }
        }
    }
}
