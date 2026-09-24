using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Auth;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Data;

public static class DbSeeder
{
    private const string AdminUsername = "admin";
    private const string AdminInitialPassword = "Admin@123";

    public static async Task SeedAdminAsync(AppDbContext context)
    {
        var adminExists = await context.TaiKhoans
            .AnyAsync(account => account.TenDangNhap == AdminUsername);

        if (adminExists)
        {
            return;
        }

        var admin = new TaiKhoan
        {
            TenDangNhap = AdminUsername,
            MatKhau = PasswordHasher.Hash(AdminInitialPassword),
            HoTen = "Quản trị viên",
            VaiTro = VaiTro.Admin,
            TrangThai = TrangThaiTaiKhoan.HoatDong
        };

        context.TaiKhoans.Add(admin);
        await context.SaveChangesAsync();
    }
}
