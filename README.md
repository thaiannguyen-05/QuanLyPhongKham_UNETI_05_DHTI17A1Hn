# Hệ thống Quản lý Phòng khám

Web quản lý phòng khám và đăng ký lịch khám bằng ASP.NET Core 10 MVC.

- **Quản trị viên:** quản lý chuyên khoa, bác sĩ, bệnh nhân, lịch khám, phiếu đăng ký khám, xem thống kê.
- **Bệnh nhân:** đăng nhập, xem danh sách bác sĩ và lịch khám, tìm kiếm, đăng ký lịch khám, xem lịch đã đăng ký của mình.

Thuật ngữ dùng trong docs theo `CONTEXT.md`. Code (class, property, biến) dùng English.

## 1. Công nghệ

- .NET 10 SDK, ASP.NET Core 10 MVC, C#, Entity Framework Core 10 (dự kiến), SQL Server (dự kiến), LINQ.
- Razor View, HTML/CSS, JavaScript ở mức cần thiết.
- Visual Studio Community 2026 hoặc VS Code. Git/GitHub.
- Solution: `QuanLyPhongKham_UNETI_05_DHTI17A1Hn.slnx` (`net10.0`, `Nullable enable`, `ImplicitUsings enable`).

> Hiện trạng code đã kiểm tra: chỉ có `HomeController` template + 6 entity trong `Models/` (`TaiKhoan`, `ChuyenKhoa`, `BacSi`, `BenhNhan`, `LichKham`, `PhieuDangKyKham`) + enum `VaiTro`/`TrangThai`. Chưa có `DbContext`, `ConnectionStrings`, package EF Core, controller nghiệp vụ. Chi tiết xem `STRUCTURE.md`.

## 2. Yêu cầu hệ thống

### Bắt buộc

- OS: Windows 10/11 64-bit hoặc Ubuntu 22.04+.
- .NET 10 SDK (đã kiểm tra bản `10.0.112`): https://dotnet.microsoft.com/download/dotnet/10.0
- Git.
- IDE: Visual Studio Community 2026 hoặc VS Code + extension C# Dev Kit.
- RAM 4 GB+, disk trống 2 GB+.

### Dự kiến (khi chốt EF + SQL Server)

- SQL Server 2019+ hoặc Docker image `mcr.microsoft.com/mssql/server:2022-latest`.
- Package EF Core bản 10.x tương thích .NET 10 (chưa thêm vào code hiện tại).
- Tool EF CLI (khi cần migration):
  ```bash
  dotnet tool install --global dotnet-ef --version 10.*
  ```

Kiểm tra sau khi cài:

```bash
dotnet --version
dotnet --list-sdks
git --version
```

## 3. Setup hệ thống — toàn bộ các bước

### Bước 0. Clone repo

```bash
git clone git@github.com:thaiannguyen-05/QuanLyPhongKham_UNETI_05_DHTI17A1Hn.git
cd QuanLyPhongKham_UNETI_05_DHTI17A1Hn
# repo local hiện tên thư mục là clinic_management
```

### Bước 1. Mở project

- Visual Studio 2026: mở `QuanLyPhongKham_UNETI_05_DHTI17A1Hn.slnx`.
- VS Code: mở thư mục repo, extension C# tự nhận `.slnx`.

### Bước 2. Restore + Build

```bash
dotnet restore QuanLyPhongKham_UNETI_05_DHTI17A1Hn.slnx
dotnet build QuanLyPhongKham_UNETI_05_DHTI17A1Hn.slnx -c Release
```

### Bước 3. Cấu hình SQL Server (dự kiến — hiện chưa cần)

Hiện `appsettings.json` chỉ có `Logging` + `AllowedHosts`, chưa có `ConnectionStrings` nên app chạy được mà không cần DB. Khi nào chốt `Q6/Q7` và thêm `Data/AppDbContext.cs` mới làm tiếp:

1. Bật SQL Server local hoặc Docker:
   ```bash
   docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Pass@word123" \
     -p 1433:1433 --name sqlserver \
     -d mcr.microsoft.com/mssql/server:2022-latest
   ```
2. Thêm vào `appsettings.json` (mẫu, chưa chốt):
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost,1433;Database=QuanLyPhongKham;User Id=sa;Password=Pass@word123;TrustServerCertificate=True"
     }
   }
   ```
3. Chạy migration (khi đã có `AppDbContext`):
   ```bash
   dotnet ef migrations add Init --project QuanLyPhongKham_UNETI_05_DHTI17A1Hn
   dotnet ef database update --project QuanLyPhongKham_UNETI_05_DHTI17A1Hn
   ```

### Bước 4. Chạy app

```bash
dotnet run --project QuanLyPhongKham_UNETI_05_DHTI17A1Hn
```

Port mặc định theo `Properties/launchSettings.json`:

- HTTP: `http://localhost:5276`
- HTTPS: `https://localhost:7296`

