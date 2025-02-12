namespace WannabeGCalendar.Data;

public interface IMailService
{
    bool SendMail(MailData mailData);
}