using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeopardToolKit.EventBus
{
    public interface IEventPublish
    {
        void Publish(string eventName, object eventData);
    }
}
