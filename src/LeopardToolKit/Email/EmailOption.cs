using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Email
{
    public class EmailOption
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public bool IsSSL { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string FromAddress { get; set; }

        public string FromDisplayName { get; set; }
    }
}
