using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    public class CacheOption
    {
        public string DefaultProvider { get; set; }

        public List<CacheItemOption> CacheCategory { get; set; }
    }
    
    public class CacheItemOption
    {
        /// <summary>
        /// The name of category
        /// </summary>
        public string CacheCategory { get; set; }

        /// <summary>
        /// The full name of the implementation type of <see cref="ICacheProvider"/>
        /// Or the name provided by <see cref="CacheProviderAliasAttribute"/>
        /// </summary>
        public string ProviderType { get; set; }
    }
}
