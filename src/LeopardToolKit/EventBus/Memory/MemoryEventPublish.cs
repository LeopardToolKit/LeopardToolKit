using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeopardToolKit.EventBus
{
    public class MemoryEventPublish : IEventPublish
    {
        private readonly MemoryEventDispatcher memoryEventDispatcher;

        public MemoryEventPublish(MemoryEventDispatcher memoryEventDispatcher)
        {
            this.memoryEventDispatcher = memoryEventDispatcher;
        }

        public void Publish(string eventName, object eventData)
        {
            this.memoryEventDispatcher.EnqueueMessage(eventName, eventData);
        }
    }
}
