using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Patient.Models;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Patient.Services.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Patient.Models.Mapping;

public static class PatientMapping
{
    public static PatientCreateDto ToCreateDto(this PatientFormViewModel model)
    {
        return new PatientCreateDto
        {
            FullName = model.FullName.Trim(),
            DateOfBirth = model.DateOfBirth,
            Gender = model.Gender,
            Phone = model.Phone?.Trim(),
            Email = model.Email?.Trim(),
            Address = model.Address?.Trim(),
            Status = model.Status,
            AccountId = model.CreateAccount ? null : model.AccountId,
            CreateAccount = model.CreateAccount,
            NewUsername = model.NewUsername?.Trim(),
            NewPassword = model.NewPassword
        };
    }

    public static PatientSaveDto ToSaveDto(this PatientFormViewModel model)
    {
        return new PatientSaveDto
        {
            FullName = model.FullName.Trim(),
            DateOfBirth = model.DateOfBirth,
            Gender = model.Gender,
            Phone = model.Phone?.Trim(),
            Email = model.Email?.Trim(),
            Address = model.Address?.Trim(),
            Status = model.Status,
            AccountId = model.AccountId
        };
    }

    public static PatientFormViewModel ToFormViewModel(this PatientDto dto)
    {
        return new PatientFormViewModel
        {
            Id = dto.Id,
            FullName = dto.FullName,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address,
            Status = dto.Status,
            AccountId = dto.AccountId
        };
    }

    public static PatientDetailViewModel ToDetailViewModel(this PatientDto dto)
    {
        return new PatientDetailViewModel
        {
            Patient = dto
        };
    }

    public static PatientDeleteViewModel ToDeleteViewModel(this PatientDto dto)
    {
        return new PatientDeleteViewModel
        {
            Patient = dto
        };
    }

    public static PatientOwnViewModel ToOwnViewModel(this PatientDto dto)
    {
        return new PatientOwnViewModel
        {
            FullName = dto.FullName,
            Username = dto.Username,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address
        };
    }

    public static PatientSaveDto ToSaveDto(this PatientDto current, PatientOwnViewModel own)
    {
        ArgumentNullException.ThrowIfNull(current);
        ArgumentNullException.ThrowIfNull(own);

        return new PatientSaveDto
        {
            FullName = current.FullName,
            DateOfBirth = own.DateOfBirth,
            Gender = own.Gender,
            Phone = own.Phone?.Trim(),
            Email = own.Email?.Trim(),
            Address = own.Address?.Trim(),
            Status = current.Status,
            AccountId = current.AccountId
        };
    }
}
