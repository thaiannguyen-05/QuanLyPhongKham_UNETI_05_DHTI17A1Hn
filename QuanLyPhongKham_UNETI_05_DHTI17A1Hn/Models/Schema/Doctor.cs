using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Schema;

public class Doctor
{
    [Key]
    [Display(Name = "Mã bác sĩ")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Họ tên bắt buộc nhập.")]
    [StringLength(100)]
    [Display(Name = "Họ tên")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Chuyên khoa")]
    public int SpecialtyId { get; set; }

    [ForeignKey(nameof(SpecialtyId))]
    public Specialty? Specialty { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày sinh")]
    public DateTime? DateOfBirth { get; set; }

    [Display(Name = "Giới tính")]
    public string? Gender { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [StringLength(15)]
    [Display(Name = "Số điện thoại")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(100)]
    [Display(Name = "Trình độ")]
    public string? Qualification { get; set; }

    [Range(0, 60, ErrorMessage = "Số năm kinh nghiệm phải >= 0.")]
    [Display(Name = "Số năm kinh nghiệm")]
    public int YearsOfExperience { get; set; } = 0;

    [Range(1, double.MaxValue, ErrorMessage = "Phí khám phải > 0.")]
    [DataType(DataType.Currency)]
    [Display(Name = "Phí khám")]
    public decimal ConsultationFee { get; set; }

    [Display(Name = "Trạng thái")]
    public DoctorStatus Status { get; set; } = DoctorStatus.Working;

    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
