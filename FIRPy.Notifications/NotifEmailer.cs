using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;


namespace FIRPy.Notifications
{
    public class NotifEmailer
    {
        public static void SendEmail(string subject, string body, string attachment = "")
        {
            
            string gUser = ConfigurationSettings.AppSettings["GmailUser"];
            string gEmail = ConfigurationSettings.AppSettings["GmailEmail"];
            string gPassword = ConfigurationSettings.AppSettings["GmailPassword"];

            MailMessage message = null;
            var fromAddress = new MailAddress(gEmail, gUser);
            var toAddress = new MailAddress(gEmail, gUser);
            string fromPassword = gPassword;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (message = new MailMessage(fromAddress, toAddress))
            {
                
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                if (!string.IsNullOrEmpty(attachment))
                {
                    message.Attachments.Add(new Attachment(attachment));
                }

                smtp.Send(message);
            }
        }
    }
}
