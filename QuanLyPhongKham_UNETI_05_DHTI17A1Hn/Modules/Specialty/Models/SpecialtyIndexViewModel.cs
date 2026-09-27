namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Specialty.Models;

public sealed class SpecialtyIndexViewModel
{
    public IReadOnlyList<SpecialtyListViewModel> Specialties { get; set; } =
        Array.Empty<SpecialtyListViewModel>();
}
