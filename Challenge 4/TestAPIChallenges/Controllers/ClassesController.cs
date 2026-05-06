using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAPIChallenges.Models;
using TestAPIChallenges.DTOs;
using TestAPIChallenges.Responses;

namespace TestAPIChallenges.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassesController : ControllerBase
    {
        private readonly CourseManagementContext _context;
        private readonly ILogger<ClassesController> _logger;

        public ClassesController(CourseManagementContext context, ILogger<ClassesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 1. Lấy danh sách lớp học (Kèm thông tin Khóa học)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Classes
                .Include(c => c.Course) // Load thêm thông tin khóa học liên kết
                .ToListAsync();
            return Ok(ApiResponse<List<Class>>.Success(data));
        }

        // 2. Lấy chi tiết 1 lớp học
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.Classes.Include(c => c.Course).FirstOrDefaultAsync(c => c.ClassId == id);
            if (item == null) return NotFound(ApiResponse<object>.Error("Không tìm thấy lớp học"));
            return Ok(ApiResponse<Class>.Success(item));
        }

        // 3. Tạo mới lớp học (Có kiểm tra CourseId)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClassDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<object>.Error("Dữ liệu lỗi", null));

            // Kiểm tra xem CourseId có tồn tại không trước khi tạo lớp
            var course = await _context.Courses.AnyAsync(c => c.CourseId == dto.CourseId);
            if (!course) return BadRequest(ApiResponse<object>.Error("Khóa học (CourseId) không tồn tại!"));

            try
            {
                var newClass = new Class
                {
                    ClassCode = dto.ClassCode,
                    CourseId = dto.CourseId,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate
                };

                _context.Classes.Add(newClass);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetById), new { id = newClass.ClassId }, ApiResponse<Class>.Success(newClass, "Tạo lớp thành công"));
            }
            catch (Exception ex)
            {
                // 1. Ghi log chi tiết cho bạn (Developer) xem ở Terminal
                _logger.LogError("--- PHÁT HIỆN LỖI KHI TẠO LỚP ---");
                _logger.LogError("Thông báo: {Message}", ex.Message);
                
                // Nếu có lỗi từ Database trả về, nó sẽ nằm ở đây
                if (ex.InnerException != null) 
                {
                    _logger.LogError("Lỗi gốc từ Database: {Inner}", ex.InnerException.Message);
                }

                _logger.LogError("Vị trí lỗi (StackTrace): {Stack}", ex.StackTrace);

                // 2. Trả về cho người dùng (Kín đáo để bảo mật)
                return StatusCode(500, ApiResponse<object>.Error("Hệ thống gặp sự cố khi lưu dữ liệu."));
            }
        }

        // 4. Cập nhật lớp học
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ClassDto dto)
        {
            var item = await _context.Classes.FindAsync(id);
            if (item == null) return NotFound(ApiResponse<object>.Error("Không tìm thấy lớp học"));

            // Kiểm tra CourseId mới có hợp lệ không nếu người dùng thay đổi khóa học của lớp
            var course = await _context.Courses.AnyAsync(c => c.CourseId == dto.CourseId);
            if (!course) return BadRequest(ApiResponse<object>.Error("Khóa học mới không hợp lệ!"));

            item.ClassCode = dto.ClassCode;
            item.CourseId = dto.CourseId;
            item.StartDate = dto.StartDate;
            item.EndDate = dto.EndDate;

            await _context.SaveChangesAsync();
            return Ok(ApiResponse<Class>.Success(item, "Cập nhật thành công"));
        }

        // 5. Xóa lớp học
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Classes.FindAsync(id);
            if (item == null) return NotFound(ApiResponse<object>.Error("Không tìm thấy"));

            _context.Classes.Remove(item);
            await _context.SaveChangesAsync();
            return Ok(ApiResponse<object>.Success(null, "Đã xóa lớp học"));
        }
    }
}