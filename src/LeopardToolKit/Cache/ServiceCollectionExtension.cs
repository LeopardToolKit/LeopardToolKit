using LeopardToolKit.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ServiceCollectionExtension
    {
        public static IServiceCollection AddCache(this IServiceCollection services, Action<CacheBuilder> builder)
        {
            var cacheBuilder = new CacheBuilder(services);
            services.TryAddSingleton<ICacheFactory, CacheFactory>();
            services.TryAdd(ServiceDescriptor.Singleton(typeof(ICache<>), typeof(Cache<>)));

            builder.Invoke(cacheBuilder);
            return services;
        }
    }
}
