using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeopardToolKit.Locker
{
    public interface ILockFactory
    {
        ILock GetLock(string lockName);
    }
}
