from flightrecorder.appbase.dbbase import AppBaseRepository
from flightrecorder.db.dblocationrepo import LocationRepository
from flightrecorder.db.dbflightrepo import FlightRepository
from flightrecorder.db.dbaircraftrepo import AircraftRepository
from flightrecorder.db.dbmodels import SightingModel
from flightrecorder.objectmodel.omsighting import Sighting


class SightingRepository(AppBaseRepository):
    """Repository for addition and querying of sightings, or  observations"""

    def __init__(self, session=None):
        super(SightingRepository, self).__init__(session, SightingModel)

    def insert(self, altitude, date, location_id, flight_id, aircraft_id):
        """Create a new sighting

        :param altitude: Aircraft altitude, in ft, as an integer
        :param date: Date of the sighting
        :param location_id: Primary key for the associated observation location record
        :param flight_id: Primary key for the associated flight record
        :param aircraft_id: Primary key for the associated aircraft record
        :return: A Sighting object instance
        """
        record = super(SightingRepository, self).insert(SightingModel(altitude=altitude,
                                                                      date=date,
                                                                      location_id=location_id,
                                                                      flight_id=flight_id,
                                                                      aircraft_id=aircraft_id))
        return self.read(record.id)

    def read(self, db_id):
        """Return a specific sighting

        :param db_id: Primary key for the sighting to return
        :return: A Sighting object instance
        """
        sighting = super(SightingRepository, self).query_first("id", db_id)
        if sighting is None:
            return None

        l = LocationRepository(self._session)
        location = l.read("id", sighting.location_id)

        f = FlightRepository(self._session)
        flight = f.read("id", sighting.flight_id)

        a = AircraftRepository(self._session)
        aircraft = a.read("id", sighting.aircraft_id)

        return Sighting(sighting.altitude, sighting.date, location, flight, aircraft, sighting.id)

    def read_all(self, column=None, value=None, order_by="id"):
        """Return all sightings, optionally filtering by a specified criterion

        :param column: Name of the columne to use to filter the results, or None for no filtering
        :param value: The value to look for (exact match), or None for no filtering
        :param order_by: The name of the column to order the results by
        :return: A generator that will yield a Sighting object instance for each match
        """
        for sighting in super(SightingRepository, self).query_all(column, value, order_by):
            yield self.read(sighting.id)
