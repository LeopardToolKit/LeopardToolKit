using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    [CacheProviderAlias("Memory")]
    public class MemoryCacheProvider : ICacheProvider
    {
        private static readonly ConcurrentDictionary<string, LeopardMemoryCache> s_registrations = new ConcurrentDictionary<string, LeopardMemoryCache>();

        private readonly IOptions<MemoryCacheOptions> optionsAccessor;

        public MemoryCacheProvider(IOptions<MemoryCacheOptions> optionsAccessor)
        {
            this.optionsAccessor = optionsAccessor;
        }

        public ICache CreateCache(string name)
        {
            name.ThrowIfNull(nameof(name));
            LeopardMemoryCache dataStore;
            if (!s_registrations.TryGetValue(name, out dataStore))
            {
                dataStore = new LeopardMemoryCache(optionsAccessor, name);
                if (!s_registrations.TryAdd(name, dataStore))
                {
                    throw new InvalidOperationException("The specified memory data store could not be created.");
                }
            }

            return dataStore;
        }
    }
}
