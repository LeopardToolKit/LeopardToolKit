using LeopardToolKit;
using LeopardToolKit.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ServiceCollectionExtension
    {
        public static IServiceCollection AddMemoryEventBus(this IServiceCollection services, Assembly[] handlerAssemblies)
        {
            services.TryAddSingleton<MemoryEventBusStarter>();
            services.TryAddSingleton<IEventPublish, MemoryEventPublish>();

            ConcurrentDictionary<string, Type> eventHandlerMap = new ConcurrentDictionary<string, Type>();
            foreach (var handlerType in handlerAssemblies.SelectMany(assembly => assembly.GetTypes().Where(type => type.IsClass && typeof(IEventHandler).IsAssignableFrom(type))))
            {
                EventHandlerAttribute eventHandlerAttribute = handlerType.GetCustomAttribute<EventHandlerAttribute>();
                eventHandlerMap.TryAdd(eventHandlerAttribute.ThrowIfNull(nameof(eventHandlerAttribute)).EventName, handlerType);
                services.Add(new ServiceDescriptor(handlerType, handlerType, eventHandlerAttribute.HandlerLifetime));
            }

            services.AddSingleton(sp => new MemoryEventDispatcher(sp, sp.GetRequiredService<ILogger<MemoryEventDispatcher>>(), eventHandlerMap));

            return services;
        }

        public static IServiceProvider StartMemoryEventBus(this IServiceProvider serviceProvider)
        {
            var starter = serviceProvider.GetRequiredService<MemoryEventBusStarter>();
            starter.Start();
            return serviceProvider;
        }

        public static IServiceCollection AddEventHandler<THandler>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Transient) where THandler : class, IEventHandler
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton<THandler>();
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped<THandler>();
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient<THandler>();
                    break;
                default:
                    services.AddTransient<THandler>();
                    break;
            }
            return services;
        }
    }
}
