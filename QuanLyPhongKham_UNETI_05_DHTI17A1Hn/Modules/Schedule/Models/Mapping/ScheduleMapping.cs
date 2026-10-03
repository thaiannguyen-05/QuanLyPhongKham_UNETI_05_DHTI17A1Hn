using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Schedule.Models;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Schedule.Services.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Schedule.Models.Mapping;

public static class ScheduleMapping
{
    public static ScheduleSaveDto ToSaveDto(this ScheduleFormViewModel model)
    {
        return new ScheduleSaveDto
        {
            Date = model.Date.Date,
            StartTime = model.StartTime,
            EndTime = model.EndTime,
            Capacity = model.Capacity
        };
    }

    public static ScheduleFormViewModel ToFormViewModel(this ScheduleDto dto)
    {
        return new ScheduleFormViewModel
        {
            Id = dto.Id,
            Date = dto.Date.Date,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Capacity = dto.Capacity
        };
    }
}
