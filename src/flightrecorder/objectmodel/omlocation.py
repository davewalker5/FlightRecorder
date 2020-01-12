from flightrecorder.appbase.ombase import AppBaseNamedEntity


class Location(AppBaseNamedEntity):
    """Class to represent an observation location"""

    def __init__(self, name, db_id=None):
        """Object initialisation

        :param name: String containing the location name
        :param db_id: Primary key for the database record for this location
        """
        # Storage of the name and database ID are handled by the superclass
        super(Location, self).__init__(name, db_id)
