using LeopardToolKit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.Distributed
{
    public static class DistributedCacheExtension
    {
        private static Newtonsoft.Json.JsonSerializerSettings IgnoreSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore };
        public static async Task<T> GetOrSetAsync<T>(this IDistributedCache distributedCache, string key, Func<Task<T>> valueGetter, TimeSpan? absoluteExpirationRelativeToNow = default, CancellationToken token = default)
        {
            var valueString = await distributedCache.GetStringAsync(key, token);
            if (valueString.IsEmpty() && !token.IsCancellationRequested)
            {

                var value = await valueGetter.Invoke();
                valueString = value?.ToNewtonsoftJson(IgnoreSerializerSettings);
                await distributedCache.SetStringAsync(key, valueString, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow }, token);
                return value;
            }
            else
            {
                return valueString.DeserializeFromJson<T>(IgnoreSerializerSettings);
            }
        }

        public static T GetOrSet<T>(this IDistributedCache distributedCache, string key, Func<T> valueGetter, TimeSpan? absoluteExpirationRelativeToNow = default)
        {
            var valueString = distributedCache.GetString(key);
            if (valueString.IsEmpty())
            {
                var value = valueGetter.Invoke();
                valueString = value?.ToNewtonsoftJson(IgnoreSerializerSettings);
                distributedCache.SetString(key, valueString, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow });
                return value;
            }
            else
            {
                return valueString.DeserializeFromJson<T>(IgnoreSerializerSettings);
            }
        }
    }
}
