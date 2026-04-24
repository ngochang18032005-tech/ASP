# SIÊU TÀI LIỆU KỸ THUẬT VÀ VẬN HÀNH: LUXMANAGE RESORT ENTERPRISE SYSTEM
**Tên dự án:** Integrated Hotel & Restaurant Management System (LuxManage)
**Phiên bản:** 1.0.0 Stable
**Ngày cập nhật:** 24 tháng 04 năm 2026
**Tình trạng:** Đã triển khai (Deployed)
**Ngôn ngữ tài liệu:** Tiếng Việt (Vietnamese)

---

## CHƯƠNG I: TỔNG QUAN CHIẾN LƯỢC VÀ NGỮ CẢNH DỰ ÁN

### 1.1. Tầm nhìn dự án (Project Vision)
LuxManage Resort không chỉ là một ứng dụng quản lý khách sạn thông thường; nó là một hệ sinh thái được thiết kế để định nghĩa lại trải nghiệm du lịch số. Trong bối cảnh các Resort cao cấp cần sự chính xác tuyệt đối và tốc độ phục vụ nhanh chóng, LuxManage đóng vai trò là "trung tâm điều hành thông minh", kết nối mọi quy trình từ đặt phòng, thanh toán đến chăm sóc khách hàng tự động.

### 1.2. Mục tiêu kỹ thuật (Technical Goals)
*   **Zero-Overbooking:** Đảm bảo không bao giờ xảy ra tình trạng đặt trùng phòng thông qua thuật toán kiểm tra tính khả dụng (Availability) đa tầng.
*   **Instant Confirmation:** Khách hàng nhận được phản hồi ngay lập tức thông qua cơ chế Polling và Email Automation.
*   **High Performance:** Hệ thống được tối ưu hóa để có thể xử lý hàng ngàn lượt truy cập đồng thời mà không bị trễ (Latency < 2s).
*   **Seamless Integration:** Sẵn sàng kết nối với các cổng thanh toán quốc tế và hệ thống quản lý bên thứ ba (OTA).

---

## CHƯƠNG II: GIẢI MÃ CẤU TRÚC MÃ NGUỒA (PROJECT ARCHITECTURE)

Hệ thống được tổ chức theo mô hình Layered Architecture chuyên nghiệp, giúp việc bảo trì và mở rộng trở nên dễ dàng.

### 2.1. Phân tích Thư mục Dự án
*   **/Controllers:** Chứa các bộ xử lý điều hướng. 
    *   `CustomerController.cs`: Quản lý đăng ký, đăng nhập, hồ sơ và lịch sử khách hàng.
    *   `DashboardController.cs`: Trung tâm điều hành của Admin, quản lý các bảng thống kê và danh sách đặt phòng.
    *   `API/BookingController.cs`: Cung cấp các Endpoint cho ứng dụng di động và các tác vụ AJAX (sinh mã QR, check trạng thái đơn).
*   **/Models:** Chứa các định nghĩa về dữ liệu (Entities).
    *   `Booking.cs`: Định nghĩa mọi thông tin về đơn hàng.
    *   `Room.cs`: Định nghĩa cấu trúc phòng vật lý.
    *   `Customer.cs`: Định nghĩa thông tin người dùng.
*   **/Services:** Chứa các dịch vụ chạy ngầm.
    *   `BookingEmailWorker.cs`: Con robot giám sát database và gửi mail tự động.
*   **/Views:** Chứa giao diện người dùng (Razor Pages).
    *   `Home/Index.cshtml`: Trang chủ với modal đặt phòng và thanh trượt cọc linh hoạt.
    *   `Customer/Profile.cshtml`: Trang xem lịch sử đặt phòng của khách.
    *   `Shared/_AdminLayout.cshtml`: Giao diện Admin với hệ thống chuông báo tích hợp.

---

## CHƯƠNG III: CHI TIẾT CÁC LOGIC NGHIỆP VỤ ĐẶC THÙ

### 3.1. Logic sinh mã VietQR Dynamic
Đây là tính năng quan trọng nhất giúp khách hàng thanh toán thuận tiện.
*   **Quy trình:** Khi khách bấm nút "Đặt phòng", JavaScript sẽ gửi dữ liệu lên API. API tính toán số tiền cọc (ví dụ: 30% của 2.000.000đ = 600.000đ).
*   **Mã hóa:** Hệ thống sử dụng thư viện xử lý chuỗi để ghép các mã định danh ngân hàng (BIN), Số tài khoản và Số tiền thành một chuỗi văn bản theo chuẩn ISO/IEC 16022.
*   **Hiển thị:** Chuỗi này được thư viện `qrcode.js` chuyển thành hình ảnh QR để khách quét.

