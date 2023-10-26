using FlightRecorder.Api.Entities;

namespace FlightRecorder.BusinessLogic.Logic
{
    public interface IBackgroundQueue<T> where T : BackgroundWorkItem
    {
        T Dequeue();
        void Enqueue(T item);
    }
}