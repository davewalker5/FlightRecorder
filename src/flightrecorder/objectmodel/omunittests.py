import unittest
import datetime as datetime
from flightrecorder.objectmodel.omairline import Airline
from flightrecorder.objectmodel.omlocation import Location
from flightrecorder.objectmodel.ommanufacturer import Manufacturer
from flightrecorder.objectmodel.ommodel import Model
from flightrecorder.objectmodel.omaircraft import Aircraft
from flightrecorder.objectmodel.omflight import Flight
from flightrecorder.objectmodel.omsighting import Sighting


class NamedEntityTests(unittest.TestCase):
    def test_airline(self):
        name = "EasyJet"
        airline = Airline(name)
        self.assertEqual(airline.name(), name)

    def test_none_airline(self):
        with self.assertRaises(ValueError):
            Airline(None)

    def test_blank_airline(self):
        with self.assertRaises(ValueError):
            Airline("")

    def test_location(self):
        name = "Abingdon"
        location = Location(name)
        self.assertEqual(location.name(), name)

    def test_none_location(self):
        with self.assertRaises(ValueError):
            Location(None)

    def test_blank_location(self):
        with self.assertRaises(ValueError):
            Location("")

    def test_manufacturer(self):
        name = "Airbus"
        manufacturer = Manufacturer(name)
        self.assertEqual(manufacturer.name(), name)

    def test_none_manufacturer(self):
        with self.assertRaises(ValueError):
            Manufacturer(None)

    def test_blank_manufacturer(self):
        with self.assertRaises(ValueError):
            Manufacturer("")


class AircraftModelTests(unittest.TestCase):
    def setUp(self):
        self._aircraft_model = "A320-214"
        self._manufacturer = Manufacturer("Airbus")

    def test_name(self):
        model = Model(self._aircraft_model, self._manufacturer)
        self.assertEqual(model.name(), self._aircraft_model)

    def test_none_name(self):
        with self.assertRaises(ValueError):
            Model(None, self._manufacturer)

    def test_blank_name(self):
        with self.assertRaises(ValueError):
            Model("", self._manufacturer)

    def test_manufacturer(self):
        model = Model(self._aircraft_model, self._manufacturer)
        self.assertEqual(model.manufacturer(), self._manufacturer)

    def test_none_manufacturer(self):
        with self.assertRaises(ValueError):
            Model(self._aircraft_model, None)

    def test_not_a_manufacturer(self):
        with self.assertRaises(ValueError):
            Model(self._aircraft_model, "Airbus")


class AircraftTests(unittest.TestCase):
    def setUp(self):
        self._manufacturer = Manufacturer("Airbus")
        self._model = Model("A320-214", self._manufacturer)
        self._registration = "G-EZPI"
        self._serial_number = "7104"
        self._this_year = datetime.datetime.now().year
        self._age = 3
        self._manufactured = self._this_year - self._age

    def test_aircraft(self):
        aircraft = Aircraft(self._registration, self._serial_number, self._manufactured, self._model)
        self.assertEqual(aircraft.registration(), self._registration)
        self.assertEqual(aircraft.serial_number(), self._serial_number)
        self.assertEqual(aircraft.age(), self._this_year - self._manufactured)
        self.assertEqual(aircraft.model(), self._model)

    def test_none_model(self):
        with self.assertRaises(ValueError):
            Aircraft(self._registration, self._serial_number, self._manufactured, None)

    def test_not_a_model(self):
        with self.assertRaises(ValueError):
            Aircraft(self._registration, self._serial_number, self._manufactured, "A320-214")

    def test_none_registration(self):
        with self.assertRaises(ValueError):
            Aircraft(None, self._serial_number, self._manufactured, self._model)

    def test_empty_registration(self):
        with self.assertRaises(ValueError):
            Aircraft("", self._serial_number, self._manufactured, self._model)

    def test_none_serial_number(self):
        with self.assertRaises(ValueError):
            Aircraft(self._registration, None, self._manufactured, self._model)

    def test_empty_serial_number(self):
        with self.assertRaises(ValueError):
            Aircraft(self._registration, "", self._manufactured, self._model)

    def test_none_age(self):
        with self.assertRaises(ValueError):
            Aircraft(self._registration, self._serial_number, None, self._model)

    def test_non_integer_age(self):
        with self.assertRaises(ValueError):
            Aircraft(self._registration, self._serial_number, 1.0, self._model)

    def test_invalid_integer_manufactured(self):
        with self.assertRaises(ValueError):
            Aircraft(self._registration, self._serial_number, 1800, self._model)


