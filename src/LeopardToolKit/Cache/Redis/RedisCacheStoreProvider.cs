using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    [CacheProviderAlias("RedisCache")]
    public class RedisCacheStoreProvider : ICacheStoreProvider
    {
        private readonly RedisDB redisDB;
        private readonly int cacheIndex;

        public RedisCacheStoreProvider(IOptions<RedisCacheStoreOption> options)
        {
            this.redisDB = new RedisDB(options.Value.RedisConnection);
            this.cacheIndex = options.Value.DataBaseIndex;
        }

        public ICacheStore CreateDataStore(string name)
        {
            return new RedisCacheStore(this.redisDB, name, cacheIndex);
        }
    }
}
