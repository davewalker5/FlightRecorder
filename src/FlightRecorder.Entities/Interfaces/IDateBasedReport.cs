using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IDateBasedReport<T> where T : class
    {
        Task<IEnumerable<T>> GenerateReportAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize);
    }
}