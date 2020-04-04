using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeopardToolKit.Email
{
    public class SmtpEmailSender : IEmailSender
    {

        private readonly EmailOption emailOption;

        public SmtpEmailSender(IOptions<EmailOption> options)
        {
            this.emailOption = options.Value;
        }


        public void Send(EmailModel emailModel)
        {
            using (SmtpClient client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(emailOption.Host, emailOption.Port, emailOption.IsSSL);

                client.Authenticate(emailOption.Username, emailOption.Password);
                client.Send(GenerateEmailMessage(emailModel));
                client.Disconnect(true);
            }
        }

        public async Task SendAsync(EmailModel emailModel)
        {
            using (SmtpClient client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(emailOption.Host, emailOption.Port, emailOption.IsSSL);

                client.Authenticate(emailOption.Username, emailOption.Password);
                await client.SendAsync(GenerateEmailMessage(emailModel));
                client.Disconnect(true);
            }
        }

        private MimeMessage GenerateEmailMessage(EmailModel emailModel)
        {
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.From = new System.Net.Mail.MailAddress(emailOption.FromAddress, emailOption.FromDisplayName ?? emailOption.FromAddress);
            emailModel.ToMails.ForEach(address => message.To.Add(address));
            message.Subject = emailModel.Subject;
            message.Body = emailModel.EmailBody;
            message.IsBodyHtml = emailModel.IsHtml;

            if (emailModel.AttachmentPaths?.Any() == true)
            {
                emailModel.AttachmentPaths.ForEach(filePath => {
                    message.Attachments.Add(new System.Net.Mail.Attachment(filePath));
                });
            }
            //https://github.com/jstedfast/MimeKit/blob/master/MimeKit/MimeMessage.cs line3041
            return (MimeMessage)message;
        }
    }
}
