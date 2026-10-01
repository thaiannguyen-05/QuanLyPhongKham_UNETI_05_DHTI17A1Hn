using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Doctor.Services.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Doctor.Services;

public interface IDoctorService
{
    Task<(IReadOnlyList<DoctorDto> Items, int TotalCount)> GetPagedAsync(
        DoctorFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<DoctorDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task CreateAsync(
        DoctorSaveDto dto,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        int id,
        DoctorSaveDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<SelectListItem>> GetSpecialtyDropdownAsync(
        CancellationToken cancellationToken = default);
}
