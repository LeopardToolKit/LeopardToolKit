using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LeopardToolKit.Email
{
    public interface IEmailSender
    {
        void Send(EmailModel emailModel);

        Task SendAsync(EmailModel emailModel);
    }
}
