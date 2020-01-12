from flightrecorder.appbase.dbbase import AppBaseRepository
from flightrecorder.db.dbmodels import LocationModel
from flightrecorder.objectmodel.omlocation import Location


class LocationRepository(AppBaseRepository):
    """Repository for addition and querying of observation locations"""

    def __init__(self, session):
        super(LocationRepository, self).__init__(session, LocationModel)

    def read(self, column, value):
        """Load and return a specified location

        :param column: The name of the column to search
        :param value: The value to look for (exact match)
        :return: A Location object instance for the matching location, or None if there is no match
        """
        location = super(LocationRepository, self).query_first(column, value)
        if location is None:
            return None
        return Location(location.name, location.id)

    def read_all(self):
        """Read all locations in the database

        :return: A generator that will yield instances of a Location object for each location in the database
        """
        for location in super(LocationRepository, self).query_all(None, None, "name"):
            yield Location(location.name, location.id)

    def create(self, name):
        """Create and return a new location or return the existing location if it already exists

        :param name: String containing the name of the location
        :return: A Location object instance for the location
        """
        exists = super(LocationRepository, self).query_exists("name", name)
        if not exists:
            super(LocationRepository, self).insert(LocationModel(name=name))
        return self.read("name", name)