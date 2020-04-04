using LeopardToolKit.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ServiceCollectionExtension
    {
        public static IServiceCollection AddEmailSender(this IServiceCollection services, Action<EmailOption> configBuilder, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        {
            services.TryAdd(ServiceDescriptor.Describe(typeof(IEmailSender), typeof(SmtpEmailSender), serviceLifetime));
            services.Configure(configBuilder);
            return services;
        }
        public static IServiceCollection AddEmailSender(this IServiceCollection services, IConfiguration configuration, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        {
            services.TryAdd(ServiceDescriptor.Describe(typeof(IEmailSender), typeof(SmtpEmailSender), serviceLifetime));
            services.Configure<EmailOption>(configuration);
            return services;
        }
    }
}
