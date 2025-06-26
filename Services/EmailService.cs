using MailKit.Net.Smtp;
using MimeKit;



namespace ESService.Services
{
    public class EmailService
    {
            private readonly string smtpServer = "smtp.gmail.com";
            private readonly int smtpPort = 587;
            private readonly string senderEmail = "keerthiswarankrk@gmail.com";
            private readonly string senderPassword = "iple pkrv uici gmwt"; // Not your real password!

            public async Task SendEmailAsync(string to, string subject, string body)
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(senderEmail));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart("plain") { Text = body };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(senderEmail, senderPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
    }
}
