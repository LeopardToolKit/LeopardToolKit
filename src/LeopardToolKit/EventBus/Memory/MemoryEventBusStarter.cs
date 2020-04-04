using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeopardToolKit.EventBus
{
    internal class MemoryEventBusStarter
    {
        private readonly MemoryEventDispatcher memoryEventDispatcher;

        public MemoryEventBusStarter(MemoryEventDispatcher memoryEventDispatcher)
        {
            this.memoryEventDispatcher = memoryEventDispatcher;
        }
        public void Start()
        {
            this.memoryEventDispatcher.Start();
        }
    }
}