### 3.2. Logic "Người giám sát ngầm" (The Background Worker)
Robot gửi mail không chỉ đơn giản là gửi đi, nó có các cơ chế tự bảo vệ:
*   **Retry Logic:** Nếu server mail bị lỗi (ví dụ Google tạm khóa do gửi quá nhiều), Robot sẽ thử lại sau 1 phút thay vì bị treo hoàn toàn.
*   **Logging:** Mọi lần quét database đều được ghi lại trong nhật ký hệ thống để Admin theo dõi sức khỏe của Robot.

---

## CHƯƠNG IV: PHÂN TÍCH DATABASE CHUYÊN SÂU (SCHEMA V13)

### 4.1. Từ điển dữ liệu bảng Bookings
Bảng này đã được nâng cấp qua 13 phiên bản để tối ưu hóa:
*   `Id`: Khóa chính (Primary Key) - Đánh chỉ mục Index để tìm kiếm cực nhanh.
*   `IsDepositPaid`: Cờ trạng thái (Flag). Khi giá trị này là 1 (True), Robot gửi mail sẽ được kích hoạt.
*   `DepositPercentage`: Lưu lại tỷ lệ cọc khách chọn (20-40%) để phục vụ việc hoàn tiền nếu có hủy đơn.
*   `CreatedAt`: Giúp Admin biết được tốc độ ra quyết định của khách hàng và thời điểm "giờ vàng" khách hay đặt phòng.

### 4.2. Ràng buộc dữ liệu (Constraints)
Hệ thống thiết lập các ràng buộc cứng:
*   Ngày trả phòng (`CheckOut`) không bao giờ được phép trước hoặc bằng ngày nhận phòng (`CheckIn`).
*   Một phòng tại một thời điểm chỉ có tối đa 1 đơn hàng ở trạng thái `PAID` hoặc `CHECKED_IN`.

---

## CHƯƠNG V: TÀI LIỆU HƯỚNG DẪN API (EXTENDED API REFERENCE)

### 5.1. Authentication API
*   `POST /api/auth/login`: Xác thực người dùng và cấp Cookie.
*   `POST /api/auth/logout`: Hủy Cookie và đăng xuất.

### 5.2. Booking API
*   `GET /api/bookings/status/{id}`: Kiểm tra trạng thái thanh toán (Dùng cho Polling).
*   `GET /api/bookings/recent`: Lấy danh sách thông báo cho chuông báo Admin.
*   `POST /api/bookings/confirm-deposit/{id}`: Admin phê duyệt tiền cọc.

---

## CHƯƠNG VI: CẨM NANG BẢO TRÌ VÀ XỬ LÝ SỰ CỐ (SOP)

### 6.1. Hướng dẫn thay đổi thông tin Ngân hàng
Khi chủ Resort đổi tài khoản ngân hàng, chỉ cần vào file `Home/Index.cshtml` tìm đoạn mã sinh mã QR và thay đổi số tài khoản. Hệ thống sẽ tự động cập nhật cho toàn bộ khách hàng mới.

### 6.2. Xử lý khi khách báo "Đã chuyển tiền nhưng hệ thống chưa báo"
1.  Admin vào SQL Server (SSMS) mở bảng `Bookings`.
2.  Tìm đơn hàng theo tên khách hoặc Email.
3.  Sửa cột `IsDepositPaid` từ `False` sang `True`.
4.  Robot sẽ tự động gửi Mail xác nhận trong vòng 5 giây và khách sẽ thấy màn hình đổi màu xanh thành công.

---

## CHƯƠNG VII: CHI TIẾT GIAO DIỆN NGƯỜI DÙNG (UI/UX DESIGN)

