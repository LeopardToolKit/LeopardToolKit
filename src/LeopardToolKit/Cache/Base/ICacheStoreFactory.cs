using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    public interface ICacheStoreFactory
    {
        /// <summary>
        /// Adds a provider to the factory.
        /// </summary>
        /// <param name="provider">The provider to add.</param>
        /// <returns>true if successful, otherwise false.</returns>
        bool AddProvider(ICacheStoreProvider provider);

        /// <summary>
        /// Creates a data store.
        /// </summary>
        /// <param name="categoryName">The name of teh data store.</param>
        /// <returns>true if successful, otherwise false.</returns>
        ICacheStore CreateDataStore(string categoryName);
    }
}
