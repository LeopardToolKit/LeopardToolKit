using LeopardToolKit;
using LeopardToolKit.Cache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ServiceCollectionExtension
    {
        public static void AddCacheOfMemory(this IServiceCollection services, IConfiguration configuration)
        {
            configuration.ThrowIfNull(nameof(configuration));
            services.Configure<CacheOption>(configuration);
            services.AddSingleton<ICacheStoreFactory, CacheStoreFactory>();
            if (configuration.GetSection("RedisCacheStoreOption") != null)
            {
                services.Configure<RedisCacheStoreOption>(configuration.GetSection("RedisCacheStoreOption"));
                services.TryAddEnumerable(ServiceDescriptor.Singleton<ICacheStoreProvider, RedisCacheStoreProvider>());
            }
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ICacheStoreProvider, MemoryCacheStoreProvider>());
        }
    }
}
