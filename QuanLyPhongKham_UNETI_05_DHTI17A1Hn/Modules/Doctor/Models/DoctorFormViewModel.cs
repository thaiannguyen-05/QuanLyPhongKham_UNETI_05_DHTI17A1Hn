using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Doctor.Models;

public sealed class DoctorFormViewModel : IValidatableObject
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Họ và tên bác sĩ là bắt buộc.")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Họ và tên bác sĩ không được để trống hoặc chỉ chứa khoảng trắng.")]
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn chuyên khoa.")]
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn một chuyên khoa hợp lệ.")]
    [Display(Name = "Chuyên khoa")]
    public int SpecialtyId { get; set; }

    [Required(ErrorMessage = "Ngày sinh là bắt buộc.")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày sinh")]
    public DateTime? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn giới tính.")]
    [Display(Name = "Giới tính")]
    public string? Gender { get; set; }

    [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
    [RegularExpression(@"^(0[3|5|7|8|9])[0-9]{8}$", ErrorMessage = "Số điện thoại không hợp lệ (Gồm 10 số, bắt đầu bằng 03, 05, 07, 08, 09).")]
    [Display(Name = "Số điện thoại")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Email là bắt buộc.")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Email bắt buộc phải có đuôi @gmail.com (Ví dụ: bacsi@gmail.com).")]
    [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự.")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Trình độ chuyên môn là bắt buộc.")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Trình độ chuyên môn không được để trống hoặc chỉ chứa khoảng trắng.")]
    [StringLength(100, ErrorMessage = "Trình độ tối đa 100 ký tự.")]
    [Display(Name = "Trình độ chuyên môn")]
    public string? Qualification { get; set; }

    [Required(ErrorMessage = "Số năm kinh nghiệm là bắt buộc.")]
    [Range(0, 60, ErrorMessage = "Số năm kinh nghiệm phải lớn hơn hoặc bằng 0.")]
    [Display(Name = "Số năm kinh nghiệm")]
    public int YearsOfExperience { get; set; } = 0;

    [Required(ErrorMessage = "Phí khám là bắt buộc.")]
    [Range(1, 100000000, ErrorMessage = "Phí khám phải lớn hơn 0 VNĐ.")]
    [Display(Name = "Phí khám (VNĐ)")]
    public decimal ConsultationFee { get; set; } = 100000;

    [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
    [Display(Name = "Trạng thái")]
    public DoctorStatus Status { get; set; } = DoctorStatus.Working;

    public IEnumerable<SelectListItem> SpecialtyList { get; set; } = Enumerable.Empty<SelectListItem>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(FullName))
        {
            yield return new ValidationResult("Họ và tên bác sĩ không được để trống hoặc chỉ chứa khoảng trắng.", new[] { nameof(FullName) });
        }

        if (string.IsNullOrWhiteSpace(Qualification))
        {
            yield return new ValidationResult("Trình độ chuyên môn không được để trống hoặc chỉ chứa khoảng trắng.", new[] { nameof(Qualification) });
        }

        if (!string.IsNullOrWhiteSpace(Email) && !Email.Trim().EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
        {
            yield return new ValidationResult("Email bắt buộc phải có đuôi @gmail.com (Ví dụ: bacsi@gmail.com).", new[] { nameof(Email) });
        }

        if (DateOfBirth.HasValue)
        {
            var today = DateTime.Today;
            var maxValidBirthDate = today.AddYears(-18);

            if (DateOfBirth.Value > today)
            {
                yield return new ValidationResult("Ngày sinh không thể lớn hơn ngày hiện tại.", new[] { nameof(DateOfBirth) });
            }
            else if (DateOfBirth.Value > maxValidBirthDate || DateOfBirth.Value.Year < 1920)
            {
                yield return new ValidationResult("Bác sĩ phải đủ từ 18 tuổi trở lên (sinh từ năm 2008 trở về trước).", new[] { nameof(DateOfBirth) });
            }
        }
    }
}
