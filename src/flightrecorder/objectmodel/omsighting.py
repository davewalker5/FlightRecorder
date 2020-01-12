from flightrecorder.appbase.ombase import AppBaseEntity
from flightrecorder.objectmodel.omlocation import Location
from flightrecorder.objectmodel.omflight import Flight
from flightrecorder.objectmodel.omaircraft import Aircraft
import datetime as datetime


class Sighting(AppBaseEntity):
    """Class to represent an aircraft sighting"""

    PROPERTY_ALTITUDE = "altitude"
    PROPERTY_DATE = "date"
    PROPERTY_LOCATION = "location"
    PROPERTY_FLIGHT = "flight"
    PROPERTY_AIRCRAFT = "aircraft"

    def __init__(self, altitude, date, location, flight, aircraft, db_id=None):
        """Object initialisation

        :param altitude: The altitude of the aircraft when observed, in ft, as an integer
        :param date: The date of the observation
        :param location: An instance of the Location class
        :param flight: An instance of the Flight class
        :param aircraft: An instance of the Aircraft class
        :param db_id: Primary key for the database record for this sighting
        """
        if not isinstance(location, Location):
            raise ValueError("Sighting requires an instance of Location")

        if not isinstance(flight, Flight):
            raise ValueError("Sighting requires an instance of Flight")

        if not isinstance(aircraft, Aircraft):
            raise ValueError("Sighting requires an instance of Aircraft")

        if not isinstance(altitude, int):
            raise ValueError("Altitude must be an integer")

        if altitude < 0:
            raise ValueError("Altitude must be 0 or greater")

        if not isinstance(date, datetime.date):
            raise ValueError("Sighting requires an instance of datetime.date")

        # Storage of the database ID is handled by the superclass
        super(Sighting, self).__init__(db_id)
        self.set_property(Sighting.PROPERTY_ALTITUDE, altitude)
        self.set_property(Sighting.PROPERTY_DATE, date)
        self.set_property(Sighting.PROPERTY_LOCATION, location)
        self.set_property(Sighting.PROPERTY_FLIGHT, flight)
        self.set_property(Sighting.PROPERTY_AIRCRAFT, aircraft)

    def altitude(self):
        """Return the altitude of the aircraft when observed"""
        return self.get_property(Sighting.PROPERTY_ALTITUDE)

    def date(self):
        """Return the date of observation"""
        return self.get_property(Sighting.PROPERTY_DATE)

    def location(self):
        """Return the location where the observation was made"""
        return self.get_property(Sighting.PROPERTY_LOCATION)

    def flight(self):
        """Return the flight, that is an instance of the Flight class"""
        return self.get_property(Sighting.PROPERTY_FLIGHT)

    def aircraft(self):
        """Return the aircraft, that is an instance of the Aircraft class"""
        return self.get_property(Sighting.PROPERTY_AIRCRAFT)
