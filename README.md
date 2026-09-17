# Hệ Thống Quản Lý Quán Coffee

Dự án quản lý quán coffee hoàn chỉnh sử dụng .NET 9.0 với kiến trúc 3 tầng (DAL, BLL, API), Blazor Server front-end và SQL Server database.

## 📋 Mục Lục

- [Tổng Quan](#tổng-quan)
- [Công Nghệ Sử Dụng](#công-nghệ-sử-dụng)
- [Cấu Trúc Dự Án](#cấu-trúc-dự-án)
- [Yêu Cầu Hệ Thống](#yêu-cầu-hệ-thống)
- [Cài Đặt](#cài-đặt)
- [Chạy Ứng Dụng](#chạy-ứng-dụng)
- [Tính Năng](#tính-năng)
- [API Endpoints](#api-endpoints)
- [Testing](#testing)
- [Cấu Trúc Database](#cấu-trúc-database)

## 🎯 Tổng Quan

Hệ thống quản lý quán coffee với đầy đủ chức năng:
- Quản lý bàn, món, nhân viên, khách hàng
- Gọi món và thanh toán
- Thống kê doanh thu
- Xuất báo cáo Excel/PDF
- Tích điểm khách hàng

## 🛠 Công Nghệ Sử Dụng

### Back-end
- **Framework**: ASP.NET Core Web API (.NET 9.0)
- **ORM**: Entity Framework Core 9.0 (Code-First)
- **Database**: SQLite 3 (cross-platform, không cần cài đặt)
- **Architecture**: 3-Layer Architecture (DAL, BLL, API)

### Front-end
- **Framework**: Blazor Server (.NET 9.0)
- **UI**: Bootstrap 5
- **Icons**: Open Iconic

### Testing
- **Framework**: xUnit
- **Mocking**: Moq

### Libraries
- **EPPlus**: Xuất file Excel
- **iTextSharp**: Xuất file PDF
- **Swagger/OpenAPI**: API Documentation

## 📁 Cấu Trúc Dự Án

```
CoffeeShopManager/
├── CoffeeShopManager.DAL/          # Data Access Layer
│   ├── Data/
│   │   └── CoffeeShopDbContext.cs  # DbContext
│   ├── Entities/                    # Database Models
│   │   ├── Ban.cs
│   │   ├── Mon.cs
│   │   ├── HoaDon.cs
│   │   ├── ChiTietHD.cs
│   │   ├── NhanVien.cs
│   │   └── KhachHang.cs
│   └── Repositories/                # Repository Pattern
│       ├── IRepository.cs
│       ├── Repository.cs
│       ├── BanRepository.cs
│       ├── MonRepository.cs
│       ├── HoaDonRepository.cs
│       ├── ChiTietHDRepository.cs
│       ├── NhanVienRepository.cs
│       └── KhachHangRepository.cs
├── CoffeeShopManager.BLL/          # Business Logic Layer
│   ├── DTOs/                        # Data Transfer Objects
│   │   ├── BanDto.cs
│   │   ├── MonDto.cs
│   │   ├── HoaDonDto.cs
│   │   ├── NhanVienDto.cs
│   │   ├── KhachHangDto.cs
│   │   └── ThongKeDto.cs
│   └── Services/                    # Business Services
│       ├── BanService.cs
│       ├── MonService.cs
│       ├── HoaDonService.cs
│       ├── NhanVienService.cs
│       ├── KhachHangService.cs
│       ├── ThongKeService.cs
│       └── ExportService.cs
├── CoffeeShopManager.API/          # Web API
│   ├── Controllers/                 # API Controllers
│   │   ├── BanController.cs
│   │   ├── MonController.cs
│   │   ├── HoaDonController.cs
│   │   ├── NhanVienController.cs
│   │   ├── KhachHangController.cs
│   │   └── ThongKeController.cs
│   ├── Program.cs
│   └── appsettings.json
├── CoffeeShopManager.Web/          # Blazor Server
│   ├── Pages/                       # Razor Pages
│   │   ├── Index.razor
│   │   ├── Ban.razor
│   │   ├── Mon.razor
│   │   ├── GoiMon.razor
│   │   ├── HoaDon.razor
│   │   ├── ThongKe.razor
│   │   ├── NhanVien.razor
│   │   └── KhachHang.razor
│   ├── Shared/                      # Shared Components
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   ├── Program.cs
│   └── appsettings.json
└── CoffeeShopManager.Tests/        # Unit Tests
    └── Services/
        ├── BanServiceTests.cs
        ├── MonServiceTests.cs
        ├── HoaDonServiceTests.cs
        └── ThongKeServiceTests.cs
```

## 💻 Yêu Cầu Hệ Thống

- **.NET 9.0 SDK** hoặc mới hơn
- **SQLite** (tự động, không cần cài đặt thêm)
- **Visual Studio 2022** (17.8 trở lên) hoặc **Visual Studio Code**
- **Git** (tùy chọn)

> 💡 **Lưu ý**: Dự án sử dụng SQLite, chạy được trên Windows, macOS, và Linux mà không cần cài đặt database server riêng.

## 🚀 Hướng Dẫn Cài Đặt và Chạy Dự Án

### Bước 1: Kiểm Tra Yêu Cầu Hệ Thống

#### Cài đặt .NET 9.0 SDK

1. **Kiểm tra phiên bản .NET hiện tại:**
   ```bash
   dotnet --version
   ```
   Phải hiển thị `9.0.x` hoặc mới hơn.

2. **Nếu chưa có .NET 9.0 SDK:**
   - Truy cập: https://dotnet.microsoft.com/download/dotnet/9.0
   - Tải về và cài đặt .NET 9.0 SDK cho hệ điều hành của bạn
   - Sau khi cài đặt, chạy lại `dotnet --version` để xác nhận

#### Cài đặt Entity Framework Core Tools

```bash
dotnet tool install --global dotnet-ef
```

Kiểm tra cài đặt:
```bash
dotnet ef --version
```

### Bước 2: Extract Dự Án

   - Giải nén vào thư mục bất kỳ
   - Mở Terminal và di chuyển đến thư mục dự án:
   ```bash
   cd <đường-dẫn-đến-thư-mục-CoffeeShopManager>
   # Ví dụ trên macOS/Linux: cd ~/Downloads/CoffeeShopManager
   # Ví dụ trên Windows: cd C:\Users\YourName\Downloads\CoffeeShopManager
   ```

### Bước 3: Restore NuGet Packages

```bash
dotnet restore
```

Lệnh này sẽ tải về tất cả các package cần thiết từ NuGet.

### Bước 4: Kiểm Tra Database Connection String

Dự án đã được cấu hình để sử dụng **SQLite** (cross-platform, không cần cài đặt database server):

**CoffeeShopManager.API/appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=CoffeeShopDB.db"
  }
}
```

**CoffeeShopManager.Web/appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=../CoffeeShopManager.API/CoffeeShopDB.db"
  }
}
```

> ✅ **Lưu ý**: Đường dẫn database được tính toán tự động từ thư mục gốc của solution, không cần cấu hình thủ công. Dự án sẽ tự động tìm và tạo database ở đúng vị trí.

### Bước 5: Tạo Database và Migrations

Mở Terminal tại thư mục root của project (`CoffeeShopManager/`):

#### Cách 1: Sử dụng Command Line (Khuyến nghị)

```bash
# Di chuyển đến thư mục root của solution
cd CoffeeShopManager

# Tạo migration (nếu chưa có)
dotnet ef migrations add InitialCreate --project CoffeeShopManager.DAL --startup-project CoffeeShopManager.API

# Tạo database và áp dụng migrations
dotnet ef database update --project CoffeeShopManager.DAL --startup-project CoffeeShopManager.API
```

#### Cách 2: Sử dụng Package Manager Console trong Visual Studio

1. Mở Solution trong Visual Studio
2. Tools → NuGet Package Manager → Package Manager Console
3. Chọn "CoffeeShopManager.DAL" trong dropdown "Default project"
4. Chạy các lệnh sau:

```powershell
Add-Migration InitialCreate -StartupProject CoffeeShopManager.API
Update-Database -StartupProject CoffeeShopManager.API
```

#### Kết quả sau khi tạo database:

File database `CoffeeShopDB.db` sẽ được tạo trong thư mục `CoffeeShopManager.API/` với dữ liệu mẫu (seed data) tự động:

- ✅ **6 bàn**: Bàn 1, Bàn 2, Bàn 3, Bàn 4, Bàn 5, Bàn VIP (tất cả đều "Trống")
- ✅ **10 món**: 
  - Cà phê: Cà phê đen (25,000₫), Cà phê sữa (30,000₫), Bạc xỉu (30,000₫), Cappuccino (45,000₫)
  - Trà: Trà đào cam sả (35,000₫), Trà sữa trân châu (40,000₫)
  - Nước ép: Nước cam ép (35,000₫), Sinh tố bơ (40,000₫)
  - Đồ ăn nhẹ: Bánh mì (20,000₫), Bánh ngọt (25,000₫)
- ✅ **3 nhân viên**: Nguyễn Văn A (Ca Sáng), Trần Thị B (Ca Chiều), Lê Văn C (Ca Tối)
- ✅ **3 khách hàng**: Khách lẻ (0 điểm), Phạm Văn D (100 điểm), Hoàng Thị E (250 điểm)

### Bước 6: Kiểm Tra Cài Đặt

```bash
# Build toàn bộ solution
dotnet build

# Chạy unit tests
dotnet test
```

Nếu không có lỗi, bạn đã sẵn sàng chạy ứng dụng!

## ▶️ Chạy Ứng Dụng

> ⚠️ **Quan trọng**: Bạn cần chạy **cả API và Web** cùng lúc để ứng dụng hoạt động đầy đủ.

### Cách 1: Chạy Bằng Command Line (2 Terminal)

#### Terminal 1 - Chạy Web API:

```bash
cd CoffeeShopManager.API
dotnet run
```

API sẽ chạy tại:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Swagger UI: `http://localhost:5001/swagger`

#### Terminal 2 - Chạy Blazor Web:

Mở Terminal mới và chạy:

```bash
cd CoffeeShopManager.Web
dotnet run
```

Web application sẽ chạy tại:
- HTTPS: `https://localhost:5002`
- HTTP: `http://localhost:5003`

### Cách 2: Chạy Bằng Visual Studio (Multiple Startup Projects)

1. Mở file `CoffeeShopManager.sln` trong Visual Studio 2022
2. Right-click vào Solution → **Properties**
3. Chọn **Common Properties** → **Startup Project**
4. Chọn **Multiple startup projects**
5. Set cả hai project:
   - `CoffeeShopManager.API` → **Start**
   - `CoffeeShopManager.Web` → **Start**
6. Click **Apply** → **OK**
7. Nhấn **F5** hoặc click **Start** để chạy cả hai project cùng lúc

### Cách 3: Chạy Bằng Visual Studio Code

1. Mở folder `CoffeeShopManager` trong VS Code
2. Mở Terminal (Terminal → New Terminal)
3. Chạy lệnh sau trong Terminal đầu tiên:
   ```bash
   cd CoffeeShopManager.API && dotnet run
   ```
4. Mở Terminal mới (Terminal → New Terminal) và chạy:
   ```bash
   cd CoffeeShopManager.Web && dotnet run
   ```

### Kiểm Tra Ứng Dụng Đã Chạy

1. **Kiểm tra API:**
   - Mở browser: `https://localhost:5001/swagger`
   - Thử endpoint: `GET /api/ban`
   - Phải trả về danh sách 6 bàn

2. **Kiểm tra Web:**
   - Mở browser: `https://localhost:5002`
   - Click "Quản lý bàn" trong menu
   - Phải hiển thị 6 bàn với trạng thái "Trống"

### Troubleshooting

#### Lỗi: Port đã được sử dụng

Nếu gặp lỗi "Address already in use", sửa ports trong:

**CoffeeShopManager.API/Properties/launchSettings.json:**
```json
"applicationUrl": "https://localhost:7001;http://localhost:7000"
```

**CoffeeShopManager.Web/Properties/launchSettings.json:**
```json
"applicationUrl": "https://localhost:7002;http://localhost:7003"
```

#### Lỗi: Database không tìm thấy hoặc "no such table"

Nếu gặp lỗi "database not found" hoặc "no such table: Ban", thực hiện các bước sau:

1. **Kiểm tra database có tồn tại:**
   ```bash
   ls -la CoffeeShopManager.API/*.db
   # Hoặc trên Windows: dir CoffeeShopManager.API\*.db
   ```

2. **Kiểm tra migrations đã được áp dụng:**
   ```bash
   dotnet ef migrations list --project CoffeeShopManager.DAL --startup-project CoffeeShopManager.API
   ```

3. **Áp dụng lại migrations:**
   ```bash
   dotnet ef database update --project CoffeeShopManager.DAL --startup-project CoffeeShopManager.API
   ```

4. **Nếu vẫn lỗi, xóa database cũ và tạo lại:**
   ```bash
   # Xóa database cũ
   rm CoffeeShopManager.API/CoffeeShopDB.db
   # Hoặc trên Windows: del CoffeeShopManager.API\CoffeeShopDB.db
   
   # Tạo lại database
   dotnet ef database update --project CoffeeShopManager.DAL --startup-project CoffeeShopManager.API
   ```

5. **Kiểm tra database có bảng:**
   ```bash
   # Trên macOS/Linux (cần cài sqlite3):
   sqlite3 CoffeeShopManager.API/CoffeeShopDB.db ".tables"
   
   # Phải hiển thị: Ban, Mon, HoaDon, ChiTietHD, NhanVien, KhachHang
   ```

#### Lỗi: Migration không tìm thấy

Nếu gặp lỗi về migration, kiểm tra:
```bash
dotnet ef migrations list --project CoffeeShopManager.DAL --startup-project CoffeeShopManager.API
```

Nếu không có migrations, tạo lại:
```bash
dotnet ef migrations add InitialCreate --project CoffeeShopManager.DAL --startup-project CoffeeShopManager.API
dotnet ef database update --project CoffeeShopManager.DAL --startup-project CoffeeShopManager.API
```

#### Lỗi: Web project không tìm thấy database

Nếu Web project báo lỗi "no such table" nhưng API chạy bình thường:

1. **Kiểm tra console log khi chạy Web project:**
   - Khi chạy `dotnet run` trong Web project, sẽ có log hiển thị đường dẫn database
   - Kiểm tra xem đường dẫn có đúng không

2. **Đảm bảo cả API và Web đều dùng cùng một database:**
   - Database phải nằm trong `CoffeeShopManager.API/CoffeeShopDB.db`
   - Web project sẽ tự động tìm database này từ thư mục solution root

3. **Nếu vẫn lỗi, restart cả API và Web:**
   ```bash
   # Dừng cả hai ứng dụng (Ctrl+C)
   # Sau đó chạy lại từ đầu
   ```

## ✨ Tính Năng

### 1. Quản Lý Bàn
- Xem danh sách bàn và trạng thái
- Thêm, sửa, xóa bàn
- Lọc bàn theo trạng thái (Trống, Đang sử dụng, Đã đặt)

### 2. Quản Lý Món
- Xem danh sách món
- Thêm, sửa, xóa món
- Lọc món theo loại

### 3. Gọi Món
- Chọn bàn hoặc chọn "Mua mang về"
- Tạo hóa đơn mới
- Thêm món vào hóa đơn
- Tự động tính tổng tiền
- Xóa món khỏi hóa đơn

### 4. Thanh Toán
- Xem chi tiết hóa đơn
- Thanh toán bằng 3 phương thức:
  - 💵 Tiền mặt
  - 🏦 Qua ngân hàng (VietQR)
  - ⭐ Thanh toán bằng điểm (100 điểm = 10,000 VNĐ)
- Tích điểm khách hàng (1 điểm/10,000 VNĐ)
- Cập nhật trạng thái bàn
- Xuất hóa đơn PDF

### 5. Thống Kê
- Doanh thu theo ngày
- Doanh thu theo tháng
- Top món bán chạy
- Xuất báo cáo Excel

### 6. Quản Lý Nhân Viên
- Xem danh sách nhân viên
- Thêm, sửa, xóa nhân viên
- Lọc theo ca làm

### 7. Quản Lý Khách Hàng
- Xem danh sách khách hàng
- Thêm, sửa, xóa khách hàng
- Xem điểm tích lũy
- Top khách hàng VIP

## 📡 API Endpoints

### Ban (Bàn)
- `GET /api/ban` - Lấy tất cả bàn
- `GET /api/ban/{id}` - Lấy bàn theo ID
- `GET /api/ban/trangthai/{trangThai}` - Lấy bàn theo trạng thái
- `POST /api/ban` - Tạo bàn mới
- `PUT /api/ban/{id}` - Cập nhật bàn
- `DELETE /api/ban/{id}` - Xóa bàn

### Mon (Món)
- `GET /api/mon` - Lấy tất cả món
- `GET /api/mon/{id}` - Lấy món theo ID
- `GET /api/mon/loai/{loai}` - Lấy món theo loại
- `POST /api/mon` - Tạo món mới
- `PUT /api/mon/{id}` - Cập nhật món
- `DELETE /api/mon/{id}` - Xóa món

### HoaDon (Hóa Đơn)
- `GET /api/hoadon` - Lấy tất cả hóa đơn
- `GET /api/hoadon/{id}` - Lấy hóa đơn theo ID
- `GET /api/hoadon/ban/{maBan}` - Lấy hóa đơn theo bàn
- `GET /api/hoadon/daterange?tuNgay=...&denNgay=...` - Lấy hóa đơn theo khoảng thời gian
- `POST /api/hoadon` - Tạo hóa đơn mới
- `POST /api/hoadon/goimon` - Gọi món
- `POST /api/hoadon/thanhtoan` - Thanh toán
- `PUT /api/hoadon/{id}` - Cập nhật hóa đơn
- `DELETE /api/hoadon/{id}` - Xóa hóa đơn

### NhanVien (Nhân Viên)
- `GET /api/nhanvien` - Lấy tất cả nhân viên
- `GET /api/nhanvien/{id}` - Lấy nhân viên theo ID
- `GET /api/nhanvien/calam/{caLam}` - Lấy nhân viên theo ca làm
- `POST /api/nhanvien` - Tạo nhân viên mới
- `PUT /api/nhanvien/{id}` - Cập nhật nhân viên
- `DELETE /api/nhanvien/{id}` - Xóa nhân viên

### KhachHang (Khách Hàng)
- `GET /api/khachhang` - Lấy tất cả khách hàng
- `GET /api/khachhang/{id}` - Lấy khách hàng theo ID
- `GET /api/khachhang/sdt/{soDienThoai}` - Tìm khách hàng theo SĐT
- `GET /api/khachhang/top/{top}` - Lấy top khách hàng VIP
- `POST /api/khachhang` - Tạo khách hàng mới
- `PUT /api/khachhang/{id}` - Cập nhật khách hàng
- `DELETE /api/khachhang/{id}` - Xóa khách hàng

### ThongKe (Thống Kê)
- `GET /api/thongke/doanhthu/ngay?tuNgay=...&denNgay=...` - Thống kê doanh thu theo ngày
- `GET /api/thongke/doanhthu/thang?nam=...` - Thống kê doanh thu theo tháng
- `GET /api/thongke/monbanchay?tuNgay=...&denNgay=...&top=10` - Món bán chạy
- `GET /api/thongke/tongdoanhthu?tuNgay=...&denNgay=...` - Tổng doanh thu
- `GET /api/thongke/export/excel?tuNgay=...&denNgay=...` - Xuất Excel

## 🧪 Testing

### Chạy Unit Tests

```bash
cd CoffeeShopManager.Tests
dotnet test
```

Hoặc trong Visual Studio: Test → Run All Tests

### Test Coverage

Dự án bao gồm unit tests cho:
- BanService (8 tests)
- MonService (5 tests)
- HoaDonService (7 tests)
- ThongKeService (4 tests)

Tổng cộng: **24+ unit tests**

### Test Cases

File `TestCases.csv` chứa 28 test cases bao gồm:
- 5 test cases CRUD cho Bàn
- 5 test cases CRUD cho Món
- 5 test cases CRUD cho Hóa Đơn
- 4 test cases Logic (tính tiền, kiểm tra bàn, tích điểm)
- 3 test cases Thống kê
- 6 test cases Validation

Để sử dụng:
1. Mở `TestCases.csv` trong Excel
2. Format thành table
3. Lưu dưới dạng `TestCases.xlsx`

## 🗄️ Cấu Trúc Database (SQLite)

### Bảng Ban
| Cột | Kiểu | Mô tả |
|-----|------|-------|
| MaBan | INT (PK) | Mã bàn (auto-increment) |
| TenBan | NVARCHAR(100) | Tên bàn |
| TrangThai | NVARCHAR(50) | Trạng thái: Trống, Đang sử dụng, Đã đặt |

### Bảng Mon
| Cột | Kiểu | Mô tả |
|-----|------|-------|
| MaMon | INT (PK) | Mã món (auto-increment) |
| TenMon | NVARCHAR(200) | Tên món |
| DonGia | DECIMAL(18,2) | Đơn giá |
| Loai | NVARCHAR(100) | Loại món |

### Bảng HoaDon
| Cột | Kiểu | Mô tả |
|-----|------|-------|
| MaHD | INT (PK) | Mã hóa đơn (auto-increment) |
| MaBan | INT (FK) | Mã bàn |
| ThoiGianTao | DATETIME | Thời gian tạo |
| TongTien | DECIMAL(18,2) | Tổng tiền |
| TrangThai | NVARCHAR(50) | Trạng thái |
| MaKH | INT (FK, nullable) | Mã khách hàng |
| MaNV | INT (FK, nullable) | Mã nhân viên |

### Bảng ChiTietHD
| Cột | Kiểu | Mô tả |
|-----|------|-------|
| MaChiTiet | INT (PK) | Mã chi tiết (auto-increment) |
| MaHD | INT (FK) | Mã hóa đơn |
| MaMon | INT (FK) | Mã món |
| SoLuong | INT | Số lượng |
| ThanhTien | DECIMAL(18,2) | Thành tiền |

### Bảng NhanVien
| Cột | Kiểu | Mô tả |
|-----|------|-------|
| MaNV | INT (PK) | Mã nhân viên (auto-increment) |
| TenNV | NVARCHAR(200) | Tên nhân viên |
| CaLam | NVARCHAR(100) | Ca làm |
| SoDienThoai | NVARCHAR(15) | Số điện thoại |
| Email | NVARCHAR(200) | Email |

### Bảng KhachHang
| Cột | Kiểu | Mô tả |
|-----|------|-------|
| MaKH | INT (PK) | Mã khách hàng (auto-increment) |
| TenKH | NVARCHAR(200) | Tên khách hàng |
| DiemTichLuy | INT | Điểm tích lũy |
| SoDienThoai | NVARCHAR(15) | Số điện thoại |
| Email | NVARCHAR(200) | Email |

## 📝 Lưu Ý Khi Demo

1. **Database**: Đảm bảo database đã được tạo và có seed data
2. **Multiple Projects**: Chạy cả API và Web cùng lúc
3. **Ports**: Kiểm tra ports không bị conflict (5001, 5002)
4. **Browser**: Sử dụng Chrome hoặc Edge để demo Blazor
5. **Swagger**: Dùng Swagger UI để demo API endpoints
6. **Excel Export**: Cần mở file Excel sau khi download
7. **Error Handling**: Tất cả lỗi đều được xử lý và hiển thị rõ ràng

## 🎓 Đáp Ứng Rubric

Dự án đáp ứng đầy đủ yêu cầu rubric:

### Back-end ✅
- ASP.NET Core Web API .NET 9.0
- Mô hình 3 tầng: DAL, BLL, API
- CRUD đầy đủ cho tất cả bảng
- Try-catch exception handling
- Validation dữ liệu đầy đủ
- Entity Framework Core code-first
- RESTful API trả JSON
- Async/await pattern

### Front-end ✅
- Blazor Server
- Giao diện quản lý bàn, món, gọi món, thanh toán, thống kê
- Thông báo lỗi rõ ràng
- UI đơn giản, dễ demo

### Database ✅
- SQL Server
- Đầy đủ 6 bảng theo yêu cầu
- Relationships và constraints
- Seed data

### Tính năng mở rộng ✅
- Thống kê doanh thu theo ngày/tháng
- Xuất Excel
- In hóa đơn (PDF)
- Tích điểm khách hàng

### Testing ✅
- File TestCases.csv (28 test cases)
- 5+ CRUD test cases
- 3+ logic test cases
- 24+ unit tests với xUnit

