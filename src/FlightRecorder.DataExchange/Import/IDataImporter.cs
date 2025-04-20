using FlightRecorder.BusinessLogic.Factory;
using System.Threading.Tasks;

namespace FlightRecorder.DataExchange.Import
{
    public interface IDataImporter
    {
        Task Import(string file, FlightRecorderFactory factory);
    }
}
