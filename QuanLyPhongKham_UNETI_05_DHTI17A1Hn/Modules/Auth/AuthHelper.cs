using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Auth;

public static class AuthHelper
{
    public static bool IsLocked(TaiKhoan account)
    {
        return account.TrangThai == TrangThaiTaiKhoan.Khoa;
    }
}
