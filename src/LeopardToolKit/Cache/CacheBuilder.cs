using LeopardToolKit.Cache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public class CacheBuilder
    {
        internal CacheBuilder(IServiceCollection services)
        {
            this.Services = services;
        }
        public IServiceCollection Services { get; }
    }

    public static class CacheBuilderExtension
    {
        #region AddMemoryProvider
        public static CacheBuilder AddMemoryProvider(this CacheBuilder cacheBuilder, Action<MemoryCacheOptions> setupAction = null)
        {
            cacheBuilder.AddCacheProvider<MemoryCacheProvider>();
            if (setupAction == null)
            {
                setupAction = option => { };
            }
            cacheBuilder.Services.Configure(setupAction);
            return cacheBuilder;
        }

        public static CacheBuilder AddMemoryProvider(this CacheBuilder cacheBuilder, IConfiguration configuration)
        {
            cacheBuilder.AddCacheProvider<MemoryCacheProvider>();
            cacheBuilder.Services.Configure<MemoryCacheOptions>(configuration);
            return cacheBuilder;
        }
        #endregion

        #region AddRedisProvider
        public static CacheBuilder AddRedisProvider(this CacheBuilder cacheBuilder, Action<RedisCacheOption> optionBuilder)
        {
            cacheBuilder.Services.Configure(optionBuilder);
            cacheBuilder.AddCacheProvider<RedisCacheProvider>();
            return cacheBuilder;
        }

        public static CacheBuilder AddRedisProvider(this CacheBuilder cacheBuilder, IConfiguration configuration)
        {
            cacheBuilder.Services.Configure<RedisCacheOption>(configuration);
            cacheBuilder.AddCacheProvider<RedisCacheProvider>();
            return cacheBuilder;
        }
        #endregion

        public static CacheBuilder AddConfiguration(this CacheBuilder cacheBuilder, IConfiguration configuration)
        {
            cacheBuilder.Services.Configure<CacheOption>(configuration);
            return cacheBuilder;
        }

        public static CacheBuilder AddConfiguration(this CacheBuilder cacheBuilder, Action<CacheOption> optionBuilder)
        {
            cacheBuilder.Services.Configure(optionBuilder);
            return cacheBuilder;
        }

        public static CacheBuilder AddCacheProvider<TCacheProvider>(this CacheBuilder cacheBuilder) 
            where TCacheProvider : ICacheProvider
        {
            cacheBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(ICacheProvider), typeof(TCacheProvider)));
            return cacheBuilder;
        }

        public static CacheBuilder AddCacheProvider<TCacheProvider>(this CacheBuilder cacheBuilder, TCacheProvider cacheProvider) 
            where TCacheProvider : ICacheProvider
        {
            cacheBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(ICacheProvider), cacheProvider));
            return cacheBuilder;
        }
    }
}
