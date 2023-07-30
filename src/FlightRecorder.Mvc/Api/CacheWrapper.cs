using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FlightRecorder.Mvc.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace FlightRecorder.Mvc.Api
{
    public class CacheWrapper : ICacheWrapper, IDisposable
    {
        private readonly MemoryCache _cache;
        private readonly ConcurrentDictionary<string, DateTime> _keys;

        public CacheWrapper(MemoryCacheOptions options)
        {
            _cache = new MemoryCache(options);
            _keys = new ConcurrentDictionary<string, DateTime>();
        }

        public T Set<T>(string key, T item, int duration)
        {
            DateTime expires = DateTime.Now + TimeSpan.FromSeconds(duration);
            _keys[key] = expires;

            // Ensure the item is evicted on expiry and register an eviction callback
            // that will remove the key from _keys
            CancellationTokenSource source = new CancellationTokenSource(TimeSpan.FromSeconds(duration + 1));
            CancellationChangeToken token = new CancellationChangeToken(source.Token);
            MemoryCacheEntryOptions options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(expires)
                .AddExpirationToken(token)
                .RegisterPostEvictionCallback(callback: OnEntryEvicted, state: this);

            return _cache.Set(key, item, options);
        }

        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public void Remove(string key)
        {
            _keys.Remove(key, out DateTime expires);
            _cache.Remove(key);
        }

        public void Clear()
        {
            _cache.Clear();
        }

        public IEnumerable<string> GetKeys()
        {
            return _keys.Where(k => k.Value > DateTime.Now).Select(k => k.Key);
        }

        public IEnumerable<string> GetFilteredKeys(string filter)
        {
            IEnumerable<string> keys = GetKeys();

            if ((filter ?? "").Trim() != "")
            {
                keys = keys.Where(k => k.Contains(filter, StringComparison.OrdinalIgnoreCase));
            }

            return keys;
        }

        private void OnEntryEvicted(object key, object value, EvictionReason reason, object state)
        {
            _keys.Remove(key.ToString(), out DateTime expires);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cache.Dispose();
            }
        }
    }
}
