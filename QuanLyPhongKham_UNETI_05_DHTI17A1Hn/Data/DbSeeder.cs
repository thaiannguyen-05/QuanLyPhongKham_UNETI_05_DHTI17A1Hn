using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Enums;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Models.Schema;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Auth.Services;

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

    public static async Task SeedSampleDataAsync(AppDbContext context)
    {
        // 1. Seed Chuyên khoa mẫu nếu chưa có
        if (!await context.Specialties.AnyAsync())
        {
            var specialties = new List<Specialty>
            {
                new() { Name = "Khoa Tim Mạch", Description = "Khám và điều trị chuyên sâu các bệnh lý tim mạch, huyết áp.", Status = SpecialtyStatus.Active },
                new() { Name = "Khoa Nội Tổng Quát", Description = "Khám, chẩn đoán và điều trị các bệnh lý nội khoa người lớn.", Status = SpecialtyStatus.Active },
                new() { Name = "Khoa Ngoại & Chấn Thương", Description = "Phẫu thuật tổng quát, chỉnh hình và điều trị chấn thương xương khớp.", Status = SpecialtyStatus.Active },
                new() { Name = "Khoa Nhi", Description = "Chăm sóc sức khỏe toàn diện, khám và điều trị bệnh cho trẻ sơ sinh và trẻ nhỏ.", Status = SpecialtyStatus.Active },
                new() { Name = "Khoa Da Liễu", Description = "Khám chữa các bệnh lý ngoài da và thẩm mỹ da liễu.", Status = SpecialtyStatus.Active },
                new() { Name = "Khoa Tai Mũi Họng", Description = "Chẩn đoán và điều trị bệnh lý tai, mũi, xoang, họng và thanh quản.", Status = SpecialtyStatus.Active },
                new() { Name = "Khoa Mắt", Description = "Khám và điều trị các bệnh về mắt, đo thị lực và tật khúc xạ.", Status = SpecialtyStatus.Active }
            };

            context.Specialties.AddRange(specialties);
            await context.SaveChangesAsync();
        }

        // 2. Seed 13 Bác sĩ thực tế nếu bảng Doctors còn trống
        if (!await context.Doctors.AnyAsync())
        {
            var timMach = await context.Specialties.FirstOrDefaultAsync(s => s.Name == "Khoa Tim Mạch");
            var noiTongQuat = await context.Specialties.FirstOrDefaultAsync(s => s.Name == "Khoa Nội Tổng Quát");
            var ngoaiKhoa = await context.Specialties.FirstOrDefaultAsync(s => s.Name == "Khoa Ngoại & Chấn Thương");
            var nhiKhoa = await context.Specialties.FirstOrDefaultAsync(s => s.Name == "Khoa Nhi");
            var daLieu = await context.Specialties.FirstOrDefaultAsync(s => s.Name == "Khoa Da Liễu");
            var taiMuiHong = await context.Specialties.FirstOrDefaultAsync(s => s.Name == "Khoa Tai Mũi Họng");
            var matKhoa = await context.Specialties.FirstOrDefaultAsync(s => s.Name == "Khoa Mắt");

            var doctors = new List<Doctor>
            {
                new()
                {
                    FullName = "Nguyễn Văn Hùng",
                    SpecialtyId = timMach!.Id,
                    DateOfBirth = new DateTime(1975, 4, 15),
                    Gender = "Nam",
                    Phone = "0912345671",
                    Email = "hung.nguyen@phongkham.vn",
                    Qualification = "PGS.TS.BS. - Phó Giáo sư, Tiến sĩ Y khoa",
                    YearsOfExperience = 25,
                    ConsultationFee = 500000,
                    Status = DoctorStatus.Working
                },
                new()
                {
                    FullName = "Trần Thị Mai Loan",
                    SpecialtyId = timMach!.Id,
                    DateOfBirth = new DateTime(1982, 8, 20),
                    Gender = "Nữ",
                    Phone = "0912345672",
                    Email = "loan.tran@phongkham.vn",
                    Qualification = "TS.BS. - Tiến sĩ Y khoa Tim mạch",
                    YearsOfExperience = 18,
                    ConsultationFee = 400000,
                    Status = DoctorStatus.Working
                },
                new()
                {
                    FullName = "Lê Hoàng Nam",
                    SpecialtyId = noiTongQuat!.Id,
                    DateOfBirth = new DateTime(1978, 11, 5),
                    Gender = "Nam",
                    Phone = "0912345673",
                    Email = "nam.le@phongkham.vn",
                    Qualification = "BSCKII. - Bác sĩ Chuyên khoa II Nội tổng quát",
                    YearsOfExperience = 20,
                    ConsultationFee = 350000,
                    Status = DoctorStatus.Working
                },
                new()
                {
                    FullName = "Phạm Thu Trang",
                    SpecialtyId = noiTongQuat!.Id,
                    DateOfBirth = new DateTime(1988, 3, 12),
                    Gender = "Nữ",
                    Phone = "0912345674",
                    Email = "trang.pham@phongkham.vn",
                    Qualification = "ThS.BS. - Thạc sĩ Y học (ĐH Y Hà Nội)",
                    YearsOfExperience = 12,
                    ConsultationFee = 250000,
                    Status = DoctorStatus.Working
                },
                new()
                {
                    FullName = "Vũ Đức Thắng",
                    SpecialtyId = ngoaiKhoa!.Id,
                    DateOfBirth = new DateTime(1984, 6, 25),
                    Gender = "Nam",
                    Phone = "0912345675",
                    Email = "thang.vu@phongkham.vn",
                    Qualification = "BSCKI. - Bác sĩ Chuyên khoa I Ngoại Chấn thương",
                    YearsOfExperience = 15,
                    ConsultationFee = 300000,
                    Status = DoctorStatus.Working
                },
                new()
                {
                    FullName = "Đỗ Quốc Bảo",
                    SpecialtyId = ngoaiKhoa!.Id,
                    DateOfBirth = new DateTime(1992, 9, 18),
                    Gender = "Nam",
                    Phone = "0912345676",
                    Email = "bao.do@phongkham.vn",
                    Qualification = "BS. - Bác sĩ Đa khoa Ngoại khoa",
                    YearsOfExperience = 8,
                    ConsultationFee = 200000,
                    Status = DoctorStatus.OnLeave
                },
                new()
                {
                    FullName = "Hoàng Minh Tuấn",
                    SpecialtyId = nhiKhoa!.Id,
                    DateOfBirth = new DateTime(1983, 1, 30),
                    Gender = "Nam",
                    Phone = "0912345677",
                    Email = "tuan.hoang@phongkham.vn",
                    Qualification = "TS.BS. - Tiến sĩ Nhi khoa - Hồi sức Nhi",
                    YearsOfExperience = 16,
                    ConsultationFee = 350000,
                    Status = DoctorStatus.Working
                },
                new()
                {
                    FullName = "Nguyễn Bích Ngọc",
                    SpecialtyId = nhiKhoa!.Id,
                    DateOfBirth = new DateTime(1990, 7, 14),
                    Gender = "Nữ",
                    Phone = "0912345678",
                    Email = "ngoc.nguyen@phongkham.vn",
                    Qualification = "ThS.BS. - Thạc sĩ Nhi khoa",
                    YearsOfExperience = 10,
                    ConsultationFee = 250000,
                    Status = DoctorStatus.Working
                },
                new()
                {
                    FullName = "Phan Thanh Hà",
                    SpecialtyId = daLieu!.Id,
                    DateOfBirth = new DateTime(1976, 12, 8),
                    Gender = "Nữ",
                    Phone = "0912345679",
                    Email = "ha.phan@phongkham.vn",
                    Qualification = "BSCKII. - Bác sĩ CKII Da liễu & Thẩm mỹ",
                    YearsOfExperience = 22,
                    ConsultationFee = 450000,
                    Status = DoctorStatus.Working
                },
                new()
                {
                    FullName = "Đặng Quỳnh Nga",
                    SpecialtyId = daLieu!.Id,
                    DateOfBirth = new DateTime(1991, 5, 22),
                    Gender = "Nữ",
                    Phone = "0912345680",
                    Email = "nga.dang@phongkham.vn",
                    Qualification = "BSCKI. - Bác sĩ Chuyên khoa I Da liễu",
                    YearsOfExperience = 9,
                    ConsultationFee = 220000,
                    Status = DoctorStatus.Working
                },
                new()
                {
                    FullName = "Trịnh Đình Khang",
                    SpecialtyId = taiMuiHong!.Id,
                    DateOfBirth = new DateTime(1985, 10, 3),
                    Gender = "Nam",
                    Phone = "0912345681",
                    Email = "khang.trinh@phongkham.vn",
                    Qualification = "ThS.BS. - Thạc sĩ Tai Mũi Họng",
                    YearsOfExperience = 14,
                    ConsultationFee = 280000,
                    Status = DoctorStatus.Working
                },
                new()
                {
                    FullName = "Bùi Phương Thảo",
                    SpecialtyId = taiMuiHong!.Id,
                    DateOfBirth = new DateTime(1989, 2, 19),
                    Gender = "Nữ",
                    Phone = "0912345682",
                    Email = "thao.bui@phongkham.vn",
                    Qualification = "BSCKI. - Bác sĩ CKI Tai Mũi Họng",
                    YearsOfExperience = 11,
                    ConsultationFee = 250000,
                    Status = DoctorStatus.Inactive
                },
                new()
                {
                    FullName = "Cao Văn Minh",
                    SpecialtyId = matKhoa!.Id,
                    DateOfBirth = new DateTime(1979, 8, 27),
                    Gender = "Nam",
                    Phone = "0912345683",
                    Email = "minh.cao@phongkham.vn",
                    Qualification = "BSCKII. - Bác sĩ Chuyên khoa II Nhãn khoa",
                    YearsOfExperience = 19,
                    ConsultationFee = 350000,
                    Status = DoctorStatus.Working
                }
            };

            context.Doctors.AddRange(doctors);
            await context.SaveChangesAsync();
        }

        await SeedDoctorAccountsAsync(context);
    }

    public static async Task SeedDoctorAccountsAsync(AppDbContext context)
    {
        var doctors = await context.Doctors
            .Where(d => d.AccountId == null)
            .ToListAsync();

        if (doctors.Count == 0)
        {
            return;
        }

        foreach (var doctor in doctors)
        {
            var baseName = (doctor.Email?.Split('@')[0] ?? $"bs{doctor.Id}")
                .Trim().ToLowerInvariant().Replace(" ", string.Empty).Replace(".", string.Empty);
            if (string.IsNullOrWhiteSpace(baseName))
            {
                baseName = $"bs{doctor.Id}";
            }

            var username = baseName;
            var suffix = 1;
            while (await context.Accounts.AnyAsync(a => a.Username == username))
            {
                suffix++;
                username = $"{baseName}{suffix}";
            }

            var account = new Account
            {
                Username = username,
                PasswordHash = PasswordHasher.Hash("Doctor@123"),
                FullName = doctor.FullName,
                Email = doctor.Email,
                Role = Role.Doctor,
                Status = AccountStatus.Active
            };

            context.Accounts.Add(account);
            await context.SaveChangesAsync();

            doctor.AccountId = account.Id;
        }

        await context.SaveChangesAsync();
    }
}
