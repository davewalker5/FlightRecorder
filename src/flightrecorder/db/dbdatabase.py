from flightrecorder.db.dbairlinerepo import AirlineRepository
from flightrecorder.db.dblocationrepo import LocationRepository
from flightrecorder.db.dbmanufacturerrepo import ManufacturerRepository
from flightrecorder.db.dbmodelrepo import ModelRepository
from flightrecorder.db.dbaircraftrepo import AircraftRepository
from flightrecorder.db.dbflightrepo import FlightRepository
from flightrecorder.db.dbsightingrepo import SightingRepository
from flightrecorder.appbase.dbbase import AppBaseRepository
from flightrecorder.db.dbmodels import Base
from flightrecorder.config.configuration import Configuration
from sqlalchemy import create_engine
import datetime
import os


class FlightRecorderDatabase:
    """Flight Recorder Database

    This is the primary class that the application uses to access the Flight Recorder database. It is written
    for SQLAlchemy and provides the following facilities:

    1. Methods to return instances of the entity repositories
    2. Methods for creation of records for the Flight Recorder entities
    3. Methods for querying the data
    """

    def __init__(self):
        # Determine where the database is
        config = Configuration()
        filename = config.database_name()
        database = "sqlite:///" + filename
        self._location = os.path.dirname(filename)

        # Create the database
        engine = create_engine(database)
        Base.metadata.create_all(engine)

        # Establish a session
        from sqlalchemy.orm import sessionmaker
        Base.metadata.bind = engine
        db_session = sessionmaker()
        db_session.bind = engine
        self._session = db_session()

        # Create the repositories
        self._airline_repo = AirlineRepository(self._session)
        self._location_repo = LocationRepository(self._session)
        self._manufacturer_repo = ManufacturerRepository(self._session)
        self._model_repo = ModelRepository(self._session)
        self._aircraft_repo = AircraftRepository(self._session)
        self._flight_repo = FlightRepository(self._session)
        self._sighting_repo = SightingRepository(self._session)

    def location(self):
        """Return the location of the Flight Recorder database file"""
        return self._location

    def airline_repo(self):
        """Return the current instance of the airline repository"""
        return self._airline_repo

    def location_repo(self):
        """Return the current instance of the observation location repository"""
        return self._location_repo

    def manufacturer_repo(self):
        """Return the current instance of the manufacturer repository"""
        return self._manufacturer_repo

    def model_repo(self):
        """Return the current instance of the aircraft model repository"""
        return self._model_repo

    def aircraft_repo(self):
        """Return the current instance of the aircraft repository"""
        return self._aircraft_repo

    def flight_repo(self):
        """Return the current instance of the flight details repository"""
        return self._flight_repo

    def sighting_repo(self):
        """Return the current instance of the sightings, or observations, repository"""
        return self._sighting_repo

    def create_airline(self, name):
        """Create and return a new airline"""
        return self._airline_repo.create(name)

    def create_location(self, name):
        """Create and return a new observation location"""
        return self._location_repo.create(name)

    def create_manufacturer(self, name):
        """Create and return a new manufacturer"""
        return self._manufacturer_repo.create(name)

    def create_model(self, name, manufacturer):
        """Create and return a new model"""
        return self._model_repo.create(name, manufacturer)

    def create_aircraft(self, registration, serial_number, manufactured, model, manufacturer):
        """Create and return a new aircraft"""
        return self._aircraft_repo.create(registration, serial_number, manufactured, model, manufacturer)

    def create_flight(self, number, embarkation, destination, airline):
        """Create and return a new flight"""
        return self._flight_repo.create(number, embarkation, destination, airline)

    def create_sighting(self, altitude, date, location_id, flight_id, aircraft_id):
        """Create and return a new sighting, or observation"""
        return self._sighting_repo.insert(altitude, date, location_id, flight_id, aircraft_id)

    def read_models(self, manufacturer):
        """Return all aircraft models associated with the specified manufacturer

        :param manufacturer: String containing the manufacturer name
        :return: A generator to return all matching Model instances
        """
        return self._model_repo.read_all_for_manufacturer(manufacturer)

    def query_sightings_by_flight(self, flight_number):
        """Return all sightings for a specified flight number

        :param flight_number: A string containing the flight number
        :return: A generator to return all matching Sighting instances
        """
        for flight in self._flight_repo.read_all("number", flight_number, "number"):
            for sighting in self._sighting_repo.read_all("flight_id", flight.db_id(), "date"):
                yield sighting

    def query_sightings_by_aircraft(self, registration):
        """Return all sightings of a specified aircraft

        :param registration: A string containing the aircraft registration
        :return: A generator to return all matching Sighting instances
        """
        aircraft = self._aircraft_repo.read("registration", registration)
        if aircraft is None:
            yield None
        else:
            for sighting in self._sighting_repo.read_all("aircraft_id", aircraft.db_id(), "date"):
                yield sighting

    def query_sightings_by_route(self, embarkation, destination):
        """Return all sightings for aircraft travelling on the specified route

        :param embarkation: String containing the 3-letter code for the embarkation airport
        :param destination: String containing the 3-letter code for the destination airport
        :return: A generator to return all matching Sighting instances
        """
        for flight in self.flight_repo().query_flights_by_route(embarkation, destination):
            for sighting in self.sighting_repo().read_all("flight_id", flight.db_id(), "id"):
                yield sighting

    def query_sightings_by_airline(self, airline):
        """Return all sightings for a specified airline

        :param airline: String containing the airline name
        :return: A generator to return all matching Sighting instances
        """
        airline = self._airline_repo.read("name", airline)
        if airline is None:
            yield None
        else:
            for flight in self._flight_repo.read_all("airline_id", airline.db_id(), "id"):
                for sighting in self._sighting_repo.read_all("flight_id", flight.db_id(), "date"):
                    yield sighting

    def query_sightings_by_location(self, location):
        """Return  all sightings made at the specified location

        :param location: String containing the location name
        :return: A generator to return all matching Sighting instances
        """
        location = self._location_repo.read("name", location)
        if location is None:
            yield None
        else:
            for sighting in self._sighting_repo.read_all("location_id", location.db_id(), "date"):
                yield sighting

    def _test_record_creation(self):
        """Method to test creation of a new record

        This method exercises all of the entity repositories. It should be used during development only.

        :return: An instance of the Sighting class
        """
        aircraft_id = self.create_aircraft("G-EZPI", "7104", 3, "A320-214", "Airbus").db_id()
        flight_id = self.create_flight("U21811", "MAN", "BSL", "EasyJet").db_id()
        location_id = self.create_location("Abingdon")
        self.create_sighting(33000, datetime.datetime.now(), location_id, flight_id, aircraft_id)


if __name__ == "__main__":
    AppBaseRepository()
