using Microsoft.AspNetCore.Mvc;
using WannabeGCalendar.Data;

[ApiController]
[Route("api/[controller]")]
public class MailController : ControllerBase
{
    IMailService mailService;
//injecting the IMailService into the constructor
    public MailController(IMailService _MailService)
    {
        mailService = _MailService;
    }
    
    [HttpPost]
    public bool SendMail(MailData mailData)
    {
        try
        {
            mailService.SendMail(mailData);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Chyba při odesílání e-mailu: {ex.Message}");
            return false;
        }
    }
}