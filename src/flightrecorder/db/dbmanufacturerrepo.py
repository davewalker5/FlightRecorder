from flightrecorder.appbase.dbbase import AppBaseRepository
from flightrecorder.db.dbmodels import ManufacturerModel
from flightrecorder.objectmodel.ommanufacturer import Manufacturer


class ManufacturerRepository(AppBaseRepository):
    """Repository for addition and querying of Manufacturers"""

    def __init__(self, session):
        super(ManufacturerRepository, self).__init__(session, ManufacturerModel)

    def exists(self, name):
        """Check for manufacturer existence

        :param name: String containing the manufacturer name
        :return: True if the manufacturer exists and False if not
        """
        return super(ManufacturerRepository, self).query_exists("name", name)

    def read(self, column, value):
        """Load and return a specified manufacturer

        :param column: The name of the column to search
        :param value: The value to look for (exact match)
        :return: A Manufacturer object instance for the matching manufacturer, or None if there is no match
        """
        manufacturer = super(ManufacturerRepository, self).query_first(column, value)
        if manufacturer is None:
            return None
        return Manufacturer(manufacturer.name, manufacturer.id)

    def read_all(self):
        """Read all manufacturers in the database

        :return: A generator that will yield instances of a Manufacturer object for each manufacturer in the database
        """
        for manufacturer in super(ManufacturerRepository, self).query_all(None, None, "name"):
            yield Manufacturer(manufacturer.name, manufacturer.id)

    def create(self, name):
        """Create and return a new manufacturer or return the existing manufacturer if it already exists

        :param name: String containing the name of the manufacturer
        :return: A Manufacturer object instance for the manufacturer
        """
        if not self.exists(name):
            super(ManufacturerRepository, self).insert(ManufacturerModel(name=name))
        return self.read("name", name)
