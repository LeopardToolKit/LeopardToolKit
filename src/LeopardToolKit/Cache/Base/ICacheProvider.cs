using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    public interface ICacheProvider
    {
        /// <summary>
        /// Creates a data store.
        /// </summary>
        /// <param name="name"></param>
        /// <returns><see cref="ICache"/> if successfull, otherwise null.</returns>
        ICache CreateCache(string name);
    }
}
