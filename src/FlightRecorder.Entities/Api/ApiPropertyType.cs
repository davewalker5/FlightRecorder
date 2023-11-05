using System.ComponentModel;

namespace FlightRecorder.Entities.Api
{
    public enum ApiPropertyType
    {
        [Description("Departure Airport")]
        DepartureAirportIATA,

        [Description("Destination Airport")]
        DestinationAirportIATA,

        [Description("Airline")]
        AirlineName,

        [Description("Airport")]
        AirportName,

        [Description("Country")]
        CountryName,

        [Description("Registration")]
        AircraftRegistration,

        [Description("24-bit ICAO Address")]
        AircraftICAOAddress,

        [Description("Serial Number")]
        AircraftSerialNumber,

        [Description("Registration Date")]
        AircraftRegistrationDate,

        [Description("Model")]
        AircraftModel,

        [Description("Model Code")]
        AircraftModelCode,

        [Description("Type")]
        AircraftType,

        [Description("Production Line")]
        AircraftProductionLine,

        [Description("Age")]
        AircraftAge,

        [Description("Manufacturer")]
        ManufacturerName
    }
}
