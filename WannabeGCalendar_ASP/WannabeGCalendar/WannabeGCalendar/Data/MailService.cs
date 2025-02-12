using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Razor.TagHelpers;
using MimeKit;
using MimeKit.Text;
using Microsoft.Extensions.Options;
using WannabeGCalendar.Configuration;
using WannabeGCalendar.Data;

public class MailService : IMailService
{
    private readonly MailSettings _mailSettings;

    public MailService(IOptions<MailSettings> options)
    {
        _mailSettings = options.Value;
    }
    
    public bool SendMail(MailData mailData)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_mailSettings.Name, _mailSettings.EmailId));
            email.To.Add(new MailboxAddress(mailData.EmailToName, mailData.EmailToId));
            email.Subject = mailData.EmailSubject;

            var builder = new BodyBuilder();
            
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/guguColor.png");
            var image = builder.LinkedResources.Add(imagePath);
            image.ContentId = "guguLogo";
            
            builder.HtmlBody = $@"
            <html>
            <body>
                <p>Dobrý den {mailData.EmailToName},</p>
                <p>{mailData.EmailBody}</p>
                <p>S pozdravem,<br>Gugu Kalendář</p>
                <img src='cid:guguLogo' alt='Gugu logo' width='150'/>
            </body>
            </html>
        ";
            
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, _mailSettings.UseSSL);
            smtp.Authenticate(_mailSettings.EmailId, _mailSettings.Password);
            smtp.Send(email);
            smtp.Disconnect(true);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Chyba při odesílání e-mailu: {ex.Message}");
            return false;
        }
    }
}