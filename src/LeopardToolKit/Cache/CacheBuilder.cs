using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
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
        public static CacheBuilder AddCache(this CacheBuilder cacheBuilder)
        {
            cacheBuilder.Services.TryAddSingleton<ICacheFactory, CacheFactory>();
            cacheBuilder.Services.TryAdd(ServiceDescriptor.Singleton(typeof(ICache<>), typeof(Cache<>)));
            return cacheBuilder;
        }

        public static CacheBuilder AddMemory(this CacheBuilder cacheBuilder)
        {
            cacheBuilder.AddCache();
            cacheBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ICacheProvider, MemoryCacheProvider>());
            return cacheBuilder;
        }

        public static CacheBuilder AddRedis(this CacheBuilder cacheBuilder, Action<RedisCacheOption> optionBuilder)
        {
            cacheBuilder.AddCache();
            cacheBuilder.Services.Configure(optionBuilder);
            cacheBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ICacheProvider, RedisCacheProvider>());
            return cacheBuilder;
        }

        public static CacheBuilder AddRedis(this CacheBuilder cacheBuilder, IConfiguration configuration)
        {
            cacheBuilder.AddCache();
            cacheBuilder.Services.Configure<RedisCacheOption>(configuration);
            cacheBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ICacheProvider, RedisCacheProvider>());
            return cacheBuilder;
        }

        public static CacheBuilder AddConfiguration(this CacheBuilder cacheBuilder, IConfiguration configuration)
        {
            cacheBuilder.AddCache();
            cacheBuilder.Services.Configure<CacheOption>(configuration);
            return cacheBuilder;
        }

        public static CacheBuilder AddConfiguration(this CacheBuilder cacheBuilder, Action<CacheOption> optionBuilder)
        {
            cacheBuilder.AddCache();
            cacheBuilder.Services.Configure(optionBuilder);
            return cacheBuilder;
        }

        public static CacheBuilder AddCacheProvider<TCacheProvider>(this CacheBuilder cacheBuilder) 
            where TCacheProvider : ICacheProvider
        {
            cacheBuilder.AddCache();
            cacheBuilder.Services.TryAddSingleton(typeof(ICacheProvider), typeof(TCacheProvider)); ;
            return cacheBuilder;
        }

        public static CacheBuilder AddCacheProvider<TCacheProvider>(this CacheBuilder cacheBuilder, TCacheProvider cacheProvider) 
            where TCacheProvider : ICacheProvider
        {
            cacheBuilder.AddCache();
            cacheBuilder.Services.TryAddSingleton<ICacheProvider>(cacheProvider);
            return cacheBuilder;
        }
    }
}
