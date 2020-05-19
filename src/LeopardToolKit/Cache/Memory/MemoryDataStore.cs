using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    public class MemoryDataStore : ICacheStore
    {
        private readonly ConcurrentDictionary<string, string> _store = new ConcurrentDictionary<string, string>();

        public T Get<T>(string key)
        {
            return _store.TryGetValue(key, out string retval) && !retval.IsEmpty() ? retval.DeserializeFromJson<T>() : default;
        }

        public void Put<T>(string key, T value)
        {
            key.ThrowIfNull(nameof(key));
            value.ThrowIfNull(nameof(value));
            _store.AddOrUpdate(key, value.ToNewtonsoftJson(), (x, y) => value.ToNewtonsoftJson());
        }

        public void Remove(string key)
        {
            _store.TryRemove(key, out var _);
        }
    }
}
