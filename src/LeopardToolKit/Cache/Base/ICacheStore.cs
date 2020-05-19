using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    public interface ICacheStore
    {
        /// <summary>
        /// Gets an item from the cache
        /// </summary>
        /// <param name="key">The key for the object.</param>
        /// <returns>The object.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Adds an item to the cache.
        /// </summary>
        /// <param name="key">The key for the object.</param>
        /// <param name="value">The value to store.</param>
        void Put<T>(string key, T value);

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        /// <param name="key">The key for the object.</param>
        void Remove(string key);
    }
}
