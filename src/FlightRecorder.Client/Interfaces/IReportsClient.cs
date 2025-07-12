using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlightRecorder.Entities.Db;
using FlightRecorder.Entities.Reporting;

namespace FlightRecorder.Client.Interfaces
{
    public interface IReportsClient
    {
        Task<SightingStatistics> SightingStatisticsAsync();
        Task<List<AirlineStatistics>> AirlineStatisticsAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize);
        Task<List<LocationStatistics>> LocationStatisticsAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize);
        Task<List<ManufacturerStatistics>> ManufacturerStatisticsAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize);
        Task<List<ModelStatistics>> ModelStatisticsAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize);
        Task<List<FlightsByMonth>> FlightsByMonthAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize);
        Task<List<JobStatus>> JobStatusAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize);
        Task<List<MyFlights>> MyFlightsAsync(DateTime? from, DateTime? to, int pageNumber, int pageSize);
        
    }
}
