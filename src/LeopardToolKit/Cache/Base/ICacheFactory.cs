using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    public interface ICacheFactory
    {
        /// <summary>
        /// Creates a cache.
        /// </summary>
        /// <param name="categoryName">The name of teh data store.</param>
        /// <returns>true if successful, otherwise false.</returns>
        ICache CreateCache(string categoryName);
    }
}
