namespace FlightRecorder.Api.Entities
{
    public class AirportsExportWorkItem : BackgroundWorkItem
    {
        public string FileName { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()}, FileName = {FileName}";
        }
    }
}
