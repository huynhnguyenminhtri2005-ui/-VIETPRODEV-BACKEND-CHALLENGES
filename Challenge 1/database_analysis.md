# Phân tích Cơ sở Dữ liệu Hệ thống Quản lý Đào tạo

## 1. Mục tiêu thiết kế
Hệ thống được thiết kế để quản lý thông tin người dùng, quyền hạn, các khóa học và quy trình đăng ký lớp học của học viên. Hệ thống đảm bảo tính bảo mật và tính toàn vẹn dữ liệu cao.

## 2. Các thực thể chính (Entities)
Dựa trên cấu trúc SQL, hệ thống bao gồm các nhóm thực thể sau:

### Nhóm Quản lý Người dùng & Bảo mật
- **Roles**: Lưu trữ danh mục các vai trò (Ví dụ: Admin, Teacher, Student).
- **Users**: Lưu trữ thông tin định danh cơ bản của một cá nhân.
- **User_Auth**: Lưu trữ thông tin nhạy cảm phục vụ đăng nhập. 
    * *Logic:* Tách riêng khỏi bảng `Users` để tăng tính bảo mật và dễ dàng mở rộng các phương thức đăng nhập (OAuth, OTP) sau này.

### Nhóm Quản lý Đào tạo
- **Courses**: Danh mục các môn học và số tín chỉ tương ứng.
- **Classes**: Các lớp học cụ thể được mở từ một môn học (có mã lớp và thời gian riêng).
- **Enrollments**: Ghi lại lịch sử đăng ký của người dùng vào các lớp học.

## 3. Phân tích quan hệ (Relationships)

| Cặp bảng | Loại quan hệ | Mô tả |
| :--- | :--- | :--- |
| **Roles - Users** | 1 : N | Một vai trò có thể được gán cho nhiều người dùng. |
| **Users - User_Auth** | 1 : 1 | Mỗi người dùng chỉ có duy nhất một bộ thông tin đăng nhập. |
| **Courses - Classes** | 1 : N | Một khóa học có thể mở nhiều lớp học khác nhau. |
| **Users - Classes** | N : N | Một người dùng có thể học nhiều lớp, một lớp có nhiều người dùng. Quan hệ này được giải quyết qua bảng trung gian **Enrollments**. |



## 4. Các quy tắc toàn vẹn dữ liệu (Data Integrity)
1. **Bảo mật:** Bảng `User_Auth` sử dụng `ON DELETE CASCADE`, nghĩa là khi xóa một User, thông tin đăng nhập liên quan sẽ tự động bị xóa bỏ để tránh dữ liệu rác.
2. **Chống trùng lặp:** * `email` và `username` là duy nhất (`UNIQUE`).
    * Ràng buộc `unique_enrollment` trong bảng `Enrollments` đảm bảo một sinh viên không thể đăng ký vào cùng một lớp học quá một lần.
3. **Kiểm tra điều kiện:** Tín chỉ (`credits`) trong bảng `Courses` luôn phải lớn hơn 0.

## 5. Tối ưu hóa (Optimization)
Hệ thống đã được thiết lập Index trên các cột thường xuyên dùng để tìm kiếm/đăng nhập:
- `idx_user_email` trên bảng `Users`.
- `idx_auth_username` trên bảng `User_Auth`.

##
Cột role_name được thiết lập UNIQUE để đảm bảo không có hai vai trò trùng tên nhau. Điều này ngăn chặn việc hệ thống bị rối loạn khi có hai nhóm quyền cùng tên "Admin". Khi vi phạm, hệ thống sẽ trả về lỗi SQL state: 23505