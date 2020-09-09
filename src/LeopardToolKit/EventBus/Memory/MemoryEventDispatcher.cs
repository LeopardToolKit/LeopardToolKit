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
        private readonly ConcurrentDictionary<string, Type> EventHandlerMap;
        private readonly ConcurrentQueue<EventMessage> MessageQueue = new ConcurrentQueue<EventMessage>();
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<MemoryEventDispatcher> logger;

        private bool disposed = false;

        internal MemoryEventDispatcher(IServiceProvider serviceProvider, ILogger<MemoryEventDispatcher> logger, ConcurrentDictionary<string, Type> eventHandlerMap)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.EventHandlerMap = eventHandlerMap.ThrowIfNull(nameof(eventHandlerMap));
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }
            disposed = true;
            cts.Cancel();
        }

        internal void EnqueueMessage(string eventName, object eventData)
        {
            MessageQueue.Enqueue(new EventMessage() { Name = eventName, Data = eventData });
        }

        public void Start()
        {
            Task.Factory.StartNew(Execute, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void Stop()
        {
            Dispose();
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
                                // Todo: if need run as async
                                await eventSubcribe.Execute(eventMessage.Data).ConfigureAwait(false);
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
