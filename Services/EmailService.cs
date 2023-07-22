using System.Net;
using System.Net.Mail;
using Todo.Configurations;

namespace Todo.Services;

public class EmailService
{
    public bool Send(
        string toName,
        string toEmail,
        string subject,
        string body,
        string fromName = "Todo - Api",
        string fromEmail = "achiles1707@gmail.com"
    )
    {
        var smtpClient = new SmtpClient(SmtpConfiguration.Host, SmtpConfiguration.Port);

        smtpClient.Credentials = new NetworkCredential(SmtpConfiguration.Username, SmtpConfiguration.Password);
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtpClient.EnableSsl = true;
        var mail = new MailMessage();

        mail.From = new MailAddress(fromEmail, fromName);
        mail.To.Add(new MailAddress(toEmail, toName));
        mail.Subject = subject;
        mail.Body = body;
        mail.IsBodyHtml = true;

        try
        {
            smtpClient.Send(mail);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}