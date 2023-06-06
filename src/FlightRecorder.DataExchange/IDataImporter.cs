using FlightRecorder.BusinessLogic.Factory;

namespace FlightRecorder.DataExchange
{
    public interface IDataImporter
    {
        void Import(string file, FlightRecorderFactory factory);
    }
}
