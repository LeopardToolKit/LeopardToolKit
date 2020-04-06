using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit
{
    public abstract class BaseDisposable : IDisposable
    {
        private bool disposed = false;
        ~BaseDisposable()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    DisposeManaged();
                }
                DisposeUnmanaged();
            }
            disposed = true;
        }

        /// <summary>
        /// Dispose managed resource
        /// </summary>
        protected abstract void DisposeManaged();

        /// <summary>
        /// Dispose unmanaged resource
        /// </summary>
        protected virtual void DisposeUnmanaged() { }
    }
}