class FlightTests(unittest.TestCase):
    def setUp(self):
        self._airline = Airline("EasyJet")
        self._number = "U21811"
        self._embarkation = "MAN"
        self._destination = "BSL"

    def test_flight(self):
        flight = Flight(self._number, self._embarkation, self._destination, self._airline)
        self.assertEqual(flight.number(), self._number)
        self.assertEqual(flight.embarkation(), self._embarkation)
        self.assertEqual(flight.destination(), self._destination)
        self.assertEqual(flight.airline(), self._airline)

    def test_none_airline(self):
        with self.assertRaises(ValueError):
            Flight(self._number, self._embarkation, self._destination, None)

    def test_not_an_airline(self):
        with self.assertRaises(ValueError):
            Flight(self._number, self._embarkation, self._destination, "EasyJet")

    def test_invalid_embarkation(self):
        with self.assertRaises(ValueError):
            Flight(self._number, "123", self._destination, self._airline)

    def test_embarkation_too_short(self):
        with self.assertRaises(ValueError):
            Flight(self._number, "MA", self._destination, self._airline)

    def test_embarkation_too_long(self):
        with self.assertRaises(ValueError):
            Flight(self._number, "MANC", self._destination, self._airline)

    def test_invalid_destination(self):
        with self.assertRaises(ValueError):
            Flight(self._number, self._embarkation, "123", self._airline)

    def test_destination_too_short(self):
        with self.assertRaises(ValueError):
            Flight(self._number, self._embarkation, "BS", self._airline)

    def test_destination_too_long(self):
        with self.assertRaises(ValueError):
            Flight(self._number, self._embarkation, "BASL", self._airline)


class SightingTests(unittest.TestCase):
    def setUp(self):
        self._altitude = 33000
        self._date = datetime.datetime.now()
        self._location = Location("Abingdon")
        self._airline = Airline("EasyJet")
        self._flight = Flight("U21811", "MAN", "BSL", self._airline)
        self._manufacturer = Manufacturer("Airbus")
        self._model = Model("A320-214", self._manufacturer)
        self._aircraft = Aircraft("G-EZPI", "7104", 2016, self._model)

    def test_sighting(self):
        sighting = Sighting(self._altitude, self._date, self._location, self._flight, self._aircraft)
        self.assertEqual(sighting.altitude(), self._altitude)
        self.assertEqual(sighting.date(), self._date)
        self.assertEqual(sighting.location(), self._location)
        self.assertEqual(sighting.flight(), self._flight)
        self.assertEqual(sighting.aircraft(), self._aircraft)

    def test_none_altitude(self):
        with self.assertRaises(ValueError):
            Sighting(None, self._date, self._location, self._flight, self._aircraft)

    def test_non_integer_altitude(self):
        with self.assertRaises(ValueError):
            Sighting(33000.0, self._date, self._location, self._flight, self._aircraft)

    def test_invalid_integer_altitude(self):
        with self.assertRaises(ValueError):
            Sighting(-1, self._date, self._location, self._flight, self._aircraft)

    def test_none_date(self):
        with self.assertRaises(ValueError):
            Sighting(self._altitude, None, self._location, self._flight, self._aircraft)

    def test_not_a_date(self):
        with self.assertRaises(ValueError):
            Sighting(self._altitude, "04/09/2019", self._location, self._flight, self._aircraft)

    def test_none_location(self):
        with self.assertRaises(ValueError):
            Sighting(self._altitude, self._date, None, self._flight, self._aircraft)

    def test_not_a_location(self):
        with self.assertRaises(ValueError):
            Sighting(self._altitude, self._date, "Abingdon", self._flight, self._aircraft)

    def test_none_flight(self):
        with self.assertRaises(ValueError):
            Sighting(self._altitude, self._date, self._location, None, self._aircraft)

    def test_not_a_flight(self):
        with self.assertRaises(ValueError):
            Sighting(self._altitude, self._date, self._location, "U21811", self._aircraft)

    def test_none_aircraft(self):
        with self.assertRaises(ValueError):
            Sighting(self._altitude, self._date, self._location, self._flight, None)

    def test_not_an_aircraft(self):
        with self.assertRaises(ValueError):
            Sighting(self._altitude, self._date, self._location, self._flight, "A320-214")


if __name__ == '__main__':
    unittest.main()
