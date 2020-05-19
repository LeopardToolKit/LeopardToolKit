using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    public class RedisCacheStoreOption
    {
        public string RedisConnection { get; set; }

        public int DataBaseIndex { get; set; }
    }
}
