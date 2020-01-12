from flightrecorder.appbase.dbbase import AppBaseRepository
from flightrecorder.db.dbmodels import AirlineModel
from flightrecorder.objectmodel.omairline import Airline


class AirlineRepository(AppBaseRepository):
    """Repository for addition and querying of Airlines"""

    def __init__(self, session):
        super(AirlineRepository, self).__init__(session, AirlineModel)

    def read(self, column, value):
        """Load and return a specified airline

        :param column: The name of the column to search
        :param value: The value to look for (exact match)
        :return: An Airline object instance for the matching airline, or None if there is no match
        """
        airline = super(AirlineRepository, self).query_first(column, value)
        if airline is None:
            return None
        return Airline(airline.name, airline.id)

    def read_all(self):
        """Read all airlines in the database

        :return: A generator that will yield instances of an Airline object for each airline in the database
        """
        for airline in super(AirlineRepository, self).query_all(None, None, "name"):
            yield Airline(airline.name, airline.id)

    def create(self, name):
        """Create and return a new airline or return the existing airline if it already exists

        :param name: String containing the name of the airline
        :return: An Airline object instance for the airline
        """
        exists = super(AirlineRepository, self).query_exists("name", name)
        if not exists:
            super(AirlineRepository, self).insert(AirlineModel(name=name))
        return self.read("name", name)
