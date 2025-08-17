using System.Net.Mail;
using System.Net;
using Company.Demo.PL.Models;
using Microsoft.Extensions.Options;

namespace Company.Demo.PL.Services
{
    public interface IEmailService
    {
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink, string userName);
        Task<bool> SendEmailAsync(string toEmail, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink, string userName)
        {
            var subject = "استعادة كلمة المرور - Company.Demo.PL";
            var body = GeneratePasswordResetEmailBody(resetLink, userName);
            
            return await SendEmailAsync(toEmail, subject, body);
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword)
                };

                var message = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                message.To.Add(toEmail);

                await client.SendMailAsync(message);
                
                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                return false;
            }
        }

        private string GeneratePasswordResetEmailBody(string resetLink, string userName)
        {
            return $@"
                <!DOCTYPE html>
                <html dir='rtl' lang='ar'>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>استعادة كلمة المرور</title>
                    <style>
                        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
                        .button {{ display: inline-block; background: #007bff; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                        .button:hover {{ background: #0056b3; }}
                        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 14px; }}
                        .warning {{ background: #fff3cd; border: 1px solid #ffeaa7; color: #856404; padding: 15px; border-radius: 5px; margin: 20px 0; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>🔐 استعادة كلمة المرور</h1>
                            <p>مرحباً {userName}</p>
                        </div>
                        
                        <div class='content'>
                            <p>لقد تلقينا طلباً لاستعادة كلمة المرور الخاصة بحسابك.</p>
                            
                            <p>إذا كنت أنت من طلب هذا، يرجى النقر على الزر أدناه لإنشاء كلمة مرور جديدة:</p>
                            
                            <div style='text-align: center;'>
                                <a href='{resetLink}' class='button'>إنشاء كلمة مرور جديدة</a>
                            </div>
                            
                            <div class='warning'>
                                <strong>⚠️ تنبيه مهم:</strong>
                                <ul style='margin: 10px 0; padding-right: 20px;'>
                                    <li>هذا الرابط صالح لمدة ساعة واحدة فقط</li>
                                    <li>إذا لم تطلب استعادة كلمة المرور، يمكنك تجاهل هذا البريد</li>
                                    <li>لن يتمكن أي شخص آخر من الوصول لحسابك من خلال هذا الرابط</li>
                                </ul>
                            </div>
                            
                            <p>إذا لم يعمل الزر، يمكنك نسخ الرابط التالي ولصقه في المتصفح:</p>
                            <p style='word-break: break-all; background: #e9ecef; padding: 10px; border-radius: 5px;'>{resetLink}</p>
                        </div>
                        
                        <div class='footer'>
                            <p>هذا البريد تم إرساله تلقائياً من نظام Company.Demo.PL</p>
                            <p>يرجى عدم الرد على هذا البريد</p>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}
