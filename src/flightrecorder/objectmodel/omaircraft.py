from flightrecorder.appbase.ombase import AppBaseEntity
from flightrecorder.objectmodel.ommodel import Model
import datetime as datetime


class Aircraft(AppBaseEntity):
    """Class to represent an aircraft"""

    EARLIEST_YEAR_OF_MANUFACTURE = 1903

    PROPERTY_REGISTRATION = "registration"
    PROPERTY_SERIAL_NUMBER = "serial_number"
    PROPERTY_MANUFACTURED = "manufactured"
    PROPERTY_AGE = "age"
    PROPERTY_MODEL = "model"

    def __init__(self, registration, serial_number, manufactured, model, db_id=None):
        """Object Initialisation

        :param registration: String containing the registration number of the aircraft
        :param serial_number: String containing the serial number of the aircraft
        :param manufactured: Year of manufacture of the aircraft, as an integer
        :param model: An instance of the Model class
        :param db_id: Primary key for the database record for this aircraft
        """
        if not isinstance(model, Model):
            raise ValueError("Aircraft requires an instance of Model")

        if not isinstance(manufactured, int):
            raise ValueError("Year of manufacture must be an integer")

        this_year = datetime.datetime.now().year
        if manufactured < Aircraft.EARLIEST_YEAR_OF_MANUFACTURE or manufactured > this_year:
            raise ValueError("Year of manufacture must be between {} and {} : supplied {}".format(
                Aircraft.EARLIEST_YEAR_OF_MANUFACTURE, this_year, manufactured))

        if registration is None or len(registration) == 0:
            raise ValueError("Registration cannot be None or empty")

        if serial_number is None or len(serial_number) == 0:
            raise ValueError("Serial number cannot be None or empty")

        # Storage of the database ID is handled by the superclass
        super(Aircraft, self).__init__(db_id)
        self.set_property(Aircraft.PROPERTY_REGISTRATION, registration)
        self.set_property(Aircraft.PROPERTY_SERIAL_NUMBER, serial_number)
        self.set_property(Aircraft.PROPERTY_MANUFACTURED, manufactured)
        self.set_property(Aircraft.PROPERTY_AGE, this_year - manufactured)
        self.set_property(Aircraft.PROPERTY_MODEL, model)

    def registration(self):
        """Return the aircraft registration number"""
        return self.get_property(Aircraft.PROPERTY_REGISTRATION)

    def serial_number(self):
        """Return the aircraft serial number"""
        return self.get_property(Aircraft.PROPERTY_SERIAL_NUMBER)

    def manufactured(self):
        """Return the year of manufacture"""
        return self.get_property(Aircraft.PROPERTY_MANUFACTURED)

    def age(self):
        """Return the age to the aircraft"""
        return self.get_property(Aircraft.PROPERTY_AGE)

    def model(self):
        """Return the aircraft model, that is an instance of the Model class"""
        return self.get_property(Aircraft.PROPERTY_MODEL)
