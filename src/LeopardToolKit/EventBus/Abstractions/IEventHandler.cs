using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeopardToolKit.EventBus
{
    public interface IEventHandler
    {
        string EventName { get; }

        Task Execute(object eventData);

        void ExceptionCallback(object eventData, Exception exception);
    }
}
