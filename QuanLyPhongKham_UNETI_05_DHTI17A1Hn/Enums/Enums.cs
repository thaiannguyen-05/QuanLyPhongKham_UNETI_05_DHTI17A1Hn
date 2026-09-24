using System.ComponentModel.DataAnnotations;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

public enum Role
{
    [Display(Name = "Quản trị viên")]
    Admin = 0,
    [Display(Name = "Bệnh nhân")]
    Patient = 1
}

public enum AccountStatus
{
    [Display(Name = "Hoạt động")]
    Active = 0,
    [Display(Name = "Khóa")]
    Locked = 1
}

public enum SpecialtyStatus
{
    [Display(Name = "Hoạt động")]
    Active = 0,
    [Display(Name = "Tạm ngừng")]
    Inactive = 1
}

public enum DoctorStatus
{
    [Display(Name = "Đang làm việc")]
    Working = 0,
    [Display(Name = "Tạm nghỉ")]
    OnLeave = 1,
    [Display(Name = "Ngừng làm việc")]
    Inactive = 2
}

public enum PatientStatus
{
    [Display(Name = "Hoạt động")]
    Active = 0,
    [Display(Name = "Tạm ngừng")]
    Inactive = 1
}

public enum ScheduleStatus
{
    [Display(Name = "Còn nhận đăng ký")]
    Open = 0,
    [Display(Name = "Đã đủ")]
    Full = 1,
    [Display(Name = "Tạm ngừng")]
    Closed = 2
}

public enum BookingStatus
{
    [Display(Name = "Chờ xác nhận")]
    Pending = 0,
    [Display(Name = "Đã xác nhận")]
    Confirmed = 1,
    [Display(Name = "Đã khám")]
    Completed = 2,
    [Display(Name = "Đã hủy")]
    Cancelled = 3
}
