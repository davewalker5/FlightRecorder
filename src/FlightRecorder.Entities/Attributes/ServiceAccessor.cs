using Microsoft.Extensions.DependencyInjection;
using System;

namespace FlightRecorder.Entities.Attributes
{
    public sealed class ServiceAccessor
    {
        private readonly object _lock = new object();
        private static IServiceProvider _provider;

        /// <summary>
        /// Set the instance of IServiceProvider that can be used to resolve instances of specified classes
        /// </summary>
        /// <param name="provider"></param>
        public void SetProvider(IServiceProvider provider)
        {
            lock (_lock)
            {
                if (_provider == null)
                {
                    _provider = provider;
                }
            }
        }

        /// <summary>
        /// Get the registered instance of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>() where T : class
        {
            return _provider.GetService<T>();
        }
    }
}
