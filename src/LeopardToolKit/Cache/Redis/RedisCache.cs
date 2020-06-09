using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    public class RedisCache : ICache
    {
        private readonly RedisDB redisDB;
        private readonly int cacheDataBaseIndex;

        public RedisCache(RedisDB redisDB, string categoryName, int cacheDataBaseIndex)
        {
            this.redisDB = redisDB;
            this.CategoryName = categoryName;
            this.cacheDataBaseIndex = cacheDataBaseIndex;
        }

        public string CategoryName { get; }

        public T Get<T>(string key)
        {
            return this.redisDB.Get<T>(cacheDataBaseIndex, CategoryName + key);
        }

        public void Put<T>(string key, T value, TimeSpan absoluteExpirationTime)
        {
            this.redisDB.Set(cacheDataBaseIndex, CategoryName + key, value, absoluteExpirationTime);
        }

        public void Remove(string key)
        {
            this.redisDB.Remove(cacheDataBaseIndex, CategoryName + key);
        }
    }
}
