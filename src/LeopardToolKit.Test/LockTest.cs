using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LeopardToolKit.Locker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Leopard.Locker.Test
{
    [TestClass]
    public class LockTest
    {
        private IServiceProvider Init(string lockType)
        {
            ServiceCollection services = new ServiceCollection();
            if(lockType == "Process")
            {
                services.AddProcessLock();
            }
            else
            {
                services.AddDistributeLock(config=> config.RedisConnectionStrings.Add("localhost:6379"));
            }

            return services.BuildServiceProvider();
        }

        [TestMethod]
        public void ProcessLockDifferentLockNameTest()
        {
            ILockFactory processLockFactory = Init("Process").GetRequiredService<ILockFactory>();
            List<Task> tasks = new List<Task>();
            int sum = 0;
            for (int i = 0; i < 1000; i++)
            {
                Task t = Task.Run(() => {
                    using (ILock pLock = processLockFactory.GetLock(Guid.NewGuid().ToString()))
                    {
                        if (pLock.Wait())
                        {
                            sum++;
                        }
                    }
                });

                tasks.Add(t);
            }

            Task.WaitAll(tasks.ToArray());
            Assert.AreNotEqual(1000, sum);
        }

        [TestMethod]
        public void ProcessLockSameLockNameTest()
        {
            ILockFactory processLockFactory = Init("Process").GetRequiredService<ILockFactory>();
            List<Task> tasks = new List<Task>();
            int sum = 0;
            string lockName = "ProcessLockSameLockNameTest";
            for (int i = 0; i < 10000; i++)
            {
                Task t = Task.Run(() => {
                    using (ILock pLock = processLockFactory.GetLock(lockName))
                    {
                        if (pLock.Wait())
                        {
                            sum++;
                        }
                    }
                });

                tasks.Add(t);
            }

            Task.WaitAll(tasks.ToArray());
            Assert.AreEqual(10000, sum);
        }

        [TestMethod]
        public async Task ProcessLockSameLockNameTestAsync()
        {
            ILockFactory processLockFactory = Init("Process").GetRequiredService<ILockFactory>();
            List<Task> tasks = new List<Task>();
            int sum = 0;
            string lockName = "ProcessLockSameLockNameTest";
            for (int i = 0; i < 10000; i++)
            {
                Task t = Task.Run(async () => {
                    using (ILock pLock = processLockFactory.GetLock(lockName))
                    {
                        if (await pLock.WaitAsync())
                        {
                            sum++;
                        }
                    }
                });

                tasks.Add(t);
            }

            await Task.WhenAll(tasks.ToArray());
            Assert.AreEqual(10000, sum);
        }

        [TestMethod]
        public async Task ProcessLockDifferentLockNameTestAsync()
        {
            ILockFactory processLockFactory = Init("Process").GetRequiredService<ILockFactory>();
            List<Task> tasks = new List<Task>();
            int sum = 0;
            for (int i = 0; i < 10000; i++)
            {
                Task t = Task.Run(async () => {
                    using (ILock pLock = processLockFactory.GetLock(Guid.NewGuid().ToString()))
                    {
                        if (await pLock.WaitAsync())
                        {
                            sum++;
                        }
                    }
                });

                tasks.Add(t);
            }

            await Task.WhenAll(tasks.ToArray());
            Assert.AreNotEqual(10000, sum);
        }

        [TestMethod]
        public void ProcessLockShouldTimeout()
        {
            ILockFactory processLockFactory = Init("Process").GetRequiredService<ILockFactory>();
            string lockName = "ProcessLockSameLockNameTest";
            bool getLock = true;
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 2; i++)
            {
                Task t = Task.Run(() =>
                {
                    using (ILock pLock = processLockFactory.GetLock(lockName))
                    {
                        getLock = pLock.Wait(TimeSpan.FromSeconds(1));
                        if (getLock)
                        {
                            System.Threading.Thread.Sleep(3000);
                        }
                    }
                });

                tasks.Add(t);
            }

            Task.WaitAll(tasks.ToArray());
            Assert.IsFalse(getLock);

            using (ILock pLock = processLockFactory.GetLock(lockName))
            {
                getLock = pLock.Wait(TimeSpan.FromSeconds(3));
            }
            Assert.IsTrue(getLock);

        }

        [TestMethod]
        public void ProcessLockShouldReleaseOnce()
        {
            ILockFactory processLockFactory = Init("Process").GetRequiredService<ILockFactory>();
            string lockName = "ProcessLockSameLockNameTest";
            bool getLock = true;
            List<Task> tasks = new List<Task>();
            int sum = 0;
            for (int i = 0; i < 100; i++)
            {
                Task t = Task.Run(() =>
                {
                    using (ILock pLock = processLockFactory.GetLock(lockName))
                    {
                        getLock = pLock.Wait();
                        if (getLock)
                        {
                            sum++;
                        }
                        pLock.Release();
                    }
                });

                tasks.Add(t);
            }

            Task.WaitAll(tasks.ToArray());
            Assert.AreEqual(100, sum);
        }

        [TestMethod]
        [Ignore("Comment this when your environment has installed redis")]
        public void DistributeLockDifferentLockNameTest()
        {
            ILockFactory processLockFactory = Init("Distribute").GetRequiredService<ILockFactory>();
            List<Task> tasks = new List<Task>();
            int sum = 0;
            for (int i = 0; i < 10000; i++)
            {
                Task t = Task.Run(() => {
                    using (ILock pLock = processLockFactory.GetLock(Guid.NewGuid().ToString()))
                    {
                        if (pLock.Wait())
                        {
                            sum++;
                        }
                    }
                });

                tasks.Add(t);
            }

            Task.WaitAll(tasks.ToArray());
            Assert.AreNotEqual(10000, sum);
        }

        [TestMethod]
        [Ignore("Comment this when your environment has installed redis")]
        public void DistributeLockSameLockNameTest()
        {
            ILockFactory processLockFactory = Init("Distribute").GetRequiredService<ILockFactory>();
            List<Task> tasks = new List<Task>();
            int sum = 0;
            string lockName = "ProcessLockSameLockNameTest";
            for (int i = 0; i < 10000; i++)
            {
                Task t = Task.Run(() => {
                    using (ILock pLock = processLockFactory.GetLock(lockName))
                    {
                        if (pLock.Wait())
                        {
                            sum++;
                        }
                    }
                });

                tasks.Add(t);
            }

            Task.WaitAll(tasks.ToArray());
            Assert.AreEqual(10000, sum);
        }

        [TestMethod]
        [Ignore("Comment this when your environment has installed redis")]
        public async Task DistributeLockSameLockNameTestAsync()
        {
            ILockFactory processLockFactory = Init("Distribute").GetRequiredService<ILockFactory>();
            List<Task> tasks = new List<Task>();
            int sum = 0;
            string lockName = "ProcessLockSameLockNameTest";
            for (int i = 0; i < 1000; i++)
            {
                Task t = Task.Run(async () => {
                    using (ILock pLock = processLockFactory.GetLock(lockName))
                    {
                        if (await pLock.WaitAsync())
                        {
                            sum++;
                        }
                    }
                });

                tasks.Add(t);
            }

            await Task.WhenAll(tasks.ToArray());
            Assert.AreEqual(1000, sum);
        }

        [TestMethod]
        [Ignore("Comment this when your environment has installed redis")]
        public async Task DistributeLockDifferentLockNameTestAsync()
        {
            ILockFactory processLockFactory = Init("Distribute").GetRequiredService<ILockFactory>();
            List<Task> tasks = new List<Task>();
            int sum = 0;
            for (int i = 0; i < 10000; i++)
            {
                Task t = Task.Run(async () => {
                    using (ILock pLock = processLockFactory.GetLock(Guid.NewGuid().ToString()))
                    {
                        if (await pLock.WaitAsync())
                        {
                            sum++;
                        }
                    }
                });

                tasks.Add(t);
            }

            await Task.WhenAll(tasks.ToArray());
            Assert.AreNotEqual(10000, sum);
        }

        [TestMethod]
        [Ignore("Comment this when your environment has installed redis")]
        public void DistributeLockShouldTimeout()
        {
            ILockFactory processLockFactory = Init("Distribute").GetRequiredService<ILockFactory>();
            string lockName = "ProcessLockSameLockNameTest";
            bool getLock = true;
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 2; i++)
            {
                Task t = Task.Run(() =>
                {
                    using (ILock pLock = processLockFactory.GetLock(lockName))
                    {
                        getLock = pLock.Wait(TimeSpan.FromSeconds(1));
                        if (getLock)
                        {
                            System.Threading.Thread.Sleep(3000);
                        }
                    }
                });

                tasks.Add(t);
            }

            Task.WaitAll(tasks.ToArray());
            Assert.IsFalse(getLock);

            using (ILock pLock = processLockFactory.GetLock(lockName))
            {
                getLock = pLock.Wait(TimeSpan.FromSeconds(3));
            }
            Assert.IsTrue(getLock);

        }

        [TestMethod]
        [Ignore("Comment this when your environment has installed redis")]
        public void DistributeLockShouldReleaseOnce()
        {
            ILockFactory processLockFactory = Init("Distribute").GetRequiredService<ILockFactory>();
            string lockName = "ProcessLockSameLockNameTest";
            bool getLock = true;
            List<Task> tasks = new List<Task>();
            int sum = 0;
            for (int i = 0; i < 100; i++)
            {
                Task t = Task.Run(() =>
                {
                    using (ILock pLock = processLockFactory.GetLock(lockName))
                    {
                        getLock = pLock.Wait();
                        if (getLock)
                        {
                            sum++;
                        }
                        pLock.Release();
                    }
                });

                tasks.Add(t);
            }

            Task.WaitAll(tasks.ToArray());
            Assert.AreEqual(100, sum);
        }
    }
}