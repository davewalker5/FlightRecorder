from flightrecorder.appbase.ombase import AppBaseNamedEntity


class Manufacturer(AppBaseNamedEntity):
    """Class to represent an aircraft manufacturer"""

    def __init__(self, name, db_id=None):
        """Object initialisation

        :param name: String containing the manufacturer name
        :param db_id: Primary key for the database record for this manufacturer
        """
        # Storage of the name and database ID are handled by the superclass
        super(Manufacturer, self).__init__(name, db_id)
