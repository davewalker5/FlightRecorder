from flightrecorder.appbase.ombase import AppBaseNamedEntity


class Airline(AppBaseNamedEntity):
    """Class to represent an airline"""

    def __init__(self, name, db_id=None):
        """Object Initialisation

        :param name: String containing the airline name
        :param db_id: Primary key for the database record for this airline
        """
        # Storage of the name and database ID are handled by the superclass
        super(Airline, self).__init__(name, db_id)
