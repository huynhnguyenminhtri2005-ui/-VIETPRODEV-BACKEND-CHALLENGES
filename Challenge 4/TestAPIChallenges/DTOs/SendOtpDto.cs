using System.ComponentModel.DataAnnotations;

namespace TestAPIChallenges.DTOs
{
    public class SendOtpDto
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = string.Empty;
    }
}