using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Doctor.Models;

public sealed class DoctorFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Họ tên bác sĩ là bắt buộc.")]
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn chuyên khoa.")]
    [Display(Name = "Chuyên khoa")]
    public int SpecialtyId { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày sinh")]
    public DateTime? DateOfBirth { get; set; }

    [Display(Name = "Giới tính")]
    public string? Gender { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [StringLength(15, ErrorMessage = "Số điện thoại tối đa 15 số.")]
    [Display(Name = "Số điện thoại")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự.")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [StringLength(100, ErrorMessage = "Trình độ tối đa 100 ký tự.")]
    [Display(Name = "Trình độ chuyên môn")]
    public string? Qualification { get; set; }

    [Range(0, 60, ErrorMessage = "Số năm kinh nghiệm phải từ 0 đến 60 năm.")]
    [Display(Name = "Số năm kinh nghiệm")]
    public int YearsOfExperience { get; set; } = 0;

    [Required(ErrorMessage = "Phí khám là bắt buộc.")]
    [Range(1000, 100000000, ErrorMessage = "Phí khám phải từ 1.000 VNĐ đến 100.000.000 VNĐ.")]
    [Display(Name = "Phí khám (VNĐ)")]
    public decimal ConsultationFee { get; set; } = 100000;

    [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
    [Display(Name = "Trạng thái")]
    public DoctorStatus Status { get; set; } = DoctorStatus.Working;

    public IEnumerable<SelectListItem> SpecialtyList { get; set; } = Enumerable.Empty<SelectListItem>();
}
