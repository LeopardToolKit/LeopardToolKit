using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LeopardToolKit.Locker
{
    public class ProcessLock :ILock, IDisposable
    {
        private SemaphoreSlim semaphoreSlim;
        private bool shouldRelease = false;

        public ProcessLock(string lockName)
        {
            semaphoreSlim = ProcessLockPool.GetSemaphoreSlim(lockName);
        }

        public void Dispose()
        {
            Release();
        }

        public void Release()
        {
            if(shouldRelease)
            {
                shouldRelease = false;
                semaphoreSlim.Release();
            }
            
        }

        public bool Wait(TimeSpan? timeout = null)
        {
            if(shouldRelease)
            {
                return true;
            }
            if(timeout == null)
            {
                timeout = TimeSpan.FromSeconds(10);
            }
            shouldRelease = semaphoreSlim.Wait(timeout.Value);                
            
            return shouldRelease;
        }

        public async Task<bool> WaitAsync(TimeSpan? timeout = null)
        {
            if (shouldRelease)
            {
                return await Task.FromResult(true);
            }

            if (timeout == null)
            {
                timeout = TimeSpan.FromSeconds(10);
            }
            shouldRelease = await semaphoreSlim.WaitAsync(timeout.Value);
            return shouldRelease;
        }
    }

    internal class ProcessLockPool
    {
        private static ConcurrentDictionary<string, SemaphoreSlim> Locks = new ConcurrentDictionary<string, SemaphoreSlim>();

        internal static SemaphoreSlim GetSemaphoreSlim(string lockName)
        {
            return Locks.GetOrAdd(lockName, x => new SemaphoreSlim(1, 1));
        }
    }
}
