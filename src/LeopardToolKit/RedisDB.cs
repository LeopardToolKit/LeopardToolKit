using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeopardToolKit
{
    /// <summary>
    /// This Helper class for redis operation
    /// </summary>
    public class RedisDB : IDisposable
    {
        private readonly ConnectionMultiplexer redis = null;

        private readonly JsonSerializerSettings DefaultSettings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };

        public RedisDB(string connectionString)
        {
            var config = ConfigurationOptions.Parse(connectionString);
            config.ConnectRetry = 3;
            config.SyncTimeout = 10;
            config.ConnectTimeout = 15000;
            redis = ConnectionMultiplexer.Connect(connectionString);
        }


        #region Common
        public void SetExpireTime(int databaseNum, string key, TimeSpan? expireTime = null)
        {
            ExecuteRedisOperation(databaseNum, db =>
            {
                return db.KeyExpire(key, expireTime);
            });
        }

        public bool Exists(int databaseNum, string key)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                return db.KeyExists(key);
            });
        }

        public bool Remove(int databaseNum, string key)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                return db.KeyDelete(key);
            });
        }

        public long Remove(int databaseNum, string[] keys)
        {
            RedisKey[] redisKeys = ConvertToRedisKeys(keys);
            return ExecuteRedisOperation(databaseNum, db =>
            {
                return db.KeyDelete(redisKeys);
            });
        }

        #endregion

        #region Key -> Value
        /// <summary>
        /// Add Or Update
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="databaseNum"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        public bool Set<T>(int databaseNum, string key, T value, TimeSpan? expireTime = null)
        {
            RedisValue redisValue = ConvertToRedisValue(value);
            return ExecuteRedisOperation(databaseNum, db =>
            {
                return db.StringSet(key, redisValue, expireTime);
            });
        }

        public T Get<T>(int databaseNum, string key)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                var redisValue = db.StringGet(key);
                return ConvertToObject<T>(redisValue);
            });
        }
        #endregion

        #region List
        public void AddToList<T>(int databaseNum, string key, T value)
        {
            var redisValue = ConvertToRedisValue(value);
            ExecuteRedisOperation(databaseNum, db =>
            {
                return db.ListLeftPush(key, redisValue);
            });
        }

        public void AddToEndOfList<T>(int databaseNum, string key, T value)
        {
            var redisValue = ConvertToRedisValue(value);
            ExecuteRedisOperation(databaseNum, db =>
            {
                return db.ListRightPush(key, redisValue);
            });
        }

        public void AddArrayToList<T>(int databaseNum, string key, IEnumerable<T> values)
        {
            var redisValues = ConvertToRedisValueArray(values);
            ExecuteRedisOperation(databaseNum, db =>
            {
                return db.ListLeftPush(key, redisValues);
            });
        }

        public void AddArrayToEndOfList<T>(int databaseNum, string key, IEnumerable<T> values)
        {
            var redisValues = ConvertToRedisValueArray(values);
            ExecuteRedisOperation(databaseNum, db =>
            {
                return db.ListRightPush(key, redisValues);
            });
        }

        public long LengthOfList(int databaseNum, string key)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                return db.ListLength(key);
            });
        }

        public long RemoveFromList<T>(int databaseNum, string key, T value)
        {
            var redisValue = ConvertToRedisValue(value);
            return ExecuteRedisOperation(databaseNum, db =>
            {
                return db.ListRemove(key, redisValue);
            });
        }

        public T[] GetAllFromList<T>(int databaseNum, string key, long start = 0, long stop = -1)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                var redisValues = db.ListRange(key, start, stop);
                return ConvertToObjectArray<T>(redisValues);
            });
        }

        public T PopFromList<T>(int databaseNum, string key)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                var redisValue = db.ListRightPop(key);
                return ConvertToObject<T>(redisValue);
            });
        }
        #endregion

        #region Queue
        public void Enqueue<T>(int databaseNum, string key, T value)
        {
            var redisValue = ConvertToRedisValue(value);
            ExecuteRedisOperation(databaseNum, db =>
            {
                return db.ListLeftPush(key, redisValue);
            });
        }

        public void Enqueue<T>(int databaseNum, string key, T[] value)
        {
            var redisValue = ConvertToRedisValueArray(value);
            ExecuteRedisOperation(databaseNum, db =>
            {
                return db.ListLeftPush(key, redisValue);
            });
        }

        public T Dequeue<T>(int databaseNum, string key)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                var redisValue = db.ListRightPop(key);
                return ConvertToObject<T>(redisValue);
            });
        }

        public long LengthOfQueue(int databaseNum, string key)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                return db.ListLength(key);
            });
        }

        public T PopFromSourcePushToDestinationQueue<T>(int databaseNum, string sourceKey, string destinationKey)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                var redisValue = db.ListRightPopLeftPush(sourceKey, destinationKey);
                return ConvertToObject<T>(redisValue);
            });
        }
        #endregion

        #region Stack
        public void PushToStack<T>(int databaseNum, string key, T value)
        {
            var redisValue = ConvertToRedisValue(value);
            ExecuteRedisOperation(databaseNum, db =>
            {
                return db.ListLeftPush(key, redisValue);
            });
        }

        public T PopFromStack<T>(int databaseNum, string key)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                var redisValue = db.ListLeftPop(key);
                return ConvertToObject<T>(redisValue);
            });
        }

        public long LengthOfStack(int databaseNum, string key)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                return db.SetLength(key);
            });
        }
        #endregion

        #region Seed
        public long SeedIncrement(int databaseNum, string key, long step = 1)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                return db.StringIncrement(key, step);
            });
        }

        public long SeedDecrement(int databaseNum, string key, long step = 1)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                return db.StringDecrement(key, step);
            });
        }
        #endregion

        #region Set
        public bool AddToSet<T>(int databaseNum, string key, T value)
        {
            var redisValue = ConvertToRedisValue(value);

            return ExecuteRedisOperation(databaseNum, db =>
            {
                return db.SetAdd(key, redisValue);
            });
        }

        public long AddToSet<T>(int databaseNum, string key, T[] values)
        {
            var redisValues = ConvertToRedisValueArray(values);

            return ExecuteRedisOperation(databaseNum, db =>
            {
                return db.SetAdd(key, redisValues);
            });
        }

        public T[] GetAllFromSet<T>(int databaseNum, string key)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                var redisValues = db.SetMembers(key);
                return ConvertToObjectArray<T>(redisValues);
            });
        }

        public T PopFromSet<T>(int databaseNum, string key)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                var redisValue = db.SetPop(key);
                return ConvertToObject<T>(redisValue);
            });
        }

        public bool RemoveFromSet<T>(int databaseNum, string key, T value)
        {
            RedisValue redisValue = ConvertToRedisValue(value);
            return ExecuteRedisOperation(databaseNum, db =>
            {
                return db.SetRemove(key, redisValue);
            });
        }

        public long RemoveFromSet<T>(int databaseNum, string key, T[] values)
        {
            RedisValue[] redisValues = ConvertToRedisValueArray(values);
            return ExecuteRedisOperation(databaseNum, db =>
            {
                return db.SetRemove(key, redisValues);
            });
        }

        public long LengthOfSet(int databaseNum, string key)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                return db.SetLength(key);
            });
        }
        #endregion

        #region Hash
        public T GetValueFromHashByField<T>(int databaseNum, string key, string field)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                var redisValue = db.HashGet(key, field);
                return ConvertToObject<T>(redisValue);
            });
        }

        public long LengthOfHash(int databaseNum, string key)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                return db.HashLength(key);
            });
        }

        public bool AddOrUpdateToHash<T>(int databaseNum, string key, string field, T value)
        {
            var redisValue = ConvertToRedisValue(value);

            return ExecuteRedisOperation(databaseNum, db =>
            {
                return db.HashSet(key, field, redisValue);
            });
        }

        public bool AddOrUpdateToHash<T>(int databaseNum, string key, KeyValuePair<string, T> entry)
        {
            return AddOrUpdateToHash(databaseNum, key, entry.Key, entry.Value);
        }

        public void AddOrUpdateToHash<T>(int databaseNum, string key, IEnumerable<KeyValuePair<string, T>> entries)
        {
            var values = entries.Select(e => new HashEntry(e.Key, ConvertToRedisValue(e.Value))).ToArray();

            ExecuteRedisOperation(databaseNum, db =>
            {
                db.HashSet(key, values);
                return 0;
            });
        }

        public bool ExistInHash(int databaseNum, string key, string field)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                return db.HashExists(key, field);
            });
        }

        public long DeleteFromHash(int databaseNum, string key, string[] fields)
        {
            var redisValues = ConvertToRedisValueArray(fields);

            return ExecuteRedisOperation(databaseNum, db =>
            {
                return db.HashDelete(key, redisValues);
            });
        }

        public bool DeleteFromHash(int databaseNum, string key, string field)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                return db.HashDelete(key, field);
            });
        }

        public string[] GetAllFieldFromHash(int databaseNum, string key)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                var redisValues = db.HashKeys(key);
                return ConvertToObjectArray<string>(redisValues);
            });
        }

        public List<KeyValuePair<string, T>> GetAllFromHash<T>(int databaseNum, string key)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                List<KeyValuePair<string, T>> result = new List<KeyValuePair<string, T>>();
                var allEntry = db.HashGetAll(key);
                foreach (var entry in allEntry)
                {
                    string field = entry.Name;
                    T entity = ConvertToObject<T>(entry.Value);
                    result.Add(new KeyValuePair<string, T>(field, entity));
                }
                return result;
            });
        }

        public long DeleteFromHashByFieldPattern(int databaseNum, string key, string fieldPattern)
        {
            return ExecuteRedisOperation(databaseNum, db =>
            {
                var redisValues = db.HashScan(key, fieldPattern + "*", 1000).Select(entry => entry.Name).ToArray();
                return db.HashDelete(key, redisValues);
            });
        }
        #endregion

        #region Subscribe
        public void Subscribe<TMessage>(string channel, Action<RedisChannel, TMessage> callback)
        {
            var sub = redis.GetSubscriber();
            sub.Subscribe(channel, (cha, val) => {
                var message = ConvertToObject<TMessage>(val);
                callback?.Invoke(cha, message);
            });
        }

        public void UnSubscribe(string channel)
        {
            var sub = redis.GetSubscriber();
            sub.Unsubscribe(channel);
        }

        public long Publish<TMessage>(string channel, TMessage message)
        {
            var sub = redis.GetSubscriber();
            return sub.Publish(channel, ConvertToRedisValue(message));
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            redis?.Dispose();
        }
        #endregion

        #region private methods
        private RedisValue ConvertToRedisValue<T>(T value)
        {
            RedisValue result = value is string ? value.ToString() :
                value.ToNewtonsoftJson(DefaultSettings);
            return result;
        }

        private RedisKey[] ConvertToRedisKeys(string[] keys)
        {
            return keys.Select(key => (RedisKey)key).ToArray();
        }

        private RedisValue[] ConvertToRedisValueArray<T>(IEnumerable<T> values)
        {
            var aray = values.ToArray();
            RedisValue[] redisValues = new RedisValue[aray.Length];
            for (int i = 0; i < aray.Length; i++)
            {
                redisValues[i] = ConvertToRedisValue(aray[i]);
            }
            return redisValues;
        }

        private T ConvertToObject<T>(RedisValue redisValue)
        {
            if (typeof(T) == typeof(string))
            {
                object obj = (string)redisValue;
                return (T)obj;
            }
            return redisValue.IsNullOrEmpty ? default(T) : ((string)redisValue).DeserializeFromJson<T>(DefaultSettings);
        }

        private T[] ConvertToObjectArray<T>(IEnumerable<RedisValue> redisValues)
        {
            var aray = redisValues.ToArray();
            T[] values = new T[aray.Length];
            for (int i = 0; i < aray.Length; i++)
            {
                values[i] = ConvertToObject<T>(aray[i]);
            }
            return values;
        }

        private TResult ExecuteRedisOperation<TResult>(int databaseNum, Func<IDatabase, TResult> func)
        {
            var database = redis.GetDatabase(databaseNum);
            int retryCount = 0;
            while (true)
            {
                try
                {
                    return func.Invoke(database);
                }
                catch (RedisTimeoutException)
                {
                    if (retryCount == 3)
                    {
                        throw;
                    }
                    else
                    {
                        retryCount++;
                    }
                }
            }
        }
        #endregion
    }
}
