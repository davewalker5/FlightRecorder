namespace FlightRecorder.BusinessLogic.Logic
{
    public interface IBackgroundQueue<T> where T : class
    {
        T Dequeue();
        void Enqueue(T item);
    }
}