using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Specialty.Services.Dto;

public sealed class SpecialtySaveDto
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public SpecialtyStatus Status { get; set; } = SpecialtyStatus.Active;
}
