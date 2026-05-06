using System.ComponentModel.DataAnnotations;

namespace TestAPIChallenges.DTOs
{
    public class VerifyOtpDto
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mã OTP là bắt buộc")]
        public string OtpCode { get; set; } = string.Empty;
    }
}