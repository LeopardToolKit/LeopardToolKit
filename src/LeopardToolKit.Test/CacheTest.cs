using LeopardToolKit.Cache;
using LeopardToolKit.Office;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeopardToolKit.Test
{
    [TestClass]
    public class CacheTest
    {
        [TestMethod]
        public async Task TestCacheDefault()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddCache(builder => {
                builder.AddMemoryProvider();
                builder.AddConfiguration(b => b.DefaultProvider = "Memory");
            });
            var provider = services.BuildServiceProvider();
            var cacheFactory = provider.GetRequiredService<ICacheFactory>();
            var cache = provider.GetRequiredService<ICache<CacheTest>>();
            int value =2;
            string key = "int";
            cache.Put(key, value, TimeSpan.FromSeconds(5));
            var cachedValue = cache.Get<int>(key);
            Assert.AreEqual(value, cachedValue);
            await Task.Delay(5000);
            cachedValue = cache.Get<int>(key);
            Assert.AreEqual(0, cachedValue);
        }

        [TestMethod]
        public void TestCacheDifferentForDifferentCategory()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddCache(builder => {
                builder.AddMemoryProvider();
                builder.AddConfiguration(b => b.DefaultProvider = "Memory");
            });
            var provider = services.BuildServiceProvider();
            var cacheFactory = provider.GetRequiredService<ICacheFactory>();
            var cache1 = cacheFactory.CreateCache("Test");
            var cache2 = provider.GetRequiredService<ICache<CacheTest>>();
            Assert.IsFalse(cache1 == cache2);
        }

        [TestMethod]
        public void TestCache()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddCache(builder => {
                builder.AddMemoryProvider();
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
