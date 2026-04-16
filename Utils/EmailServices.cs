using MailKit.Net.Smtp;
using MimeKit;

public class EmailServices(IConfiguration config)
{
    private async Task SendAsync(string toEmail, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(config["Email:Username"]));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") {Text = body};
        
        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(config["Email:Host"], int.Parse(config["Email:Port"]!), MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(config["Email:Username"], config["Email:Password"]);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }

    public async Task SendVerificationEmailAsync(string toEmail, string code)
    {
        await SendAsync(toEmail, "Xác thực email của bạn",
            $"<h2>Mã xác thực của bạn là: <strong>{code}</strong> </h2>" +
            $"<p>Mã này có hiệu lực trong 24 giờ</p>");
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string fullname)
    {
        await SendAsync(toEmail, "Chào mừng bạn", 
            $"<h2>Xin chào {fullname}!</h2>" +
                  $"<p>Tài khoản của bạn đã được xác thực thành công</p>" );
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
    {
        await SendAsync(toEmail, "Reset mật khẩu",
            $"<h2>Reset mật khẩu</h2>" +
            $"<p>Click vào link bên dưới để reset mật khẩu:</p>" +
            $"<a href='{resetLink}'>{resetLink}</a>" +
            $"<p>Link có hiệu lực trong 1 giờ.</p>");
    }
    
    public async Task SendResetSuccessEmailAsync(string toEmail)
    {
        await SendAsync(toEmail, "Mật khẩu đã được reset",
            "<h2>Mật khẩu của bạn đã được reset thành công!</h2>");
    }
    
        
}