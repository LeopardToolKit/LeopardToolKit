using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeopardToolKit.Locker
{
    public class ProcessLockFactory : ILockFactory
    {
        public ILock GetLock(string lockName)
        {
            return new ProcessLock(lockName);
        }
    }
}
