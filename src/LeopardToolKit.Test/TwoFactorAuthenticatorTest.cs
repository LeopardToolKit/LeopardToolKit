using LeopardToolKit.GoogleAuthenticator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Test
{
    [TestClass]
    public class TwoFactorAuthenticatorTest
    {
        [TestMethod]
        public void TestGoogleTwoFactorAuthenticator()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddGoogleTwoFactorAuthenticator();
            var provider = services.BuildServiceProvider();
            var twoFactor = provider.GetRequiredService<ITwoFactorAuthenticator>();
            var code = twoFactor.GenerateCode(Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10));
            Assert.IsNotNull(code);
        }
    }
}
