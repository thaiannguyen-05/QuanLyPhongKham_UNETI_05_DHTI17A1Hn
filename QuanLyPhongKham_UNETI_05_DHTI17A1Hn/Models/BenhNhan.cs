// Shared foundation - ca nhom. SV nhan Module dien Ho ten/MSSV khi sua file nay.
// Noi dung: Entity BenhNhan theo muc 7.1 de tai 04.
using System.ComponentModel.DataAnnotations;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models;

public class BenhNhan
{
    [Key]
    [Display(Name = "Mã bệnh nhân")]
    public int MaBenhNhan { get; set; }

    [Display(Name = "Mã tài khoản")]
    public int? MaTaiKhoan { get; set; }

    [ForeignKey(nameof(MaTaiKhoan))]
    public TaiKhoan? TaiKhoan { get; set; }

    [Required(ErrorMessage = "Họ tên bắt buộc nhập.")]
    [StringLength(100)]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;

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

    [StringLength(200)]
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngày đăng ký")]
    public DateTime NgayDangKy { get; set; } = DateTime.Today;

    [Display(Name = "Trạng thái")]
    public TrangThaiBenhNhan TrangThai { get; set; } = TrangThaiBenhNhan.HoatDong;

    public ICollection<PhieuDangKyKham> PhieuDangKyKhams { get; set; } = new List<PhieuDangKyKham>();
}
