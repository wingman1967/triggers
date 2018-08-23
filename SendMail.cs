using System.Net.Mail;

namespace ConfigureOneFlag
{
    /// <summary>
    /// Send email to specified users for errors encountered during XML mapping
    /// </summary>
    class SendMail
    {
        public static bool suppressRecips = false;
        public static void MailMessage(string messageBody, string messageSubject)
        {
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = true;
            mail.From = new MailAddress(DatabaseFactory.emailFrom);
            mail.To.Add(new MailAddress(DatabaseFactory.emailaddr));
            if (!suppressRecips)
            {
                mail.To.Add(new MailAddress("GrantH@NatlPump.com"));
                mail.To.Add(new MailAddress("Thomas.Stock@NatlPump.com"));
            }
            
            mail.Subject = messageSubject;
            mail.Body = messageBody;
            SmtpClient smtp = new SmtpClient(DatabaseFactory.emailServer);
            smtp.Send(mail);
        }
    }
}
