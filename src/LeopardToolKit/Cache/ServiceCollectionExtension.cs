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
            AddCache(services, configuration);
        }


        private static void AddCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<LeopardToolKit.Cache.CacheOption>(configuration);
            services.AddSingleton<ICacheStoreFactory, CacheStoreFactory>();
        }
    }
}
