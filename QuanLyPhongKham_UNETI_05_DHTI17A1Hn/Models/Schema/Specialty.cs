using System.ComponentModel.DataAnnotations;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Schema;

public class Specialty
{
    [Key]
    [Display(Name = "Mã chuyên khoa")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên chuyên khoa bắt buộc nhập.")]
    [StringLength(100)]
    [Display(Name = "Tên chuyên khoa")]
    public string Name { get; set; } = string.Empty;

    [DataType(DataType.MultilineText)]
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [Display(Name = "Trạng thái")]
    public SpecialtyStatus Status { get; set; } = SpecialtyStatus.Active;

    public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
