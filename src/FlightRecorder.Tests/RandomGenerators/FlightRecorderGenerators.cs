using FlightRecorder.Entities.Db;
using System;
using System.Collections.Generic;

namespace FlightRecorder.Tests.RandomGenerators
{
    public static class FlightRecorderGenerators
    {
        private const int RandomNameLength = 20;
        private const int RandomNameWords = 3;
        
        /// <summary>
        /// Generate a random airline
        /// </summary>
        /// <returns></returns>
        public static Airline GenerateRandomAirline()
        {
            var airline = new Airline { Name = StringGenerators.GenerateRandomPhrase(RandomNameWords) };
            return airline;
        }

        /// <summary>
        /// Generate a random location
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public static Location GenerateRandomLocation()
        {
            var location = new Location { Name = StringGenerators.GenerateRandomWord(RandomNameLength) };
            return location;
        }

        /// <summary>
        /// Generate a random manufacturer
        /// </summary>
        /// <returns></returns>
        public static Manufacturer GenerateRandomManufacturer()
        {
            var location = new Manufacturer { Name = StringGenerators.GenerateRandomPhrase(RandomNameWords) };
            return location;
        }

        /// <summary>
        /// Generate a random country
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public static Country GenerateRandomCountry()
        {
            var country = new Country { Name = StringGenerators.GenerateRandomWord(RandomNameLength) };
            return country;
        }

        /// <summary>
        /// Generate a random model
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        public static Model GenerateRandomModel()
        {
            var model = new Model
            {
                Name = StringGenerators.GenerateRandomPhrase(RandomNameWords),
                Manufacturer = GenerateRandomManufacturer()
            };

            return model;
        }

        /// <summary>
        /// Generate a random aircraft
        /// </summary>
        /// <returns></returns>
        public static Aircraft GenerateRandomAircraft()
        {
            var aircraft = new Aircraft
            {
                Registration = $"G-{StringGenerators.GenerateRandomWord(4)}",
                SerialNumber = StringGenerators.GenerateRandomNumericString(1, 1000000, 7),
                Manufactured = DateGenerators.GenerateRandomYear(2000, 23),
                Model = GenerateRandomModel()
            };

            return aircraft;
        }

        /// <summary>
        /// Generate a random flight
        /// </summary>
        /// <returns></returns>
        public static Flight GenerateRandomFlight()
        {
            var prefix = StringGenerators.GenerateRandomWord(2);
            var suffix = StringGenerators.GenerateRandomNumericString(1, 9999, 4);

            var flight = new Flight
            {
                Number = $"{prefix}{suffix}",
                Embarkation = StringGenerators.GenerateRandomWord(3),
                Destination = StringGenerators.GenerateRandomWord(3),
                Airline = GenerateRandomAirline()
            };

            return flight;
        }

        /// <summary>
        /// Generate a random sighting
        /// </summary>
        /// <returns></returns>
        public static Sighting GenerateRandomSighting()
        {
            var sighting = new Sighting
            {
                Date = DateGenerators.GenerateRandomDate(DateTime.Now.Year - 1),
                Aircraft = GenerateRandomAircraft(),
                Flight = GenerateRandomFlight(),
                Location = GenerateRandomLocation()
            };

            return sighting;
        }

        /// <summary>
        /// Generate a list of randomly generated sightings
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static List<Sighting> GenerateListOfRandomSightings(int number)
        {
            var sightings = new List<Sighting>();

            for (int i = 0; i < number; i++)
            {
                var sighting = GenerateRandomSighting();
                sightings.Add(sighting);
            }

            return sightings;
        }

        /// <summary>
        /// Generate a random airport
        /// </summary>
        /// <returns></returns>
        public static Airport GenerateRandomAirport()
        {
            var airport = new Airport
            {
                Code = StringGenerators.GenerateRandomWord(3),
                Name = StringGenerators.GenerateRandomPhrase(RandomNameWords),
                Country = GenerateRandomCountry()
            };

            return airport;
        }

        /// <summary>
        /// Generate a list of randomly generated airports
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static List<Airport> GenerateListOfRandomAirports(int number)
        {
            var airports = new List<Airport>();

            for (int i = 0; i < number; i++)
            {
                var airport = GenerateRandomAirport();
                airports.Add(airport);
            }

            return airports;
        }
    }
}
