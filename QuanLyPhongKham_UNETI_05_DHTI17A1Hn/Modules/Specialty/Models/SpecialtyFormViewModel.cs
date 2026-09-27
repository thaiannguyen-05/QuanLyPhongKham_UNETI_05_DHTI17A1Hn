using System.ComponentModel.DataAnnotations;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Specialty.Models;

public sealed class SpecialtyFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Tên chuyên khoa là bắt buộc.")]
    [StringLength(100, ErrorMessage = "Tên chuyên khoa không được vượt quá 100 ký tự.")]
    [Display(Name = "Tên chuyên khoa")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự.")]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
    [Display(Name = "Trạng thái")]
    public SpecialtyStatus? Status { get; set; } = SpecialtyStatus.Active;
}
