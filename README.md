CHALLENGE 1: THIẾT KẾ DATABASE
- Entity: Users, User_Auth, Roles, Courses, Classes, Enrollments và các bảng phụ nếu cần.
- Tạo init.sql: script tạo bảng, khóa chính/ngoại, ràng buộc, index,...
- Vẽ sơ đồ ERD (erd.png) bằng Draw.io
- Tài liệu database_analysis.md: mô tả database
- Dữ liệu mẫu (sample_data.sql): ~10 bản ghi/bảng
- Chuẩn hóa 3NF, toàn vẹn dữ liệu
- Tạo dự án Back-End, kết nối Database, cấu hình tự động sinh Entities/Models từ Database cho dự án. (.NET: scaffold, Node: sequelize-auto)


CHALLENGE 2: CRUD KHÓA HỌC & LỚP HỌC
- Entity: Courses, Classes
- API Courses: create, read, update, delete
- API Classes: create, read, update, delete
- Validator
- Try-catch: xử lý lỗi
- Response: chuẩn JSON, status (success/error), violations cho lỗi
- Logging: log API calls, errors

CHALLENGE 3: ĐĂNG KÝ & ĐĂNG NHẬP
- API: register, login, profile (GET)
- Register: lưu email, name, password_hash vào Users/User_Auth
- Bảo mật: hash mật khẩu (BCrypt: .NET BCrypt.Net, Node bcrypt), JWT (access token 1 giờ)
- Validator: email hợp lệ, password tối thiểu 6 ký tự
- Response: chuẩn JSON, violations cho lỗi (email trùng, password yếu)
- Try-catch: xử lý lỗi đăng ký/đăng nhập
- Logging: log đăng ký/đăng nhập thất bại

CHALLENGE 4: GỬI OTP XÁC THỰC
- API: send-otp (sau register), verify-otp
- OTP: 6 chữ số, lưu trong User_Auth (otp_code, otp_expiry: 5 phút), gửi qua email (SendGrid)
- Quy trình: register → gửi OTP → verify OTP → kích hoạt tài khoản
- Validator: kiểm tra email tồn tại, OTP hợp lệ
- Response: chuẩn JSON, violations (OTP hết hạn, sai mã)
- Try-catch: xử lý lỗi gửi email, OTP không hợp lệ
- Logging: log gửi OTP, xác thực OTP
- Tạo email service (SendGrid: .NET MailKit, Node nodemailer), mẫu HTML đơn giản
>>>>>>> main
