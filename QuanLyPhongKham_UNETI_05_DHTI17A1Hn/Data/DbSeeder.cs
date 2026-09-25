using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Schema;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Services.Auth;

namespace QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Data;

public static class DbSeeder
{
    private const string AdminUsername = "admin";

    public static async Task SeedAdminAsync(AppDbContext context)
    {
        var adminExists = await context.Accounts
            .AnyAsync(account => account.Username == AdminUsername);

        if (adminExists)
        {
            return;
        }

        var admin = new Account
        {
            Username = AdminUsername,
            PasswordHash = PasswordHasher.Hash("Admin@123"),
            FullName = "Quản trị viên",
            Role = Role.Admin,
            Status = AccountStatus.Active
        };

        context.Accounts.Add(admin);
        await context.SaveChangesAsync();
    }
}