Lần đầu chạy HTTPS cần trust cert (nếu trình duyệt báo):

```bash
dotnet dev-certs https --trust
```

### Bước 5. Kiểm tra

- Mở `https://localhost:7296` → trang `Home/Index` template hiện ra là OK.
- Route mặc định: `{controller=Home}/{action=Index}/{id?}`.

### Bước 6. Publish (tùy chọn)

```bash
dotnet publish QuanLyPhongKham_UNETI_05_DHTI17A1Hn/QuanLyPhongKham_UNETI_05_DHTI17A1Hn.csproj -c Release -o ./publish
```

CI (`.github/workflows/ci.yml`) chạy 2 jobs song song `build` + `publish` trên mỗi push/PR vào `main`.

## 4. Quy trình Git cho team mới — BẮT BUỘC

> Nguyên tắc cứng: **không code trực tiếp trên `main`**. Mỗi việc là 1 branch mới tách từ `main` mới nhất. Repo đang bật rule *Changes must be made through a pull request* nên push thẳng `main` sẽ bị chặn.

### 4.1. Lần đầu sau khi clone

```bash
git checkout main
git pull origin main
git status -sb
# phải thấy: ## main...origin/main, working tree clean
```

### 4.2. Mỗi lần làm việc mới — luôn tách branch mới từ main

```bash
# 1. Về main trước
git checkout main
# 2. Lấy code mới nhất về main rồi mới tách branch
git pull origin main
# 3. Tách branch mới (đặt tên rõ việc)
git checkout -b feature/crud-chuyen-khoa
```

Tên branch gợi ý: `feature/<việc>`, `fix/<lỗi>`, `docs/<tài-liệu>`. Ví dụ: `feature/crud-bac-si`, `fix/tim-kiem-bac-si`.

### 4.3. Làm + commit + push trên branch đó

```bash
git status -sb
git add <file-đã-sửa>
git commit -m "[MSSV-HoTen] feat: mô tả ngắn bằng Tiếng Việt"
git push -u origin feature/crud-chuyen-khoa
```

Lần push sau trên cùng branch chỉ cần `git push`.

### 4.4. Tạo Pull Request về main

1. Lên GitHub → `Compare & pull request`, base `main` ← compare là branch của bạn.
2. Chờ review, bấm `Merge pull request` (repo hiện dùng squash-merge).
3. Sau khi merge xong, GitHub sẽ xóa branch remote.

### 4.5. Sau khi PR đã merge — dọn và chuẩn bị việc tiếp theo

```bash
# 1. Về main
git checkout main
# 2. Pull code mới (đã gồm PR vừa merge)
git pull origin main
# 3. Xóa branch cũ local (đã merge xong)
git branch -d feature/crud-chuyen-khoa
# 4. Việc tiếp theo lại lặp từ 4.2: tách branch mới từ main
```

### 4.6. Lỗi team mới hay gặp

- `git pull` khi đang ở `feature/...` thì chỉ cập nhật branch đó, **không** cập nhật `main`. Muốn lấy code mới nhất của team: phải `git checkout main` rồi mới `git pull origin main`.
- Quên tách branch mới mà code tiếp trên branch cũ đã merge → dễ conflict. Luôn kiểm tra `git branch --show-current` phải là branch mới trước khi code.
- Push thẳng `main` bị chặn là đúng rule, đừng bypass. Tạo branch + PR.
- Trước khi tạo branch mới mà `git status` báo còn file sửa dở: commit hoặc `git stash` rồi mới `checkout main`.

## 5. Quy ước commit — BẮT BUỘC

```
[MSSV-HoTen] <type>: <mô tả ngắn bằng Tiếng Việt>
```

Ví dụ: `[23103100059-NguyenThaiAn] feat: thêm CRUD Chuyên khoa`. `<type>`: `feat`, `fix`, `docs`, `refactor`, `chore`, `test`.

## 6. Docs liên quan

- `AGENTS.md` / `CLAUDE.md`: quy ước vận hành.
- `CONTEXT.md`: glossary duy nhất (không định nghĩa lại ở file khác).
- `STRUCTURE.md`: cây thư mục mục tiêu và trạng thái config (Q6/Q7 còn treo).
