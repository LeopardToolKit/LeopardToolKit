using LeopardToolKit;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Caching.Distributed
{
    public static class DistributedCacheExtension
    {
        private static JsonSerializerSettings IgnoreSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore };
        public static async Task<T> GetOrSetAsync<T>(this IDistributedCache distributedCache, string key, Func<Task<T>> valueGetter, TimeSpan? absoluteExpirationRelativeToNow = default, CancellationToken token = default, JsonSerializerSettings serializerSettings = default)
        {
            serializerSettings = serializerSettings ?? IgnoreSerializerSettings;
            var valueString = await distributedCache.GetStringAsync(key, token);
            if (valueString.IsEmpty() && !token.IsCancellationRequested)
            {

                var value = await valueGetter.Invoke();
                valueString = value?.ToNewtonsoftJson(serializerSettings);
                await distributedCache.SetStringAsync(key, valueString, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow }, token);
                return value;
            }
            else
            {
                return valueString.DeserializeFromJson<T>(serializerSettings);
            }
        }

        public static T GetOrSet<T>(this IDistributedCache distributedCache, string key, Func<T> valueGetter, TimeSpan? absoluteExpirationRelativeToNow = default, JsonSerializerSettings serializerSettings)
        {
            serializerSettings = serializerSettings ?? IgnoreSerializerSettings;
            var valueString = distributedCache.GetString(key);
            if (valueString.IsEmpty())
            {
                var value = valueGetter.Invoke();
                valueString = value?.ToNewtonsoftJson(serializerSettings);
                distributedCache.SetString(key, valueString, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow });
                return value;
            }
            else
            {
                return valueString.DeserializeFromJson<T>(serializerSettings);
            }
        }
    }
}
