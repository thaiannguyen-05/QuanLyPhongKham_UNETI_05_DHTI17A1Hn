// Shared foundation - ca nhom.
// Enums cho VaiTro + TrangThai (thay string de type-safe, hien thi tieng Viet qua Display).
using System.ComponentModel.DataAnnotations;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;

public enum VaiTro
{
    [Display(Name = "Quản trị viên")]
    Admin = 0,
    [Display(Name = "Bệnh nhân")]
    BenhNhan = 1
}

public enum TrangThaiTaiKhoan
{
    [Display(Name = "Hoạt động")]
    HoatDong = 0,
    [Display(Name = "Khóa")]
    Khoa = 1
}

public enum TrangThaiChuyenKhoa
{
    [Display(Name = "Hoạt động")]
    HoatDong = 0,
    [Display(Name = "Tạm ngừng")]
    TamNgung = 1
}

public enum TrangThaiBacSi
{
    [Display(Name = "Đang làm việc")]
    DangLamViec = 0,
    [Display(Name = "Tạm nghỉ")]
    TamNghi = 1,
    [Display(Name = "Ngừng làm việc")]
    NgungLamViec = 2
}

public enum TrangThaiBenhNhan
{
    [Display(Name = "Hoạt động")]
    HoatDong = 0,
    [Display(Name = "Tạm ngừng")]
    TamNgung = 1
}

public enum TrangThaiLichKham
{
    [Display(Name = "Còn nhận đăng ký")]
    ConNhanDangKy = 0,
    [Display(Name = "Đã đủ")]
    DaDu = 1,
    [Display(Name = "Tạm ngừng")]
    TamNgung = 2
}

public enum TrangThaiPhieu
{
    [Display(Name = "Chờ xác nhận")]
    ChoXacNhan = 0,
    [Display(Name = "Đã xác nhận")]
    DaXacNhan = 1,
    [Display(Name = "Đã khám")]
    DaKham = 2,
    [Display(Name = "Đã hủy")]
    DaHuy = 3
}
