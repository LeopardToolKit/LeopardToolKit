using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit
{
    public interface ITwoFactorAuthenticator
    {
        string GenerateCode(string key);

        bool ValidateCode(string key, string code);
    }
}
