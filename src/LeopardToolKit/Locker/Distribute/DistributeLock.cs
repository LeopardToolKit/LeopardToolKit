using RedLockNet;
using RedLockNet.SERedis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LeopardToolKit.Locker
{
    public class DistributeLock : ILock, IDisposable
    {
        private readonly string lockName;
        private readonly RedLockFactory redLockFactory;
        private IRedLock redLock;

        private bool hasReleased = false;

        public DistributeLock(string lockName, RedLockFactory redLockFactory)
        {
            this.lockName = lockName;
            this.redLockFactory = redLockFactory;
        }   
        public void Dispose()
        {
            Release();
        }

        public void Release()
        {
            if (hasReleased)
            {
                return;
            }
            this.redLock?.Dispose();
            hasReleased = true;
        }

        public bool Wait(TimeSpan? timeout = null)
        {
            if(timeout == null)
            {
                timeout = TimeSpan.FromSeconds(20);
            }
            this.redLock= this.redLockFactory.CreateLock(lockName, TimeSpan.FromSeconds(30), timeout.Value, TimeSpan.FromSeconds(0.1));
            return redLock.IsAcquired;
        }

        public async Task<bool> WaitAsync(TimeSpan? timeout = null)
        {
            if (timeout == null)
            {
                timeout = TimeSpan.FromSeconds(20);
            }
            this.redLock = await this.redLockFactory.CreateLockAsync(lockName, TimeSpan.FromSeconds(30), timeout.Value, TimeSpan.FromSeconds(0.1));
            if (redLock.IsAcquired)
            {
                hasReleased = false;
            }
            return redLock.IsAcquired;
        }
    }
}
