using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
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
    public class LeopardMemoryCache : ICache
    {
        private readonly MemoryCache _store = new MemoryCache(Options.Create(new MemoryCacheOptions()));

        public T Get<T>(string key)
        {
            return _store.Get<T>(key);
        }

        public void Put<T>(string key, T value, TimeSpan absoluteExpirationTime)
        {
            _store.Set(key, value, absoluteExpirationTime);
        }

        public void Remove(string key)
        {
            _store.Remove(key);
        }
    }
}
