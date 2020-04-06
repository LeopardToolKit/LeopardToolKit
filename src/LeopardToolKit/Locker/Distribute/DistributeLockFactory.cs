using RedLockNet.SERedis;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Locker
{
    public class DistributeLockFactory : ILockFactory, IDisposable
    {
        private readonly RedLockFactory redLockFactory;
        public DistributeLockFactory(RedLockFactory redLockFactory)
        {
            this.redLockFactory = redLockFactory;
        }

        public void Dispose()
        {
            this.redLockFactory.Dispose();
        }

        public ILock GetLock(string lockName)
        {
            return new DistributeLock(lockName, this.redLockFactory);
        }
    }
}
