using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    [CacheProviderAlias("MemoryCache")]
    public class MemoryCacheStoreProvider : ICacheStoreProvider
    {
        private static readonly ConcurrentDictionary<string, MemoryCacheStore> s_registrations = new ConcurrentDictionary<string, MemoryCacheStore>();

        public ICacheStore CreateDataStore(string name)
        {
            name.ThrowIfNull(nameof(name));
            MemoryCacheStore dataStore;
            if (!s_registrations.TryGetValue(name, out dataStore))
            {
                dataStore = new MemoryCacheStore();
                if (!s_registrations.TryAdd(name, dataStore))
                {
                    throw new InvalidOperationException("The specified memory data store could not be created.");
                }
            }

            return dataStore;
        }
    }
}
