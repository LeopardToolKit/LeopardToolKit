using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    public class MemoryDataStoreProvider : ICacheStoreProvider
    {
        private static readonly ConcurrentDictionary<string, MemoryDataStore> s_registrations = new ConcurrentDictionary<string, MemoryDataStore>();

        public ICacheStore CreateDataStore(string name)
        {
            name.ThrowIfNull(nameof(name));
            MemoryDataStore dataStore;
            if (!s_registrations.TryGetValue(name, out dataStore))
            {
                dataStore = new MemoryDataStore();
                if (!s_registrations.TryAdd(name, dataStore))
                {
                    throw new InvalidOperationException("The specified memory data store could not be created.");
                }
            }

            return dataStore;
        }
    }
}
