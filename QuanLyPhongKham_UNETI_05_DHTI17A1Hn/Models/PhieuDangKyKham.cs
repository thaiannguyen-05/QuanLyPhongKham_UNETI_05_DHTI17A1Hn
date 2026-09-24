// Shared foundation - ca nhom. SV nhan Module dien Ho ten/MSSV khi sua file nay.
// Noi dung: Entity PhieuDangKyKham theo muc 7.3 de tai 04.
using System.ComponentModel.DataAnnotations;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models;

public class PhieuDangKyKham
{
    [Key]
    [Display(Name = "Mã phiếu")]
    public int MaPhieu { get; set; }

    [Required]
    [Display(Name = "Bệnh nhân")]
    public int MaBenhNhan { get; set; }

    [ForeignKey(nameof(MaBenhNhan))]
    public BenhNhan? BenhNhan { get; set; }

    [Required]
    [Display(Name = "Lịch khám")]
    public int MaLichKham { get; set; }

    [ForeignKey(nameof(MaLichKham))]
    public LichKham? LichKham { get; set; }

    [Display(Name = "Ngày đăng ký")]
    public DateTime NgayDangKy { get; set; } = DateTime.Now;

    [DataType(DataType.MultilineText)]
    [Display(Name = "Lý do khám")]
    public string? LyDoKham { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiPhieu TrangThai { get; set; } = TrangThaiPhieu.ChoXacNhan;
}
