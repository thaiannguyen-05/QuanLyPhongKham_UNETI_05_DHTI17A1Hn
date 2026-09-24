using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Data;

public static class DbSeeder
{
    private const string AdminUsername = "admin";

    // Hash của mật khẩu ban đầu do nhóm thống nhất; không lưu mật khẩu thường.
    private const string AdminPasswordHash = "AQAAAAIAAYagAAAAENXDDa1DA716k6kh9dSK/72ksrOxBvDVRPFM+a/aZj6iwWPUrhzPDRpSVCAR7/LeGQ==";

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
            MatKhau = AdminPasswordHash,
            HoTen = "Quản trị viên",
            VaiTro = VaiTro.Admin,
            TrangThai = TrangThaiTaiKhoan.HoatDong
        };

        context.TaiKhoans.Add(admin);
        await context.SaveChangesAsync();
    }
}
