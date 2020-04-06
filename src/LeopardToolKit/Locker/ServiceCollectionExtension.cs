using LeopardToolKit.Locker;
using LeopardToolKit.Locker.Distribute;
using Microsoft.Extensions.DependencyInjection;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Leopard.Locker
{
    public static partial class ServiceCollectionExtension
    {
        public static IServiceCollection AddProcessLock(this IServiceCollection services)
        {
            services.AddSingleton<ILockFactory, ProcessLockFactory>();
            return services;
        }

        public static IServiceCollection AddDistributeLock(this IServiceCollection services, Action<DistributeLockOption> redisConfigure)
        {
            DistributeLockOption distributeLockOption = new DistributeLockOption() { RedisConnectionStrings=new List<string>() };
            redisConfigure.Invoke(distributeLockOption);
            var multiplexers =  new List<RedLockMultiplexer>();
            foreach (var conn in distributeLockOption.RedisConnectionStrings)
            {
                multiplexers.Add(ConnectionMultiplexer.Connect(conn));
            }
            var redlockFactory = RedLockFactory.Create(multiplexers);
            services.AddSingleton(redlockFactory);
            services.AddSingleton<ILockFactory, DistributeLockFactory>();
            return services;
        }
    }
}
