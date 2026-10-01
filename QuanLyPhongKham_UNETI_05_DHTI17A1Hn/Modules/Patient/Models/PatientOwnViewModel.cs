using System.ComponentModel.DataAnnotations;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Patient.Models;

public sealed class PatientOwnViewModel : IValidatableObject
{
    [Display(Name = "Họ tên")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Tên đăng nhập")]
    public string? Username { get; set; }

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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
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
    }
}
