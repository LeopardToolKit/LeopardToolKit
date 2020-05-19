using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LeopardToolKit.Cache
{
    public class MemoryCacheStore : ICacheStore
    {
        private readonly ConcurrentDictionary<string, CacheItem> _store = new ConcurrentDictionary<string, CacheItem>();

        public T Get<T>(string key)
        {
            if (_store.TryGetValue(key, out CacheItem cacheItem))
            {
                if(cacheItem == null || cacheItem.Value.IsEmpty())
                {
                    return default;
                }
                else if(cacheItem.ExpirationTime < DateTimeOffset.Now)
                {
                    Remove(key);
                    return default;
                }
                else
                {
                    ScanForExpiredItems(this);
                    return cacheItem.Value.DeserializeFromJson<T>();
                }
            }
            else
            {
                return default;
            }
        }

        public void Put<T>(string key, T value, TimeSpan absoluteExpirationTime)
        {
            key.ThrowIfNull(nameof(key));
            value.ThrowIfNull(nameof(value));
            CacheItem cacheItem = new CacheItem();
            cacheItem.ExpirationTime = DateTimeOffset.Now.Add(absoluteExpirationTime);
            cacheItem.Value = value.ToNewtonsoftJson();
            _store.AddOrUpdate(key, cacheItem, (x, y) => cacheItem);
            ScanForExpiredItems(this);
        }

        public void Remove(string key)
        {
            _store.TryRemove(key, out var _);
            ScanForExpiredItems(this);
        }

        private class CacheItem
        {
            public string Value { get; set; }

            public DateTimeOffset ExpirationTime { get; set; }
        }

        private DateTimeOffset lastScanTime = DateTimeOffset.Now;

        private void ScanForExpiredItems(MemoryCacheStore memoryCacheStore)
        {
            if (memoryCacheStore.lastScanTime.AddMinutes(1) < DateTimeOffset.Now)
            {
                memoryCacheStore.lastScanTime = DateTimeOffset.Now;
                Task.Factory.StartNew(state =>
                {
                    List<string> removingKeys = new List<string>();
                    MemoryCacheStore cacheStore = (MemoryCacheStore)state;
                    foreach (var kvp in cacheStore._store.ToList())
                    {
                        if(kvp.Value.ExpirationTime < DateTimeOffset.Now)
                        {
                            removingKeys.Add(kvp.Key);
                        }
                    }
                    foreach (var key in removingKeys)
                    {
                        memoryCacheStore.Remove(key);
                    }
                }, memoryCacheStore,
                CancellationToken.None,
                TaskCreationOptions.DenyChildAttach,
                TaskScheduler.Default);
            }

        }
    }
}
