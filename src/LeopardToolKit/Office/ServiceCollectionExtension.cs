using LeopardToolKit.Office;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ServiceCollectionExtension
    {
        public static IServiceCollection AddNpoiOffice(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        {
            services.TryAdd(ServiceDescriptor.Describe(typeof(IOffice), typeof(NPOIOffice), serviceLifetime));
            return services;
        }
    }
}
