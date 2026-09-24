# AGENTS.md — Hệ thống Quản lý Phòng khám

> Docs Tiếng Việt. Glossary duy nhất tại `CONTEXT.md`. File này + `CLAUDE.md` dùng chung nội dung vận hành.

## 1. Tổng quan

Web Hệ thống quản lý phòng khám và đăng ký lịch khám bằng ASP.NET Core 10 MVC.

- **Admin:** quản lý chuyên khoa, bác sĩ, bệnh nhân, lịch khám, phiếu đăng ký khám, xem thống kê.
- **Bệnh nhân:** đăng nhập, xem danh sách bác sĩ và lịch khám, tìm kiếm, đăng ký lịch khám, xem lịch đã đăng ký của mình.

Chi tiết thuật ngữ xem `CONTEXT.md`. Không định nghĩa lại thuật ngữ ở đây.

## 2. Công nghệ bắt buộc

- .NET 10 SDK, ASP.NET Core 10 MVC, C#, Entity Framework Core 10, SQL Server, LINQ.
- Razor View, HTML/CSS, JavaScript ở mức cần thiết.
- Visual Studio Community 2026 hoặc VS Code. Git/GitHub.
- Project hiện tại: `clinic_management/clinic_management.csproj` (`net10.0`). Package EF Core khi thêm phải là bản 10.x tương thích .NET 10.

## 3. Điều không chấp nhận

- ASP.NET MVC trên .NET Framework.
- ASP.NET Core phiên bản thấp hơn 10.
- Project cũ đổi hình thức nhưng không đúng công nghệ trên.
- Sinh viên phải hiểu, giải thích và chỉnh sửa được mã nguồn khi bảo vệ. Không commit code không hiểu.

## 4. Quy ước commit — BẮT BUỘC

Mọi commit phải chứa đủ 3 thông tin: **mã sinh viên, tên sinh viên, nội dung thay đổi**. Commit thiếu sẽ bị yêu cầu sửa lại.

Format:

```
[MSSV-HoTen] <type>: <mô tả ngắn bằng Tiếng Việt>
```

- Ví dụ: `[2312001-NguyenVanA] feat: thêm CRUD Chuyên khoa`
- Ví dụ: `[2312001-NguyenVanA] fix: sửa lỗi tìm kiếm bác sĩ theo tên`
- `<type>` gợi ý: `feat`, `fix`, `docs`, `refactor`, `chore`, `test`.

## 5. Quy ước làm việc

- Docs Tiếng Việt. Code (class, property, biến) dùng English.
- `CONTEXT.md` chỉ là glossary, không ghi chi tiết implementation.
- Chi tiết còn treo (sẽ grill tiếp): phân biệt Lịch khám vs Phiếu đăng ký khám (Q6), Auth Identity vs custom (Q7). Không tự chốt khi chưa xác nhận với chủ repo.
- Giữ `AGENTS.md` và `CLAUDE.md` đồng bộ khi sửa đổi.
