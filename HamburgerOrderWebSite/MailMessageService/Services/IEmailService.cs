using MailMessageService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailMessageService.Services
{
    public interface IEmailService
    {
        void SendEmail(MailMessage mailMessage);
    }
}
