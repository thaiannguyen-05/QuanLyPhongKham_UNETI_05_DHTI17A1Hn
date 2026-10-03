using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Patient.Services.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Patient.Services;

public interface IPatientService
{
    Task<(IReadOnlyList<PatientDto> Items, int TotalCount)> GetPagedAsync(
        PatientFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<PatientDto> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task CreateAsync(
        PatientCreateDto dto,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        int id,
        PatientSaveDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<SelectListItem>> GetAvailableAccountsAsync(
        int? excludePatientId = null,
        CancellationToken cancellationToken = default);
}
