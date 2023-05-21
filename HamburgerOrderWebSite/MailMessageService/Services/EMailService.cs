using MailMessageService.Configurations;
using MimeKit.Text;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailMessageService.Models;
using MailKit.Net.Smtp;

namespace MailMessageService.Services
{
    public class EMailService : IEmailService
    {
        private readonly EmailConfig _emailConfig;
        public EMailService(EmailConfig emailConfig)
        {
            //DEPENDENCY INJECTION
            _emailConfig = emailConfig;
        }

        public void SendEmail(MailMessage mailMessage)
        {
            var emailMessage = CreateEmailMessage(mailMessage);
            Send(emailMessage);
        }

        private void Send(MimeMessage emailMessage)
        {
            //Smtp Client - Simple Mail Transfer Protocol Client
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    //Authentication - GİRİŞ YAPMA İŞLEMİ
                    client.Authenticate(_emailConfig.Username, _emailConfig.Password);
                    client.Send(emailMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Mesaj gönderilirken hata oluştu {ex.Message}");
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

        private MimeMessage CreateEmailMessage(MailMessage mailMessage)
        {
            MimeMessage emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Mail Onay Mesajı", _emailConfig.From));
            emailMessage.To.AddRange(mailMessage.To);
            emailMessage.Subject = mailMessage.Subject;
            emailMessage.Body = new TextPart(TextFormat.Html) { Text = mailMessage.Body };
            return emailMessage;
        }
    }
}
