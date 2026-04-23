using System.ComponentModel.DataAnnotations;

namespace TestAPIChallenges.DTOs
{
    public class CreateCourseDto
    {
        [Required(ErrorMessage = "Tên khóa học là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên khóa học không quá 100 ký tự")]
        public string CourseName { get; set; }

        public string Description { get; set; }
    }
}