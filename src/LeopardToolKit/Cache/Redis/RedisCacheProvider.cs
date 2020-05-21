using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    [CacheProviderAlias("Redis")]
    public class RedisCacheProvider : ICacheProvider
    {
        private readonly RedisDB redisDB;
        private readonly int cacheIndex;

        public RedisCacheProvider(IOptions<RedisCacheOption> options)
        {
            this.redisDB = new RedisDB(options.Value.RedisConnection);
            this.cacheIndex = options.Value.DataBaseIndex;
        }

        public ICache CreateCache(string name)
        {
            return new RedisCache(this.redisDB, name, cacheIndex);
        }
    }
}
