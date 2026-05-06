using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using TestAPIChallenges.Models; 
using TestAPIChallenges.Responses;
using TestAPIChallenges.DTOs;
using TestAPIChallenges.Services; // THÊM DÒNG NÀY: Để gọi EmailService


namespace TestAPIChallenges.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly CourseManagementContext _context;
        private readonly ILogger<AuthController> _logger;
        private readonly EmailService _emailService; // THÊM DÒNG NÀY

        // SỬA HÀM KHỞI TẠO: Bơm thêm EmailService vào
        public AuthController(CourseManagementContext context, ILogger<AuthController> logger, EmailService emailService)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
        }

        // 4. Đăng nhập (Login)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<object>.Error("Dữ liệu không hợp lệ"));

            try
            {
                var auth = await _context.UserAuths
                    .Include(a => a.User) 
                    .FirstOrDefaultAsync(a => a.Username == dto.Username);

                if (auth == null) return Unauthorized(ApiResponse<object>.Error("Tên đăng nhập hoặc mật khẩu không đúng"));

                bool isValid = BCrypt.Net.BCrypt.Verify(dto.Password, auth.PasswordHash);
                if (!isValid) return Unauthorized(ApiResponse<object>.Error("Tên đăng nhập hoặc mật khẩu không đúng"));

                // ==========================================
                // THÊM ĐOẠN NÀY: KIỂM TRA TÀI KHOẢN ĐÃ KÍCH HOẠT CHƯA
                // ==========================================
                if (auth.User.IsActive == false || auth.User.IsActive == null)
                {
                    return Unauthorized(ApiResponse<object>.Error("Tài khoản chưa được kích hoạt. Vui lòng kiểm tra email để xác thực mã OTP."));
                }

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
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);

                if (user == null) 
                {
                    return NotFound(ApiResponse<object>.Error("Không tìm thấy thông tin người dùng"));
                }

                var result = new 
                {
                    user.UserId,
                    user.FullName,
                    user.Email,
                    user.IsActive // Có thể trả thêm trạng thái này để FE biết
                };

                return Ok(ApiResponse<object>.Success(result, "Lấy thông tin cá nhân thành công!"));
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
            if (!ModelState.IsValid) 
                return BadRequest(ApiResponse<object>.Error("Dữ liệu đầu vào không hợp lệ"));

            var isEmailTaken = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            var isUsernameTaken = await _context.UserAuths.AnyAsync(a => a.Username == dto.Username);

            if (isEmailTaken || isUsernameTaken)
            {
                var errors = new List<string>();
                if (isEmailTaken) errors.Add("Email này đã được sử dụng.");
                if (isUsernameTaken) errors.Add("Tên đăng nhập này đã tồn tại.");
                return BadRequest(ApiResponse<object>.Error("Lỗi đăng ký", errors));
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                
                // ==========================================
                // THÊM: TẠO MÃ OTP 6 SỐ NGẪU NHIÊN
                // ==========================================
                string generatedOtp = new Random().Next(100000, 999999).ToString();

                var newUser = new User { 
                    FullName = dto.FullName, 
                    Email = dto.Email,
                    IsActive = false // Thêm trạng thái mặc định là chưa kích hoạt
                };
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync(); 

                var newAuth = new UserAuth { 
                    UserId = newUser.UserId, 
                    Username = dto.Username, 
                    PasswordHash = passwordHash,
                    OtpCode = generatedOtp, // Lưu OTP vào DB
                    OtpExpiry = DateTime.Now.AddMinutes(5) // Thời hạn 5 phút
                };
                _context.UserAuths.Add(newAuth);
                await _context.SaveChangesAsync();

                // Giao dịch DB thành công
                await transaction.CommitAsync();

                // ==========================================
                // THÊM: GỬI EMAIL CHỨA OTP
                // ==========================================
                await _emailService.SendOtpEmailAsync(dto.Email, generatedOtp);
                _logger.LogInformation($"Đã gửi OTP {generatedOtp} đến email {dto.Email}");

                return Ok(ApiResponse<object>.Success(null, "Đăng ký thành công! Vui lòng kiểm tra email để nhận mã OTP."));
            }
            catch (Exception ex) {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Lỗi nghiêm trọng trong quá trình đăng ký");
                return StatusCode(500, ApiResponse<object>.Error("Lỗi hệ thống, vui lòng thử lại sau"));
            }
        }

        // ==========================================
        // 7. XÁC THỰC OTP (HÀM MỚI HOÀN TOÀN)
        // ==========================================
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            try
            {
                // Tìm User theo email
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (user == null) 
                    return BadRequest(ApiResponse<object>.Error("Không tìm thấy người dùng với email này."));

                // Tìm thông tin Auth chứa OTP
                var auth = await _context.UserAuths.FirstOrDefaultAsync(a => a.UserId == user.UserId);
                if (auth == null) 
                    return BadRequest(ApiResponse<object>.Error("Lỗi dữ liệu xác thực."));

                // Kiểm tra OTP hợp lệ không
                var violations = new List<string>();
                if (auth.OtpCode != dto.OtpCode)
                {
                    violations.Add("Mã OTP không chính xác.");
                }
                else if (auth.OtpExpiry < DateTime.Now)
                {
                    violations.Add("Mã OTP đã hết hạn (quá 5 phút).");
                }

                if (violations.Any())
                {
                    return BadRequest(ApiResponse<object>.Error("Xác thực thất bại", violations));
                }

                // NẾU OTP ĐÚNG -> KÍCH HOẠT TÀI KHOẢN
                user.IsActive = true; 
                
                // Xóa mã OTP đi để không bị lợi dụng
                auth.OtpCode = null; 
                auth.OtpExpiry = null;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Tài khoản {user.Email} đã xác thực thành công.");
                return Ok(ApiResponse<object>.Success(null, "Xác thực tài khoản thành công! Bạn có thể đăng nhập."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xác thực OTP");
                return StatusCode(500, ApiResponse<object>.Error("Lỗi hệ thống khi xác thực"));
            }
        }
        
        // 8. Gửi lại mã OTP (send-otp)
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpDto dto)
        {
            try
            {
                // Kiểm tra email tồn tại
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (user == null)
                    return BadRequest(ApiResponse<object>.Error("Không tìm thấy tài khoản với email này."));

                // Nếu tài khoản đã kích hoạt rồi thì không cho gửi nữa
                if (user.IsActive == true)
                    return BadRequest(ApiResponse<object>.Error("Tài khoản này đã được kích hoạt."));

                var auth = await _context.UserAuths.FirstOrDefaultAsync(a => a.UserId == user.UserId);
                if (auth == null)
                    return BadRequest(ApiResponse<object>.Error("Lỗi dữ liệu hệ thống."));

                // Tạo OTP mới
                string newOtp = new Random().Next(100000, 999999).ToString();
                
                // Cập nhật vào DB
                auth.OtpCode = newOtp;
                auth.OtpExpiry = DateTime.Now.AddMinutes(5);
                await _context.SaveChangesAsync();

                // Gửi email
                await _emailService.SendOtpEmailAsync(dto.Email, newOtp);
                
                // Logging
                _logger.LogInformation("Đã gửi lại mã OTP {Otp} cho email {Email}", newOtp, dto.Email);

                return Ok(ApiResponse<object>.Success(null, "Đã gửi lại mã OTP thành công. Vui lòng kiểm tra email."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi lại mã OTP cho email {Email}", dto.Email);
                return StatusCode(500, ApiResponse<object>.Error("Lỗi hệ thống khi gửi email."));
            }
        }
    }
}