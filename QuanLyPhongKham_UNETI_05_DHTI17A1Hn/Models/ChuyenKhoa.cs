// Shared foundation - ca nhom. SV nhan Module dien Ho ten/MSSV khi sua file nay.
// Noi dung: Entity ChuyenKhoa theo muc 5.5 de tai 04.
using System.ComponentModel.DataAnnotations;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models;

public class ChuyenKhoa
{
    [Key]
    [Display(Name = "Mã chuyên khoa")]
    public int MaChuyenKhoa { get; set; }

    [Required(ErrorMessage = "Tên chuyên khoa bắt buộc nhập.")]
    [StringLength(100)]
    [Display(Name = "Tên chuyên khoa")]
    public string TenChuyenKhoa { get; set; } = string.Empty;

    [DataType(DataType.MultilineText)]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiChuyenKhoa TrangThai { get; set; } = TrangThaiChuyenKhoa.HoatDong;

    public ICollection<BacSi> BacSis { get; set; } = new List<BacSi>();
}
