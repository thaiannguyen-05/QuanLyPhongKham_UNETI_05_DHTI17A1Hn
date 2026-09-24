// Shared foundation - ca nhom. SV nhan Module dien Ho ten/MSSV khi sua file nay.
// Noi dung: Entity BacSi theo muc 6.1 de tai 04.
using System.ComponentModel.DataAnnotations;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models;

public class BacSi
{
    [Key]
    [Display(Name = "Mã bác sĩ")]
    public int MaBacSi { get; set; }

    [Required(ErrorMessage = "Họ tên bắt buộc nhập.")]
    [StringLength(100)]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Chuyên khoa")]
    public int MaChuyenKhoa { get; set; }

    [ForeignKey(nameof(MaChuyenKhoa))]
    public ChuyenKhoa? ChuyenKhoa { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày sinh")]
    public DateTime? NgaySinh { get; set; }

    [Display(Name = "Giới tính")]
    public string? GioiTinh { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [StringLength(15)]
    [Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }

    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(100)]
    [Display(Name = "Trình độ")]
    public string? TrinhDo { get; set; }

    [Range(0, 60, ErrorMessage = "Số năm kinh nghiệm phải >= 0.")]
    [Display(Name = "Số năm kinh nghiệm")]
    public int SoNamKinhNghiem { get; set; } = 0;

    [Range(1, double.MaxValue, ErrorMessage = "Phí khám phải > 0.")]
    [DataType(DataType.Currency)]
    [Display(Name = "Phí khám")]
    public decimal PhiKham { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiBacSi TrangThai { get; set; } = TrangThaiBacSi.DangLamViec;

    public ICollection<LichKham> LichKhams { get; set; } = new List<LichKham>();
}
