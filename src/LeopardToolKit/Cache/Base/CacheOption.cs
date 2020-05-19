﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    public class CacheOption
    {
        public string DefaultProvider { get; set; }

        public string DefaultCategory { get; set; }

        public List<CacheItemOption> CacheItems { get; set; }
    }

    public class CacheItemOption
    {
        public string CacheCategory { get; set; }

        public string ProviderType { get; set; }
    }
}
