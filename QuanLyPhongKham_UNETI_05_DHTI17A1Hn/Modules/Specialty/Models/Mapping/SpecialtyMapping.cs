using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Specialty.Services.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Specialty.Models.Mapping;

public static class SpecialtyMapping
{
    public static SpecialtySaveDto ToSaveDto(SpecialtyFormViewModel model)
    {
        return new SpecialtySaveDto
        {
            Name = model.Name,
            Description = model.Description,
            Status = model.Status ?? SpecialtyStatus.Active
        };
    }

    public static SpecialtyListViewModel ToListViewModel(SpecialtyDto dto)
    {
        return new SpecialtyListViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Status = dto.Status,
            DoctorCount = dto.DoctorCount
        };
    }

    public static SpecialtyDetailViewModel ToDetailViewModel(SpecialtyDto dto)
    {
        return new SpecialtyDetailViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Status = dto.Status,
            DoctorCount = dto.DoctorCount
        };
    }

    public static SpecialtyFormViewModel ToFormViewModel(SpecialtyDto dto)
    {
        return new SpecialtyFormViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Status = dto.Status
        };
    }

    public static SpecialtyDeleteViewModel ToDeleteViewModel(SpecialtyDto dto)
    {
        return new SpecialtyDeleteViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Status = dto.Status,
            DoctorCount = dto.DoctorCount
        };
    }
}
