using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAPIChallenges.Models;
using TestAPIChallenges.DTOs;
using TestAPIChallenges.Responses;

namespace TestAPIChallenges.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly CourseManagementContext _context;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(CourseManagementContext context, ILogger<CoursesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // 1. GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Lấy danh sách khóa học");
            var data = await _context.Courses.ToListAsync();
            return Ok(ApiResponse<List<Course>>.Success(data));
        }

        // 2. CREATE (POST)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourseDto dto)
        {
            // Tự động kiểm tra [Required], [StringLength] ở DTO
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<object>.Error("Dữ liệu không hợp lệ", errors));
            }

            try
            {
                var course = new Course { CourseName = dto.CourseName, Description = dto.Description };
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                
                return CreatedAtAction(nameof(GetAll), ApiResponse<Course>.Success(course, "Tạo thành công"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tạo khóa học");
                return StatusCode(500, ApiResponse<object>.Error("Lỗi hệ thống"));
            }
        }

        // 3. UPDATE (PUT)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateCourseDto dto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound(ApiResponse<object>.Error("Không tìm thấy"));

            try
            {
                course.CourseName = dto.CourseName;
                course.Description = dto.Description;
                await _context.SaveChangesAsync();
                return Ok(ApiResponse<Course>.Success(course, "Cập nhật thành công"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật");
                return StatusCode(500, ApiResponse<object>.Error("Lỗi hệ thống"));
            }
        }

        // 4. DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound(ApiResponse<object>.Error("Không tìm thấy"));

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return Ok(ApiResponse<object>.Success(null, "Xóa thành công"));
        }
    }
}