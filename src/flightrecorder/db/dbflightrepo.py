from flightrecorder.appbase.dbbase import AppBaseRepository
from flightrecorder.db.dbairlinerepo import AirlineRepository
from flightrecorder.db.dbmodels import FlightModel
from flightrecorder.objectmodel.omflight import Flight


class FlightRepository(AppBaseRepository):
    """Repository for addition and querying of Flights"""

    def __init__(self, session):
        super(FlightRepository, self).__init__(session, FlightModel)

    def exists(self, number):
        """Check for flight existence

        :param name: String containing the flight number
        :return: True if the flight exists and False if not
        """
        return super(FlightRepository, self).query_exists("number", number)

    def read(self, column, value):
        """Load and return a specified flight

        :param column: The name of the column to search
        :param value: The value to look for (exact match)
        :return: A Flight object instance for the matching flight, or None if there is no match
        """
        flight = super(FlightRepository, self).query_first(column, value)
        if flight is None:
            return None
        m = AirlineRepository(self._session)
        airline = m.read("id", flight.airline_id)
        return Flight(flight.number, flight.embarkation, flight.destination, airline, flight.id)

    # TODO : Is the "order_by" parameter needed, as it doesn't seem to be used?
    def read_all(self, column, value, order_by):
        """Return all flights matching the specified criteria

        :param column: The name of the column to search
        :param value: The value to look for (exact match)
        :param order_by: The name of the column to order the results by
        :return: A generator that will yield instances of a Flight object for each match
        """
        for record in super(FlightRepository, self).query_all(column, value, "number"):
            yield self.read("id", record.id)

    def query_flights_by_route(self, embarkation, destination):
        """Return all flights matching the  specified route

        :param embarkation: String containing the 3-letter code for the embarkation airport
        :param destination: String containing the 3-letter code for the destination airport
        :return: A generator to return all matching Flight instances
        """
        if embarkation is not None and destination is None:
            for flight in super(FlightRepository, self).query_all("embarkation", embarkation, "id"):
                yield self.read("id", flight.id)
        elif embarkation is None and destination is not None:
            for flight in super(FlightRepository, self).query_all("destination", destination, "id"):
                yield self.read("id", flight.id)
        else:
            for flight in self._session.query(FlightModel).filter(FlightModel.embarkation == embarkation)\
                    .filter(FlightModel.destination == destination).all():
                yield self.read("id", flight.id)

    def create(self, number, embarkation, destination, airline):
        """Create a new flight or return an existing matching flight (based on the flight number)

        :param number: String containing the flight number
        :param embarkation: String containing the 3-letter code for the embarkation airport
        :param destination: String containing the 3-letter code for the destination airport
        :param airline: String containing the airline name
        :return: A Flight object representing the flight
        """
        if not self.exists(number):
            m = AirlineRepository(self._session)
            airline_id = m.create(airline).db_id()
            flight = super(FlightRepository, self).insert(FlightModel(number=number, embarkation=embarkation,
                                                                      destination=destination, airline_id=airline_id))
            return self.read("id", flight.id)
        else:
            return self.read("number", number)
