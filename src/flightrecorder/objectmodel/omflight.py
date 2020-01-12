from flightrecorder.appbase.ombase import AppBaseEntity
from flightrecorder.objectmodel.omairline import Airline
import re


class Flight(AppBaseEntity):
    """Class to represent a flight"""

    PROPERTY_FLIGHT_NUMBER = "number"
    PROPERTY_EMBARKATION = "embarkaton"
    PROPERTY_DESTINATION = "destination"
    PROPERTY_AIRLINE = "airline"

    def __init__(self, number, embarkation, destination, airline, db_id=None):
        """Object initialisation

        :param number: String containing the flight number
        :param embarkation: String containing the 3-letter code for the airport of embarkation, or N/A
        :param destination: String containing the 3-letter code for the destination airport, or N/A
        :param airline: An instance of the Airline class
        :param db_id: Primary key for the database record for this flight
        """
        if not isinstance(airline, Airline):
            raise ValueError("Flight requires an instance of Airline")

        # There are instances where this regex doesn't allow valid flight numbers to be entered
        # https://stackoverflow.com/questions/35688864/regular-expressions-to-match-flight-number
        # number_regex = re.compile(r"(?<![\dA-Z])(?!\d{2})([A-Z\d]{2})\s?(\d{2,4})(?!\d)")
        # match = number_regex.match(number)
        # if not match:
        #    raise ValueError("Invalid flight number '{}'".format(number))

        # Both embarkation and destination must be 3-letter codes
        airport_regex = re.compile("^[A-Z]{3}$")
        if embarkation != "N/A":
            match = airport_regex.match(embarkation)
            if not match:
                raise ValueError("Invalid embarkation airport code '{}'".format(embarkation))

        if destination != "N/A":
            match = airport_regex.match(destination)
            if not match:
                raise ValueError("Invalid destination airport code '{}'".format(destination))

        # Storage of the database ID is handled by the superclass
        super(Flight, self).__init__(db_id)
        self.set_property(Flight.PROPERTY_FLIGHT_NUMBER, number)
        self.set_property(Flight.PROPERTY_EMBARKATION, embarkation)
        self.set_property(Flight.PROPERTY_DESTINATION, destination)
        self.set_property(Flight.PROPERTY_AIRLINE, airline)

    def number(self):
        """Return the flight number"""
        return self.get_property(Flight.PROPERTY_FLIGHT_NUMBER)

    def embarkation(self):
        """Return the 3-letter code for the airport of embarkation"""
        return self.get_property(Flight.PROPERTY_EMBARKATION)

    def destination(self):
        """Return the 3-letter code for the destination airport"""
        return self.get_property(Flight.PROPERTY_DESTINATION)

    def airline(self):
        """Return the airline, that is an instance of the Airline class"""
        return self.get_property(Flight.PROPERTY_AIRLINE)
