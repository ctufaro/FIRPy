using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;


namespace FIRPy.Notifications
{
    public class Emailer
    {
        public static void SendEmail(string subject, string body)
        {
           
            string gUser = ConfigurationSettings.AppSettings["GmailUser"];
            string gEmail = ConfigurationSettings.AppSettings["GmailEmail"];
            string gPassword = ConfigurationSettings.AppSettings["GmailPassword"];

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
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }
    }
}
