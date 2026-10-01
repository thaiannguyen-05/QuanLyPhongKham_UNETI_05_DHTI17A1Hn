namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Patient.Services.Dto;

public sealed class PatientCreateDto : PatientSaveDto
{
    public bool CreateAccount { get; set; }
    public string? NewUsername { get; set; }
    public string? NewPassword { get; set; }
}
