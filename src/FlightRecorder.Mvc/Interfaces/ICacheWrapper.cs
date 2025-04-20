using System.Collections.Generic;

namespace FlightRecorder.Mvc.Interfaces
{
    public interface ICacheWrapper
    {
        void Dispose();
        T Get<T>(string key);
        IEnumerable<string> GetKeys();
        IEnumerable<string> GetFilteredKeys(string filter);
        void Remove(string key);
        void Clear();
        T Set<T>(string key, T item, int duration);
    }
}
