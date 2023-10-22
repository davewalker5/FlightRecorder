using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IDateBasedReport<T> where T : class
    {
        Task<IEnumerable<T>> GenerateReport(DateTime? from, DateTime? to, int pageNumber, int pageSize);
    }
}