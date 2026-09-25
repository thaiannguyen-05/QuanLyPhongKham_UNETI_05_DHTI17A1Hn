using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Schema;

public class Patient
{
    [Key]
    [Display(Name = "Mã bệnh nhân")]
    public int Id { get; set; }

    [Display(Name = "Mã tài khoản")]
    public int? AccountId { get; set; }

    [ForeignKey(nameof(AccountId))]
    public Account? Account { get; set; }

    [Required(ErrorMessage = "Họ tên bắt buộc nhập.")]
    [StringLength(100)]
    [Display(Name = "Họ tên")]
    public string FullName { get; set; } = string.Empty;

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

    [StringLength(200)]
    [Display(Name = "Địa chỉ")]
    public string? Address { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày đăng ký")]
    public DateTime RegisteredAt { get; set; } = DateTime.Today;

    [Display(Name = "Trạng thái")]
    public PatientStatus Status { get; set; } = PatientStatus.Active;

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
