using System.ComponentModel.DataAnnotations;

namespace TestAPIChallenges.DTOs
{
    public class ClassDto
    {
        [Required(ErrorMessage = "Mã lớp là bắt buộc")]
        public string ClassCode { get; set; }

        [Required(ErrorMessage = "Phải chọn khóa học")]
        public int CourseId { get; set; }

        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}