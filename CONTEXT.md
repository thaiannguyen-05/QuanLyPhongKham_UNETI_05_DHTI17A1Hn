# Quản Lý Phòng Khám

Hệ thống web quản lý phòng khám và đăng ký lịch khám bằng ASP.NET Core 10 MVC.

## Language

### Vai trò

**Quản trị viên**:
Người quản trị toàn bộ hệ thống phòng khám.
_Avoid_: Admin viết tắt trong docs, Người dùng chung chung

**Bệnh nhân**:
Người đăng ký và sử dụng dịch vụ khám bệnh.
_Avoid_: Khách hàng, Client, User chung chung

### Tài khoản đăng nhập

**Tài khoản**:
Thông tin đăng nhập hệ thống của một Quản trị viên hoặc một Bệnh nhân.
_Avoid_: User chung chung, Client

### Nghiệp vụ cốt lõi

**Chuyên khoa**:
Lĩnh vực chuyên môn của phòng khám dùng để nhóm bác sĩ.
_Avoid_: Khoa, Department chung chung

**Bác sĩ**:
Người hành nghề khám chữa bệnh, thuộc một chuyên khoa.
_Avoid_: Physician, Thầy thuốc

**Lịch khám**:
Khung thời gian khám do quản trị viên thiết lập để bệnh nhân đăng ký.
_Avoid_: Ca khám, Slot (trong docs, chỉ dùng trong code nếu cần)

**Phiếu đăng ký khám**:
Lượt đăng ký của một bệnh nhân vào một lịch khám cụ thể.
_Avoid_: Đơn khám, Booking, Appointment dùng trong docs

**Thống kê**:
Thông tin tổng hợp để quản trị viên theo dõi hoạt động phòng khám.
_Avoid_: Report, Báo cáo chi tiết
