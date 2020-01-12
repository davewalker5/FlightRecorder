from flightrecorder.appbase.constants import *
import datetime as datetime


def safe_get_value(properties, key):
    if key in properties:
        return properties[key]
    else:
        return None


def safe_get_date(properties, key):
    date = safe_get_value(properties, key)
    if date is not None:
        return datetime.datetime.strptime(date[:10], DATE_FORMAT)
    else:
        return None


class AppBaseEntity:
    """Base class for all application entities. Holds the database ID for subclass instances"""

    PROPERTY_DB_ID = "db_id"

    def __init__(self, db_id):
        """Object initialisation

        :param db_id: Primary key for the database record for this entity
        """
        self._properties = {}
        self.set_property(AppBaseEntity.PROPERTY_DB_ID, db_id)

    def set_property(self, key, value):
        self._properties.update({key: value})

    def get_property(self, key):
        return safe_get_value(self._properties, key)

    def db_id(self):
        """Return the database ID for this entity"""
        return self.get_property(AppBaseEntity.PROPERTY_DB_ID)


class AppBaseNamedEntity(AppBaseEntity):
    """Base class for named application entities, that support the name() method. Maintains the name in
    state and relies on the base class, BaseEntity, to maintain the database ID.
    """

    PROPERTY_NAME = "name"

    def __init__(self, name, db_id=None):
        """Object initialisation

        :param name: String containing the name associated with the object
        :param db_id: Primary key for the database record for this entity
        """
        if name is None or len(name) == 0:
            raise ValueError("Name cannot be None or empty")

        # Storage of the database ID is handled by the superclass
        super(AppBaseNamedEntity, self).__init__(db_id)
        self.set_property(AppBaseNamedEntity.PROPERTY_NAME, name)

    def name(self):
        """Return the name associated with the current instance"""
        return self.get_property(AppBaseNamedEntity.PROPERTY_NAME)
