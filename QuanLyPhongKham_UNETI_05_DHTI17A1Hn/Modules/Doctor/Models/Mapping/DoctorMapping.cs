using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Doctor.Models;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Doctor.Services.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Doctor.Models.Mapping;

public static class DoctorMapping
{
    public static DoctorSaveDto ToSaveDto(this DoctorFormViewModel model)
    {
        return new DoctorSaveDto
        {
            FullName = model.FullName.Trim(),
            SpecialtyId = model.SpecialtyId,
            DateOfBirth = model.DateOfBirth,
            Gender = model.Gender,
            Phone = model.Phone?.Trim(),
            Email = model.Email?.Trim(),
            Qualification = model.Qualification?.Trim(),
            YearsOfExperience = model.YearsOfExperience,
            ConsultationFee = model.ConsultationFee,
            Status = model.Status,
            AccountId = model.AccountId
        };
    }

    public static DoctorFormViewModel ToFormViewModel(this DoctorDto dto)
    {
        return new DoctorFormViewModel
        {
            Id = dto.Id,
            FullName = dto.FullName,
            SpecialtyId = dto.SpecialtyId,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            Phone = dto.Phone,
            Email = dto.Email,
            Qualification = dto.Qualification,
            YearsOfExperience = dto.YearsOfExperience,
            ConsultationFee = dto.ConsultationFee,
            Status = dto.Status,
            AccountId = dto.AccountId
        };
    }

    public static DoctorDetailViewModel ToDetailViewModel(this DoctorDto dto)
    {
        return new DoctorDetailViewModel
        {
            Doctor = dto
        };
    }

    public static DoctorDeleteViewModel ToDeleteViewModel(this DoctorDto dto)
    {
        return new DoctorDeleteViewModel
        {
            Doctor = dto
        };
    }
}
