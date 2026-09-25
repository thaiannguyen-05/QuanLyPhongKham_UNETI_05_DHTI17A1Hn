using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Accounts;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Services.Account.Dto;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Account.Mapping;

public static class AccountMapping
{
    public static CreateAccountDto ToCreateDto(AccountFormViewModel model)
    {
        return new CreateAccountDto
        {
            Username = model.Username,
            Password = model.Password ?? string.Empty,
            FullName = model.FullName,
            Email = model.Email,
            Role = model.Role,
            Status = model.Status
        };
    }

    public static UpdateAccountDto ToUpdateDto(AccountFormViewModel model)
    {
        return new UpdateAccountDto
        {
            Username = model.Username,
            Password = model.Password,
            FullName = model.FullName,
            Email = model.Email,
            Role = model.Role,
            Status = model.Status
        };
    }

    public static AccountListViewModel ToListViewModel(AccountListDto dto)
    {
        return new AccountListViewModel
        {
            Id = dto.Id,
            Username = dto.Username,
            FullName = dto.FullName,
            Email = dto.Email,
            Role = dto.Role,
            Status = dto.Status
        };
    }

    public static AccountFormViewModel ToFormViewModel(AccountDetailDto dto)
    {
        return new AccountFormViewModel
        {
            Id = dto.Id,
            Username = dto.Username,
            FullName = dto.FullName,
            Email = dto.Email,
            Role = dto.Role,
            Status = dto.Status
        };
    }
}
