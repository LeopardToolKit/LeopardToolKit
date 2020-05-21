using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CacheProviderAliasAttribute : Attribute
    {
        public string Name { get; set; }

        public CacheProviderAliasAttribute(string name)
        {
            this.Name = name;
        }
    }
}
