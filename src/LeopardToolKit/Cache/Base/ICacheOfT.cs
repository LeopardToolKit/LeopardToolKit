using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    public interface ICache<out TCategory> : ICache
    {
    }
}
