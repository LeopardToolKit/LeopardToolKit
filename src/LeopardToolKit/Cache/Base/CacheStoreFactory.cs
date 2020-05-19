using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LeopardToolKit.Cache
{
    public class CacheStoreFactory : ICacheStoreFactory
    {
        private readonly ConcurrentDictionary<string, ICacheStoreProvider> providers;
        private readonly CacheOption cacheOption;

        public CacheStoreFactory(IOptions<CacheOption> options, IEnumerable<ICacheStoreProvider> cacheStoreProviders)
        {
            this.cacheOption = options.Value;
            this.providers = new ConcurrentDictionary<string, ICacheStoreProvider>();
            foreach (var provider in cacheStoreProviders)
            {
                providers.TryAdd(GetProviderName(provider.GetType()), provider);
            }
        }

        public bool AddProvider(ICacheStoreProvider provider)
        {
            provider.ThrowIfNull(nameof(provider));

            return !providers.ContainsKey(GetProviderName(provider.GetType())) 
                && providers.TryAdd(GetProviderName(provider.GetType()), provider);
        }

        public ICacheStore CreateDataStore(string categoryName)
        {
            categoryName = categoryName ?? this.cacheOption.DefaultCategory;
            categoryName.ThrowIfNull(nameof(categoryName));

            var cacheItem = this.cacheOption.CacheItems.FirstOrDefault(i => i.CacheCategory == categoryName);
            var providerType = cacheItem?.ProviderType;
            if (providerType.IsEmpty())
            {
                providerType = this.cacheOption.DefaultProvider;
            }

            providerType.ThrowIfNull(nameof(providerType));

            if(providers.TryGetValue(providerType, out var provider))
            {
                return provider.CreateDataStore(categoryName);
            }
            else
            {
                throw new Exception($"Can not find a provider for provider type: {providerType}");
            }
        }

        private string GetProviderName(Type providerType)
        {
            var attr =providerType.GetCustomAttribute<CacheProviderAliasAttribute>();
            return attr?.Name ?? providerType.FullName;
        }
    }
}
