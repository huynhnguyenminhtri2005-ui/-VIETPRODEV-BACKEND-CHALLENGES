CHALLENGE 3: ĐĂNG KÝ & ĐĂNG NHẬP
- API: register, login, profile (GET)
- Register: lưu email, name, password_hash vào Users/User_Auth
- Bảo mật: hash mật khẩu (BCrypt: .NET BCrypt.Net, Node bcrypt), JWT (access token 1 giờ)
- Validator: email hợp lệ, password tối thiểu 6 ký tự
- Response: chuẩn JSON, violations cho lỗi (email trùng, password yếu)
- Try-catch: xử lý lỗi đăng ký/đăng nhập
- Logging: log đăng ký/đăng nhập thất bại
