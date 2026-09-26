using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Specialty.Services.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Specialty.Services;

public interface ISpecialtyService
{
    Task<IReadOnlyList<SpecialtyDto>> ListAsync(CancellationToken cancellationToken = default);

    Task<SpecialtyDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task CreateAsync(SpecialtySaveDto dto, CancellationToken cancellationToken = default);

    Task UpdateAsync(int id, SpecialtySaveDto dto, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task SetStatusAsync(
        int id,
        Enums.SpecialtyStatus status,
        CancellationToken cancellationToken = default);
}
