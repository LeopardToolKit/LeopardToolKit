using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace LeopardToolKit.EventBus
{
    public class MemoryEventDispatcher : IDisposable
    {
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private readonly ConcurrentDictionary<string, Type> EventHandlerMap = new ConcurrentDictionary<string, Type>();
        private readonly ConcurrentQueue<EventMessage> MessageQueue = new ConcurrentQueue<EventMessage>();
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<MemoryEventDispatcher> logger;

        public MemoryEventDispatcher(IServiceProvider serviceProvider, ILogger<MemoryEventDispatcher> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            var assemblyTypes = AppDomain.CurrentDomain.GetAssemblies();
            using (var scope = this.serviceProvider.CreateScope())
            {
                foreach (var handlerType in assemblyTypes.SelectMany(ass => ass.GetTypes().Where(type => type.IsClass && typeof(IEventHandler).IsAssignableFrom(type))))
                {
                    IEventHandler eventHandler = (IEventHandler)scope.ServiceProvider.GetRequiredService(handlerType);
                    EventHandlerMap.TryAdd(eventHandler.EventName, eventHandler.GetType());
                }

            }
        }

        public void Dispose()
        {
            cts.Cancel();
        }

        public void EnqueueMessage(string eventName, object eventData)
        {
            MessageQueue.Enqueue(new EventMessage() { Name = eventName, Data = eventData });
        }

        internal void Start()
        {
            Task.Factory.StartNew(Execute, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private async Task Execute()
        {
            while (!cts.IsCancellationRequested)
            {
                EventMessage eventMessage = null; ;
                IEventHandler eventSubcribe = null; ;
                try
                {
                    if (MessageQueue.TryDequeue(out eventMessage))
                    {
                        if (EventHandlerMap.TryGetValue(eventMessage.Name, out Type eventHandler))
                        {
                            using (var scope = this.serviceProvider.CreateScope())
                            {
                                eventSubcribe = (IEventHandler)scope.ServiceProvider.GetRequiredService(eventHandler);
                                await eventSubcribe.Execute(eventMessage.Data);
                            }
                        }
                    }
                    else
                    {
                        await Task.Delay(1000);
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex,"Execute MemoryEvent error");
                    try{eventSubcribe?.ExceptionCallback(eventMessage?.Data, ex);}catch{}
                }

            }
        }
    }
}
