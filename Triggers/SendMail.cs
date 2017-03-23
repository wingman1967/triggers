using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace ConfigureOneFlag
{
    /// <summary>
    /// Send email to specified users for errors encountered during XML mapping
    /// </summary>
    class SendMail
    {
        public static void MailMessage(string messageBody, string messageSubject)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(DatabaseFactory.emailFrom);
            mail.To.Add(new MailAddress(DatabaseFactory.emailaddr));
            mail.Subject = messageSubject;
            mail.Body = messageBody;
            SmtpClient smtp = new SmtpClient(DatabaseFactory.emailServer);
            smtp.Send(mail);
        }
    }
}
