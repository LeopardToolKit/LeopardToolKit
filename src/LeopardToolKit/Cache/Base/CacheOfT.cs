using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    public class Cache<TCategory> : ICache<TCategory> where TCategory : class
    {
        private ICache cache;

        public Cache(ICacheFactory cacheFactory)
        {
            this.cache = cacheFactory.CreateCache(typeof(TCategory).FullName);
        }
        public T Get<T>(string key)
        {
            return this.cache.Get<T>(key);
        }

        public void Put<T>(string key, T value, TimeSpan absoluteExpirationTime)
        {
            this.cache.Put(key, value, absoluteExpirationTime);
        }

        public void Remove(string key)
        {
            this.cache.Remove(key);
        }
    }
}
