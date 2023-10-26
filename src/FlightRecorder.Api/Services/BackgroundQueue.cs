using FlightRecorder.Api.Entities;
using System;
using System.Collections.Concurrent;

namespace FlightRecorder.BusinessLogic.Logic
{
    public class BackgroundQueue<T> : IBackgroundQueue<T> where T : BackgroundWorkItem
    {
        private readonly ConcurrentQueue<T> _queue = new();

        /// <summary>
        /// Add a new item to the concurrent queue
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Enqueue(T item)
        {
            if (item != null)
            {
                _queue.Enqueue(item);
            }
            else
            {
                throw new ArgumentNullException(nameof(item));
            }
        }

        /// <summary>
        /// De-queue an item
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            var successful = _queue.TryDequeue(out T item);
            return successful ? item : null;
        }
    }
}
