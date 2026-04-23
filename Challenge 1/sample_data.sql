-- 1. Thêm Roles (Nếu chưa có)
INSERT INTO Roles (role_name) VALUES 
('Admin'), ('Teacher'), ('Student')
ON CONFLICT (role_name) DO NOTHING;

-- 2. Thêm Users (10 bản ghi - Dùng email để check trùng)
INSERT INTO Users (full_name, email, role_id) VALUES 
('Huỳnh Nguyễn Minh Trí', 'minhtri@gmail.com', 1),
('Nguyễn Văn Giáo Viên', 'gv_a@school.edu.vn', 2),
('Trần Thị Giảng Viên', 'gv_b@school.edu.vn', 2),
('Lê Văn Sinh Viên', 'sv_c@student.com', 3),
('Phạm Thị Hoa', 'hoa.pham@student.com', 3),
('Nguyễn Hoàng Nam', 'nam.hoang@student.com', 3),
('Trần Bảo Ngọc', 'ngoc.tran@student.com', 3),
('Lý Gia Hân', 'han.ly@student.com', 3),
('Võ Minh Quân', 'quan.vo@student.com', 3),
('Đặng Thu Thảo', 'thao.dang@student.com', 3)
ON CONFLICT (email) DO NOTHING;

-- 3. Thêm User_Auth (Sử dụng Username để check trùng)
-- Password mẫu là 'hash_123456' cho tất cả
INSERT INTO User_Auth (user_id, username, password_hash) VALUES 
((SELECT user_id FROM Users WHERE email = 'minhtri@gmail.com'), 'minhtri_admin', 'hash_123456'),
((SELECT user_id FROM Users WHERE email = 'gv_a@school.edu.vn'), 'teacher_a', 'hash_123456'),
((SELECT user_id FROM Users WHERE email = 'gv_b@school.edu.vn'), 'teacher_b', 'hash_123456'),
((SELECT user_id FROM Users WHERE email = 'sv_c@student.com'), 'tu_student', 'hash_123456'),
((SELECT user_id FROM Users WHERE email = 'hoa.pham@student.com'), 'hoa_pham', 'hash_123456'),
((SELECT user_id FROM Users WHERE email = 'nam.hoang@student.com'), 'nam_hoang', 'hash_123456'),
((SELECT user_id FROM Users WHERE email = 'ngoc.tran@student.com'), 'ngoc_tran', 'hash_123456'),
((SELECT user_id FROM Users WHERE email = 'han.ly@student.com'), 'han_ly', 'hash_123456'),
((SELECT user_id FROM Users WHERE email = 'quan.vo@student.com'), 'quan_vo', 'hash_123456'),
((SELECT user_id FROM Users WHERE email = 'thao.dang@student.com'), 'thao_dang', 'hash_123456')
ON CONFLICT (username) DO NOTHING;

-- 4. Thêm Courses (10 môn học)
INSERT INTO Courses (course_name, credits, description) VALUES 
('Lập trình .NET Core', 3, 'Backend Core cơ bản'),
('Cấu trúc dữ liệu', 3, 'Giải thuật cơ bản'),
('Cơ sở dữ liệu SQL', 3, 'PostgreSQL & SQL Server'),
('Lập trình ReactJS', 4, 'Frontend cơ bản'),
('Thiết kế hệ thống', 3, 'System Design'),
('Lập trình Java', 3, null),
('An toàn thông tin', 2, null),
('Trí tuệ nhân tạo', 4, null),
('Phát triển Mobile', 3, 'Flutter/React Native'),
('Điện toán đám mây', 3, 'AWS/Azure')
ON CONFLICT DO NOTHING;

-- 5. Thêm Classes (Dùng Subquery để tự tìm course_id theo tên môn học)
INSERT INTO Classes (course_id, class_code, start_date) VALUES 
((SELECT course_id FROM Courses WHERE course_name = 'Lập trình .NET Core' LIMIT 1), 'DOTNET_K01', '2026-05-01'),
((SELECT course_id FROM Courses WHERE course_name = 'Lập trình .NET Core' LIMIT 1), 'DOTNET_K02', '2026-06-01'),
((SELECT course_id FROM Courses WHERE course_name = 'Cấu trúc dữ liệu' LIMIT 1), 'DSA_01', '2026-05-15'),
((SELECT course_id FROM Courses WHERE course_name = 'Cơ sở dữ liệu SQL' LIMIT 1), 'SQL_01', '2026-05-20'),
((SELECT course_id FROM Courses WHERE course_name = 'Lập trình ReactJS' LIMIT 1), 'REACT_01', '2026-06-10'),
((SELECT course_id FROM Courses WHERE course_name = 'Thiết kế hệ thống' LIMIT 1), 'DESIGN_01', '2026-07-01'),
((SELECT course_id FROM Courses WHERE course_name = 'Lập trình Java' LIMIT 1), 'JAVA_01', '2026-05-05'),
((SELECT course_id FROM Courses WHERE course_name = 'An toàn thông tin' LIMIT 1), 'SEC_01', '2026-08-15'),
((SELECT course_id FROM Courses WHERE course_name = 'Trí tuệ nhân tạo' LIMIT 1), 'AI_01', '2026-09-01'),
((SELECT course_id FROM Courses WHERE course_name = 'Phát triển Mobile' LIMIT 1), 'MOB_01', '2026-10-01')
ON CONFLICT (class_code) DO NOTHING;

-- 6. Thêm Enrollments (Đăng ký học mẫu)
INSERT INTO Enrollments (user_id, class_id, status) VALUES 
(4, 1, 'Active'), (5, 1, 'Active'), (6, 1, 'Active'),
(7, 2, 'Active'), (8, 3, 'Active'), (9, 4, 'Active'),
(10, 5, 'Active'), (4, 6, 'Active'), (5, 7, 'Active'), (6, 8, 'Active')
ON CONFLICT ON CONSTRAINT unique_enrollment DO NOTHING;

--
{

"email": "tri@example.com",

"fullName": "Huynh Nguyen Minh Tri",

"password": "password123",

"username": "minhtri_dev"

}
SELECT * FROM Users WHERE Email = 'tri@example.com';