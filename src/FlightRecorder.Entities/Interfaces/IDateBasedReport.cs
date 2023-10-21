using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightRecorder.Entities.Interfaces
{
    public interface IDateBasedReport<T> where T : class
    {
        Task<List<T>> GenerateReport(DateTime? from, DateTime? to);
    }
}