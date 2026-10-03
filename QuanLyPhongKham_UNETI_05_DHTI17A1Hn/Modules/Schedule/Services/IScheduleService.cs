using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Schedule.Services.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Schedule.Services;

public interface IScheduleService
{
    Task<(IReadOnlyList<ScheduleDto> Items, int TotalCount)> GetPagedAsync(
        ScheduleFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ScheduleDto> Items, int TotalCount)> GetMyAsync(
        int doctorId,
        ScheduleFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ScheduleDto> Items, int TotalCount)> GetAvailableAsync(
        ScheduleFilterDto filter,
        CancellationToken cancellationToken = default);

    Task<ScheduleDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task ProposeAsync(int doctorId, ScheduleSaveDto dto, CancellationToken cancellationToken = default);
    Task UpdatePendingAsync(int doctorId, int id, ScheduleSaveDto dto, CancellationToken cancellationToken = default);
    Task DeletePendingAsync(int doctorId, int id, CancellationToken cancellationToken = default);

    Task ApproveAsync(int id, CancellationToken cancellationToken = default);
    Task RejectAsync(int id, CancellationToken cancellationToken = default);
    Task CloseAsync(int id, CancellationToken cancellationToken = default);
    Task ReopenAsync(int id, CancellationToken cancellationToken = default);

    Task<IEnumerable<SelectListItem>> GetDoctorDropdownAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SelectListItem>> GetSpecialtyDropdownAsync(CancellationToken cancellationToken = default);
}
