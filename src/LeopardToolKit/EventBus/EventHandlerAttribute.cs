using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.EventBus
{
    public class EventHandlerAttribute : Attribute
    {
        public string EventName { get; private set; }

        public ServiceLifetime HandlerLifetime { get; set; }

        public EventHandlerAttribute(string eventName, ServiceLifetime handlerLifetime = ServiceLifetime.Transient)
        {
            this.EventName = eventName;
            this.HandlerLifetime = handlerLifetime;
        }
    }
}
