using LeopardToolKit.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ServiceCollectionExtension
    {
        public static IServiceCollection AddMemoryEventBus(this IServiceCollection services)
        {
            services.TryAddSingleton<MemoryEventDispatcher>();
            services.TryAddSingleton<MemoryEventBusStarter>();
            services.TryAddSingleton<IEventPublish, MemoryEventPublish>();
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
