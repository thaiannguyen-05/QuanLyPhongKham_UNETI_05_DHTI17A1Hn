// Shared foundation - ca nhom. SV nhan Module dien Ho ten/MSSV khi sua file nay.
// Noi dung: Entity LichKham theo muc 7.2 de tai 04.
using System.ComponentModel.DataAnnotations;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models;

public class LichKham
{
    [Key]
    [Display(Name = "Mã lịch khám")]
    public int MaLichKham { get; set; }

    [Required]
    [Display(Name = "Bác sĩ")]
    public int MaBacSi { get; set; }

    [ForeignKey(nameof(MaBacSi))]
    public BacSi? BacSi { get; set; }

    [Required(ErrorMessage = "Ngày khám bắt buộc nhập.")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày khám")]
    public DateTime NgayKham { get; set; }

    [Required]
    [Display(Name = "Giờ bắt đầu")]
    public TimeSpan GioBatDau { get; set; }

    [Required]
    [Display(Name = "Giờ kết thúc")]
    public TimeSpan GioKetThuc { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Số lượng tối đa phải > 0.")]
    [Display(Name = "Số lượng tối đa")]
    public int SoLuongToiDa { get; set; }

    [Range(0, int.MaxValue)]
    [Display(Name = "Số lượng đã đăng ký")]
    public int SoLuongDaDangKy { get; set; } = 0;

    [Display(Name = "Trạng thái")]
    public TrangThaiLichKham TrangThai { get; set; } = TrangThaiLichKham.ConNhanDangKy;

    public ICollection<PhieuDangKyKham> PhieuDangKyKhams { get; set; } = new List<PhieuDangKyKham>();
}
