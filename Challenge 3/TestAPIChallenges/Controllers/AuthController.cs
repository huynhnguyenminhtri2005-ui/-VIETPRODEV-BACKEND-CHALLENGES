using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
// Kiểm tra 2 dòng dưới đây xem đã đúng tên thư mục chưa
using TestAPIChallenges.Models; 
using TestAPIChallenges.Responses;
using TestAPIChallenges.DTOs;


namespace TestAPIChallenges.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly CourseManagementContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(CourseManagementContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }
        // 4. Đăng nhập (Login)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // 1. Kiểm tra Validator
            if (!ModelState.IsValid) return BadRequest(ApiResponse<object>.Error("Dữ liệu không hợp lệ"));

            try
            {
                // 2. Tìm thông tin Auth và kèm theo thông tin User
                var auth = await _context.UserAuths
                    .Include(a => a.User) 
                    .FirstOrDefaultAsync(a => a.Username == dto.Username);

                // Nếu không thấy username trong bảng User_Auth
                if (auth == null) return Unauthorized(ApiResponse<object>.Error("Tên đăng nhập hoặc mật khẩu không đúng"));

                // 3. Kiểm tra mật khẩu (So sánh mật khẩu nhập vào với mật khẩu đã Hash trong DB)
                bool isValid = BCrypt.Net.BCrypt.Verify(dto.Password, auth.PasswordHash);
                
                if (!isValid) return Unauthorized(ApiResponse<object>.Error("Tên đăng nhập hoặc mật khẩu không đúng"));

                // 4. Trả về thông tin cơ bản khi thành công (Sau này sẽ trả về Token JWT ở đây)
                var result = new {
                    auth.User.UserId,
                    auth.User.FullName,
                    auth.Username
                };

                return Ok(ApiResponse<object>.Success(result, "Đăng nhập thành công!"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi đăng nhập");
                return StatusCode(500, ApiResponse<object>.Error("Lỗi hệ thống"));
            }
        }
        // 5. Lấy thông tin cá nhân (Profile)
        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetProfile(int id)
        {
            try
            {
                // Tìm User trong Database, có thể kèm theo Role nếu cần
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == id);

                // Nếu không tìm thấy User
                if (user == null) 
                {
                    return NotFound(ApiResponse<object>.Error("Không tìm thấy thông tin người dùng"));
                }

                // Trả về dữ liệu sạch (không bao gồm mật khẩu hay thông tin nhạy cảm)
                var result = new 
                {
                    user.UserId,
                    user.FullName,
                    user.Email
                };

                return Ok(ApiResponse<object>.Success(result, "Lấy thông personal thành công!"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy Profile của User ID: {Id}", id);
                return StatusCode(500, ApiResponse<object>.Error("Lỗi hệ thống khi tải profile"));
            }
        }
        // 6. Đăng ký (Register)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // 1. Validator: Kiểm tra định dạng (Email, độ dài mật khẩu...) dựa trên RegisterDto
            if (!ModelState.IsValid) 
                return BadRequest(ApiResponse<object>.Error("Dữ liệu đầu vào không hợp lệ"));

            // 2. Kiểm tra nghiệp vụ: Email hoặc Username đã tồn tại chưa?
            var isEmailTaken = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            var isUsernameTaken = await _context.UserAuths.AnyAsync(a => a.Username == dto.Username);

            if (isEmailTaken || isUsernameTaken)
            {
                var errors = new List<string>();
                if (isEmailTaken) errors.Add("Email này đã được sử dụng.");
                if (isUsernameTaken) errors.Add("Tên đăng nhập này đã tồn tại.");
                
                return BadRequest(ApiResponse<object>.Error("Lỗi đăng ký", errors));
            }

            // Dùng Transaction để đảm bảo an toàn dữ liệu cho cả 2 bảng
            using var transaction = await _context.Database.BeginTransactionAsync();

            try {
                // 3. Hash mật khẩu bằng BCrypt
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                // 4. Lưu vào bảng Users
                var newUser = new User { 
                    FullName = dto.FullName, 
                    Email = dto.Email 
                };
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync(); 

                // 5. Lưu vào bảng UserAuths (Dùng UserId vừa sinh ra từ bước 4)
                var newAuth = new UserAuth { 
                    UserId = newUser.UserId, 
                    Username = dto.Username, 
                    PasswordHash = passwordHash 
                };
                _context.UserAuths.Add(newAuth);
                await _context.SaveChangesAsync();

                // Hoàn tất giao dịch
                await transaction.CommitAsync();

                return Ok(ApiResponse<object>.Success(null, "Đăng ký tài khoản thành công!"));
            }
            catch (Exception ex) {
                // Nếu có lỗi, hủy bỏ mọi thay đổi đã làm trong DB
                await transaction.RollbackAsync();
                
                _logger.LogError(ex, "Lỗi nghiêm trọng trong quá trình đăng ký");
                return StatusCode(500, ApiResponse<object>.Error("Lỗi hệ thống, vui lòng thử lại sau"));
            }
        }
    }
}