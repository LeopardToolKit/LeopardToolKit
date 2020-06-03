using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.GoogleAuthenticator
{
    public static partial class ServiceCollectionExtension
    {
        public static IServiceCollection AddGoogleTwoFactorAuthenticator(this IServiceCollection services)
        {
            services.AddSingleton<ITwoFactorAuthenticator, GoogleTwoFactorAuthenticator>();
            return services;
        }
    }
}
