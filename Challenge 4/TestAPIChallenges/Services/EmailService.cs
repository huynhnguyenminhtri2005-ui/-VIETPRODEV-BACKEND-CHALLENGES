using MailKit.Net.Smtp;
using MimeKit;

namespace TestAPIChallenges.Services
{
    public class EmailService
    {
        public async Task SendOtpEmailAsync(string toEmail, string otpCode)
        {
            var email = new MimeMessage();
            
            // 1. Khai báo người gửi (Chính là Gmail thật của bạn)
            email.From.Add(new MailboxAddress("Hệ thống Khóa Học", "huynhnguyenminhtri2005@gmail.com")); 
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = "Mã xác thực OTP của hệ thống Khóa Học"; //Mã OPT của bạn đây nè, nhớ nhập vào ô OTP nhé!

            // 2. Giao diện Email đơn giản, lịch sự
            string htmlTemplate = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 10px; max-width: 500px; margin: auto;'>
                <h2 style='color: #198754; text-align: center;'>Kích Hoạt Tài Khoản</h2>
                <p>Chào bạn,</p>
                <p>Hệ thống đã nhận được yêu cầu đăng ký tài khoản của bạn. Vui lòng nhập mã OTP dưới đây để hoàn tất kích hoạt:</p>
                <div style='text-align: center; margin: 20px 0;'>
                    <span style='font-size: 28px; font-weight: bold; background: #e9ecef; padding: 10px 20px; border-radius: 5px; letter-spacing: 8px; color: #dc3545;'>{otpCode}</span>
                </div>
                <p style='color: #6c757d; font-size: 14px; text-align: center;'>Mã OTP này có hiệu lực trong vòng <strong>5 phút</strong>.</p>
            </div>";

            email.Body = new BodyBuilder { HtmlBody = htmlTemplate }.ToMessageBody();

            try 
            {
                using var smtp = new SmtpClient();
                
                // 3. Kết nối đến trạm phát sóng của Google
                await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                
                // 4. ĐĂNG NHẬP (Thay 16 ký tự của bạn vào đây, nhớ xóa khoảng trắng nếu có)
                await smtp.AuthenticateAsync("huynhnguyenminhtri2005@gmail.com", "egbthhgchfvpxsbw"); 
                
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Nếu bị lỗi, nó sẽ in ra Terminal để bạn biết
                Console.WriteLine("Lỗi gửi mail qua Gmail: " + ex.Message);
            }
        }
    }
}