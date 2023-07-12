using aims_api.Repositories.Interface;
using aims_api.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace aims_api.Repositories.Implementation
{
    public class EmailSvcRepository : IEmailSvcRepository
    {
        public async Task<bool> SendEmail(Tenant tenant, string recipients, string subject, string emailBody)
        {
            //init credentials
            string? sender = tenant.SenderEmail;
            string? pass = tenant.SenderKey;
            string? smtp = tenant.SMTP;
            int port = tenant.Port;

            //init smpt service
            SmtpClient smtpClient = new SmtpClient(smtp, port);
            smtpClient.Timeout = 100000;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;

            //setup email credential
            if (pass.Length > 0)
            {
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(sender, pass);
            }
            else
            {
                smtpClient.EnableSsl = false;
            }

            MailMessage mailMessage = new MailMessage(sender, recipients, subject, emailBody);
            mailMessage.IsBodyHtml = true;
            mailMessage.BodyEncoding = UTF8Encoding.UTF8;
            await smtpClient.SendMailAsync(mailMessage);
            return true;
        }
    }
}