### 7.1. Bảng màu chủ đạo (Color Palette)
*   **Gold (#D4AF37):** Sử dụng cho các nút hành động quan trọng, thể hiện sự đẳng cấp của Resort.
*   **Dark Background (#121212):** Nền tối sâu giúp làm nổi bật các hình ảnh phòng.
*   **Glass Panels:** Sử dụng `backdrop-filter: blur(10px)` tạo hiệu ứng kính mờ cho các Modal và Sidebar Admin.

### 7.2. Trải nghiệm người dùng (UX)
*   **Mobile Friendly:** Mọi trang từ Đặt phòng đến Admin đều co giãn (Responsive) mượt mà trên iPhone, Samsung và iPad.
*   **Micro-interactions:** Các nút bấm có hiệu ứng Hover nhẹ nhàng, các Modal xuất hiện với hiệu ứng trượt mượt mà.

---

## CHƯƠNG VIII: BẢO MẬT VÀ QUY TRÌNH XÁC THỰC (SECURITY DEEP DIVE)

### 8.1. Cơ chế Cookie Authentication
Hệ thống sử dụng ASP.NET Core Identity/Cookie Auth thay vì Session truyền thống.
*   **Tính năng:** Cookie được mã hóa và chỉ có server mới giải mã được.
*   **HttpOnly:** Ngăn chặn các cuộc tấn công XSS (JavaScript không thể đọc trộm Cookie).
*   **Role Mapping:** Khi người dùng đăng nhập, hệ thống gắn kèm "Claims" là `ADMIN` hoặc `CUSTOMER`. Toàn bộ các API nhạy cảm đều được bảo vệ bởi attribute `[Authorize(Roles = "ADMIN")]`.

### 8.2. Bảo mật mật khẩu
Mật khẩu khách hàng không bao giờ được lưu dưới dạng văn bản thuần túy. Hệ thống sử dụng thuật toán băm (Hashing) hiện đại. Dù database bị lộ, kẻ tấn công cũng không thể biết được mật khẩu thực của khách.

---

## CHƯƠNG IX: TỐI ƯU HÓA HIỆU NĂNG DATABASE (PERFORMANCE TUNING)

### 9.1. Đánh chỉ mục (Indexing Strategy)
Chúng tôi đã đánh chỉ mục cho các trường thường xuyên bị truy vấn:
*   `IX_Booking_Email`: Tăng tốc độ hiển thị lịch sử đặt phòng cho khách.
*   `IX_Booking_Status`: Tăng tốc độ cho Robot gửi mail khi lọc đơn chưa gửi.
*   `IX_Booking_DateRange`: Tăng tốc độ kiểm tra phòng trống khi khách đặt phòng.

### 9.2. Cơ chế Lazy Loading vs Eager Loading
Hệ thống sử dụng **Eager Loading** thông qua lệnh `.Include(b => b.Room)` để giảm thiểu số lượng truy vấn tới database (tránh lỗi N+1 query), giúp trang Admin load nhanh hơn gấp 5 lần.

---

## CHƯƠNG X: HƯỚNG DẪN CÀI ĐẶT MÔI TRƯỜNG PHÁT TRIỂN (DEV SETUP)

### 10.1. Yêu cầu hệ thống
*   **Phần mềm:** Visual Studio 2022 hoặc VS Code.
*   **SDK:** .NET 7.0 SDK.
*   **Database:** SQL Server 2019/2022 (Bản Express hoặc Developer).

### 10.2. Các bước khởi chạy dự án
1.  Mở Solution trong Visual Studio.
2.  Cập nhật chuỗi kết nối trong `appsettings.json`.
3.  Chạy lệnh `Update-Database` trong Package Manager Console để khởi tạo Database v13.
4.  Nhấn F5 để chạy dự án.

---

## CHƯƠNG XI: KỊCH BẢN KIỂM THỬ TOÀN DIỆN (TEST CASES)

| ID | Kịch bản kiểm thử | Kết quả mong đợi |
| :--- | :--- | :--- |
| **TC01** | Đặt phòng vào ngày đã có người ở | Hệ thống báo lỗi "Phòng đã kín". |
| **TC02** | Nhập sai mã OTP | Hệ thống báo "Mã không chính xác". |
| **TC03** | Xác nhận cọc trong Admin | Mail gửi về khách trong 5 giây. |
| **TC04** | Truy cập Admin bằng tài khoản khách | Bị đẩy ra trang Login. |

---

## CHƯƠNG XII: PHÂN TÍCH CHI TIẾT ROBOT GỬI MAIL (CODE DEEP DIVE)

Phần này dành cho các kỹ sư muốn hiểu rõ cách vận hành của `BookingEmailWorker.cs`.

### 12.1. Vòng lặp vô hạn (Infinite Loop)
Robot sử dụng `while (!stoppingToken.IsCancellationRequested)` để đảm bảo nó luôn chạy khi máy chủ đang hoạt động.
```csharp
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    while (!stoppingToken.IsCancellationRequested)
    {
        // Thực thi logic quét
        await Task.Delay(5000, stoppingToken);
    }
}
```

### 12.2. Xử lý đa luồng an toàn
Do database context là Scoped, robot phải tạo ra một vùng Scope mới mỗi lần quét để tránh lỗi "Context is disposed":
```csharp
using (var scope = _scopeFactory.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Truy vấn dữ liệu tại đây...
}
```

---

## CHƯƠNG XIII: CẤU HÌNH HỆ THỐNG EMAIL (MAIL ENGINE CONFIG)

### 13.1. Thiết lập Google App Password
Để hệ thống gửi được mail từ Gmail cá nhân, bạn phải:
1.  Bật **Bảo mật 2 lớp** cho tài khoản Google.
2.  Truy cập mục **App Passwords**.
3.  Chọn ứng dụng là "Thư" và thiết bị là "Máy tính Windows".
4.  Copy mã 16 ký tự và dán vào `appsettings.json`.

### 13.2. Email Template (Mẫu thư)
Hệ thống sử dụng HTML Inline CSS để đảm bảo Email hiển thị đẹp mắt trên cả Gmail Web và ứng dụng điện thoại. Các thẻ màu Gold (#D4AF37) được sử dụng đồng bộ với thương hiệu Resort.

---

## CHƯƠNG XIV: MA TRẬN PHÂN QUYỀN (PERMISSION MATRIX)

| Chức năng | Admin | Customer | Guest |
| :--- | :---: | :---: | :---: |
| Xem danh sách phòng | ✔ | ✔ | ✔ |
| Đặt phòng | ✔ | ✔ | ❌ |
| Xác nhận thanh toán | ✔ | ❌ | ❌ |
| Thay đổi trạng thái phòng | ✔ | ❌ | ❌ |
| Xem lịch sử cá nhân | ✔ | ✔ | ❌ |
| Quản lý nhân viên | ✔ | ❌ | ❌ |

---

## CHƯƠNG XV: CẨM NĂNG VẬN HÀNH CHO NHÂN VIÊN LỄ TÂN

### 15.1. Quy trình Check-in (Nhận phòng)
1.  Khi khách đến, lễ tân tìm đơn hàng trên Dashboard.
2.  Bấm nút **"Check-in"**.
3.  Hệ thống sẽ tự động cập nhật trạng thái phòng sang màu Đỏ (Occupied).
4.  Lúc này, khách không thể đặt phòng đó trên website nữa.

### 15.2. Quy trình Check-out (Trả phòng)
1.  Khách trả chìa khóa, lễ tân bấm **"Check-out"**.
2.  Hệ thống cập nhật trạng thái phòng sang màu Xám (Cleaning).
3.  Sau khi nhân viên buồng phòng báo đã dọn xong, Admin cập nhật về "Available" để đón khách tiếp theo.

---

## CHƯƠNG XVI: KẾ HOẠCH BẢO TRÌ VÀ SAO LƯU DỮ LIỆU

### 16.1. Sao lưu Database (Backup)
Khuyến nghị sao lưu database hàng ngày vào lúc 1:00 sáng.
Câu lệnh SQL: `BACKUP DATABASE ASP_DB_v13 TO DISK = 'D:\Backups\LuxManage_Daily.bak'`

### 16.2. Kiểm tra sức khỏe hệ thống (Health Check)
Admin nên định kỳ kiểm tra file Log của Robot gửi mail để đảm bảo không có lỗi kết nối SMTP nào xảy ra làm chậm trễ quá trình thông báo cho khách.

---

## CHƯƠNG XVII: TẦM NHÌN PHÁT TRIỂN PHIÊN BẢN 2.0 (THE FUTURE)

LuxManage 1.0 mới chỉ là điểm bắt đầu. Trong các phiên bản tiếp theo, chúng tôi sẽ tích hợp:
1.  **Hệ thống AI dự báo:** Tự động đề xuất tăng giá phòng vào các ngày lễ dựa trên dữ liệu lịch sử.
2.  **Tích hợp POS Nhà hàng:** Khách chỉ cần quét mã QR tại bàn ăn, tiền ăn sẽ tự động được cộng dồn vào hóa đơn phòng khi Check-out.
3.  **App Mobile dành cho Quản lý:** Nhận thông báo và xác nhận thanh toán ngay trên điện thoại thông qua thông báo đẩy (Push Notification).

---
**LuxManage Resort - Hệ thống quản lý nghỉ dưỡng hàng đầu cho kỷ nguyên số.**
*(Tài liệu chi tiết kỹ thuật chuyên sâu - Siêu tài liệu v1.0.0)*
