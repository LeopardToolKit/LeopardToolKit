using LeopardToolKit.Cache;
using LeopardToolKit.Office;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LeopardToolKit.Test
{
    [TestClass]
    public class CacheTest
    {
        [TestMethod]
        public void TestCache()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddCache(builder => {
                builder.AddCache();
                builder.AddMemory();
                builder.AddConfiguration(b => b.DefaultProvider = "Memory");
            });
            var provider = services.BuildServiceProvider();
            var cacheFactory = provider.GetRequiredService<ICacheFactory>();
            var cache1 = cacheFactory.CreateCache("Test");
            var cache2 = provider.GetRequiredService<ICache<CacheTest>>();
            Assert.IsFalse(cache1 == cache2);
        }
    }

}
