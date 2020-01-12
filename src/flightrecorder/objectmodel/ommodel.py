from flightrecorder.appbase.ombase import AppBaseNamedEntity
from flightrecorder.objectmodel.ommanufacturer import Manufacturer


class Model(AppBaseNamedEntity):
    """Class to represent an aircraft model"""

    PROPERTY_MANUFACTURER = "manufacturer"

    def __init__(self, name, manufacturer, db_id=None):
        """Object initialisation

        :param name: String containing the model name
        :param manufacturer: String containing the manufacturer name
        :param db_id: Primary key for the database record for this model
        """

        # Storage of the name and database ID are handled by the superclass
        super(Model, self).__init__(name, db_id)

        if not isinstance(manufacturer, Manufacturer):
            raise ValueError("Model requires an instance of Manufacturer")

        self.set_property(Model.PROPERTY_MANUFACTURER, manufacturer)

    def manufacturer(self):
        """Return the manufacturer, that is an instance of the Manufacturer class"""
        return self.get_property(Model.PROPERTY_MANUFACTURER)
