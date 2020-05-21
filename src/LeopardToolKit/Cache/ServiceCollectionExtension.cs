using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddCache(this IServiceCollection services, Action<CacheBuilder> builder)
        {
            var cacheBuilder = new CacheBuilder(services);
            builder.Invoke(cacheBuilder);
            return services;
        }
    }
}
