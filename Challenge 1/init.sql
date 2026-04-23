-- 1. Bảng Roles
CREATE TABLE Roles (
    role_id SERIAL PRIMARY KEY,
    role_name VARCHAR(50) UNIQUE NOT NULL
);

-- 2. Bảng Users
CREATE TABLE Users (
    user_id SERIAL PRIMARY KEY,
    full_name VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    phone VARCHAR(20),
    role_id INT REFERENCES Roles(role_id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 3. Bảng User_Auth (Bảo mật: Tách thông tin đăng nhập)
CREATE TABLE User_Auth (
    auth_id SERIAL PRIMARY KEY,
    user_id INT UNIQUE REFERENCES Users(user_id) ON DELETE CASCADE,
    username VARCHAR(50) UNIQUE NOT NULL,
    password_hash TEXT NOT NULL,
    last_login TIMESTAMP
);

-- 4. Bảng Courses
CREATE TABLE Courses (
    course_id SERIAL PRIMARY KEY,
    course_name VARCHAR(200) NOT NULL,
    description TEXT,
    credits INT CHECK (credits > 0)
);

-- 5. Bảng Classes
CREATE TABLE Classes (
    class_id SERIAL PRIMARY KEY,
    course_id INT REFERENCES Courses(course_id) ON DELETE CASCADE,
    class_code VARCHAR(50) UNIQUE NOT NULL,
    start_date DATE,
    end_date DATE
);

-- 6. Bảng Enrollments (N-N giữa Users và Classes)
CREATE TABLE Enrollments (
    enrollment_id SERIAL PRIMARY KEY,
    user_id INT REFERENCES Users(user_id),
    class_id INT REFERENCES Classes(class_id),
    enroll_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    status VARCHAR(20) DEFAULT 'Active',
    CONSTRAINT unique_enrollment UNIQUE(user_id, class_id)
);

-- Index để tăng tốc truy vấn email và username
CREATE INDEX idx_user_email ON Users(email);
CREATE INDEX idx_auth_username ON User_Auth(username);
