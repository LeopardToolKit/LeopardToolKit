using Newtonsoft.Json;
using System;
using System.Reflection;

namespace LeopardToolKit
{
    public static class CoreExtensions
    {
        public static T ThrowIfNull<T>(this T obj, string objName)
        {
            if (typeof(T).GetTypeInfo().IsValueType || null != obj)
            {
                return obj;
            }

            throw new ArgumentNullException(objName);
        }

        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str);
        }

        public static string ToNewtonsoftJson(this object obj, JsonSerializerSettings settings = null)
        {
            return JsonConvert.SerializeObject(obj, settings);
        }

        public static T DeserializeFromJson<T>(this string value, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeObject<T>(value, settings);
        }
    }
}
