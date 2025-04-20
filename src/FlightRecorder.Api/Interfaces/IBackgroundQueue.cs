using FlightRecorder.Api.Entities;

namespace FlightRecorder.Api.Interfaces
{
    public interface IBackgroundQueue<T> where T : BackgroundWorkItem
    {
        T Dequeue();
        void Enqueue(T item);
    }
}