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
    }
}
