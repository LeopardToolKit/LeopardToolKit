using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    public class RedisCacheStore : ICacheStore
    {
        private readonly RedisDB redisDB;
        private readonly string categoryName;
        private readonly int cacheDataBaseIndex;

        public RedisCacheStore(RedisDB redisDB, string categoryName, int cacheDataBaseIndex)
        {
            this.redisDB = redisDB;
            this.categoryName = categoryName;
            this.cacheDataBaseIndex = cacheDataBaseIndex;
        }

        public T Get<T>(string key)
        {
            return this.redisDB.Get<T>(cacheDataBaseIndex, categoryName + key);
        }

        public void Put<T>(string key, T value, TimeSpan absoluteExpirationTime)
        {
            this.redisDB.Set(cacheDataBaseIndex, categoryName + key, value, absoluteExpirationTime);
        }

        public void Remove(string key)
        {
            this.redisDB.Remove(cacheDataBaseIndex, categoryName + key);
        }
    }
}
