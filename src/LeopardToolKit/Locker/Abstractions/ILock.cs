using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeopardToolKit.Locker
{
    public interface ILock : IDisposable
    {
        void Release();

        bool Wait(TimeSpan? timeout = null);

        Task<bool> WaitAsync(TimeSpan? timeout = null);
    }
}
