from flightrecorder.appbase.dbbase import AppBaseRepository
from flightrecorder.db.dbmodelrepo import ModelRepository
from flightrecorder.db.dbmodels import AircraftModel
from flightrecorder.objectmodel.omaircraft import Aircraft


class AircraftRepository(AppBaseRepository):
    """Repository for addition and querying of Aircraft"""

    def __init__(self, session):
        super(AircraftRepository, self).__init__(session, AircraftModel)

    def exists(self, registration):
        """Check for aircraft existence

        :param registration: String containing the registration number for the aircraft
        :return: True if an aircraft with that egistration exists, False if not
        """
        return super(AircraftRepository, self).query_exists("registration", registration)

    def read(self, column, value):
        """Load and return a specified aircraft

        :param column: The name of the column to search
        :param value: The value to look for (exact match)
        :return: An Aircraft object instance for the matching aircraft, or None if there is no match
        """
        aircraft = super(AircraftRepository, self).query_first(column, value)
        if aircraft is None:
            return None
        m = ModelRepository(self._session)
        model = m.read("id", aircraft.model_id)
        return Aircraft(aircraft.registration, aircraft.serial_number, aircraft.manufactured, model, aircraft.id)

    def create(self, registration, serial_number, manufactured, model, manufacturer):
        """Create and return a new aircraft or return the existing aircraft if it already exists

        :param registration: String containing the aircraft registration number
        :param serial_number: String containing the aircraft serial number
        :param manufactured: The year of manufacture of the aircraft, as an integer
        :param model: String containing the aircraft model
        :param manufacturer: String containing the manufacturer name
        :return: An Aircraft object instance for the aircraft
        """
        if not self.exists(registration):
            m = ModelRepository(self._session)
            model_id = m.create(model, manufacturer).db_id()
            super(AircraftRepository, self).insert(AircraftModel(registration=registration,
                                                                 serial_number=serial_number,
                                                                 manufactured=manufactured,
                                                                 model_id=model_id))
        return self.read("registration", registration)
