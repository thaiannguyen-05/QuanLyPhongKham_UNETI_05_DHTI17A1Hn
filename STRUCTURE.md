# STRUCTURE.md — Cây thư mục và config dự án (Services/ đã chốt, Q6/Q7 còn treo)

> Docs Tiếng Việt. Code dùng English. `CONTEXT.md` vẫn chỉ là glossary, không copy định nghĩa từ đó sang đây.

## 1. Config hiện tại đã có trong code (đã kiểm tra)

- `clinic_management/clinic_management.csproj:3-6`: `net10.0`, `Nullable enable`, `ImplicitUsings enable`, SDK `Microsoft.NET.Sdk.Web`. Chưa có package EF Core / SQL Server / Identity.
- `clinic_management/Program.cs:3`: chỉ `AddControllersWithViews()`. Chưa có `DbContext`, `ConnectionStrings`, DI cho service, `Authentication` (hiện chỉ có `UseAuthorization()` dòng 19), Session/Cookie.
- `clinic_management/appsettings.json:1-9` + `appsettings.Development.json:1-8`: chỉ `Logging` + `AllowedHosts`. Chưa có `ConnectionStrings`.
- `clinic_management/Controllers/`: chỉ `HomeController.cs` template. Chưa có controller nghiệp vụ.
- `clinic_management/Models/`: chỉ `ErrorViewModel.cs`. Chưa có entity.
- `clinic_management/Views/`: template `Home/`, `Shared/`, `_ViewImports.cshtml`, `_ViewStart.cshtml`.
- Route mặc định `clinic_management/Program.cs:23-26`: `{controller=Home}/{action=Index}/{id?}`.

## 2. Config chưa chốt (treo, không tự chốt code)

- Q6 (`AGENTS.md:46`): tách `Lịch khám` vs `Phiếu đăng ký khám` thành 2 entity/module riêng hay gộp — treo.
- Q7 (`AGENTS.md:46`): Auth dùng Identity hay custom (Cookie/Session tự viết) — treo, quyết định trực tiếp tới `Services/Auth/`.
- Chưa chốt: provider EF Core 10.x + SQL Server, chuỗi kết nối, `Data/AppDbContext.cs`, có dùng `Areas/Admin` cho quản trị viên hay dùng chung Controller + phân quyền, chiến lược Seed dữ liệu, test.

## 3. Cây thư mục mục tiêu (tuân thủ MVC)

Nguyên tắc (pilot Auth 2026-09-25): backend theo feature `Modules/<Tên>/{Controllers,Services,Models}`, `Views/<Tên>/` tách riêng. `Modules/Auth, Account, Home, Specialty/` đã xong. Shared giữ nguyên: `Models/Schema/`, `Data/`, `Common/`, `Enums/`, `Views/` tách riêng. Module mới (`Doctor, Patient, Schedule, Booking, Statistic`) làm theo cùng mẫu.

```text
clinic_management/
  Controllers/                  # DUY NHẤT chứa Controller
    HomeController.cs           # có sẵn
    SpecialtyController.cs      # dự kiến (Admin)
    DoctorController.cs         # dự kiến (Admin + Bệnh nhân xem/tìm kiếm)
    PatientController.cs        # dự kiến (Admin)
    ScheduleController.cs       # dự kiến (Lịch khám)
    BookingController.cs        # dự kiến (Phiếu đăng ký khám, phụ thuộc Q6)
    StatisticController.cs      # dự kiến (Thống kê)
    AuthController.cs           # dự kiến (Đăng nhập/đăng xuất, phụ thuộc Q7)
  Models/                       # ViewModel / DTO cho View (không để entity trần ở đây khi đã có Data/)
    ErrorViewModel.cs           # có sẵn
  Views/
    Home/ Shared/               # có sẵn
    Specialty/ Doctor/ Patient/ Schedule/ Booking/ Statistic/ Auth/  # dự kiến, 1 folder/Views 1 Controller
  Data/
    AppDbContext.cs             # dự kiến, EF Core 10.x + SQL Server (chưa chốt connection string)
  Services/                     # mỗi nghiệp vụ 1 folder, chỉ Service + Interface (đã chốt)
    Specialty/
      ISpecialtyService.cs
      SpecialtyService.cs
    Doctor/
      IDoctorService.cs
      DoctorService.cs
    Patient/
      IPatientService.cs
      PatientService.cs
    Schedule/
      IScheduleService.cs
      ScheduleService.cs        # phụ thuộc Q6
    Booking/
      IBookingService.cs
      BookingService.cs         # phụ thuộc Q6
    Statistic/
      IStatisticService.cs
      StatisticService.cs
    Auth/
      IAuthService.cs
      AuthService.cs            # phụ thuộc Q7, chưa chốt Identity vs custom
  Properties/
  wwwroot/
  appsettings.json              # dự kiến thêm ConnectionStrings (chưa chốt)
  Program.cs                    # dự kiến thêm DbContext + AddScoped Interface cho từng Service (Q6/Q7 chốt mới đủ danh sách)
```

## 4. Quy ước áp dụng (đã chốt, đồng bộ `AGENTS.md:§6`)

1. Controller mỏng: chỉ nhận request, check `ModelState`, gọi `Services/*/*Service`, trả View/Redirect, gắn `[Authorize]/[ValidateAntiForgeryToken]`, `try-catch` exception có sẵn của .NET rồi trả View lỗi. Cấm `DbContext`, LINQ (`Where/ToList/...`), SQL/`SaveChanges`, validation nghiệp vụ, `new Service()` thủ công.
2. Service dày: chứa LINQ + EF Core + validation nghiệp vụ theo từng subfolder `Services/<Tên>/` (`I<Tên>Service.cs + <Tên>Service.cs`, method English như `GetAllAsync/SearchAsync/CreateAsync`). Được inject Service khác qua DI. Lỗi nghiệp vụ `throw` exception có sẵn của .NET với message Tiếng Việt. Đăng ký `Interface + AddScoped` trong `Program.cs`.
3. Ví dụ đúng (tìm kiếm Bác sĩ theo tên + Chuyên khoa): Controller gọi `await _doctorService.SearchAsync(name, specialtyId)`; LINQ `Where` nằm trong `DoctorService.SearchAsync`.
4. Khi Q6/Q7 chốt mới tạo code tương ứng trong `Services/Schedule/`, `Services/Booking/`, `Services/Auth/`. Hiện chỉ là placeholder trong cây thư mục này.
