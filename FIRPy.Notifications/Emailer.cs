using System;
using System.Collections.Generic;
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
            var fromAddress = new MailAddress("ctufaro@gmail.com", "Chris Tufaro");
            var toAddress = new MailAddress("ctufaro@gmail.com", "Chris Tufaro");
            const string fromPassword = "marble890";

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
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}
