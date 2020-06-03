using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.GoogleAuthenticator
{
    public class GoogleTwoFactorAuthenticator : ITwoFactorAuthenticator
    {
        private TwoFactorAuthenticator googleTwoFactorAuthenticator = new TwoFactorAuthenticator();
        public string GenerateCode(string key)
        {
            var code = googleTwoFactorAuthenticator.GenerateSetupCode("", "", Encoding.UTF8.GetBytes(key), 20, false);
            return code.ManualEntryKey;
        }

        public bool ValidateCode(string key, string code)
        {
            return googleTwoFactorAuthenticator.ValidateTwoFactorPIN(key, code, TimeSpan.FromMinutes(1));
        }
    }
}
