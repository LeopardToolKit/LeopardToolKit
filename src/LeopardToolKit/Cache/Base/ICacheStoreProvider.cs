using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    public interface ICacheStoreProvider
    {
        /// <summary>
        /// Creates a data store.
        /// </summary>
        /// <param name="name"></param>
        /// <returns><see cref="ICacheStore"/> if successfull, otherwise null.</returns>
        ICacheStore CreateDataStore(string name);
    }
}
