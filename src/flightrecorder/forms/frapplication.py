from flightrecorder.db.dbdatabase import FlightRecorderDatabase
from flightrecorder.dataexchange.dxcsv import import_csv, export_csv
from flightrecorder.config.configuration import Configuration
from flightrecorder.forms.frconstants import *
from flightrecorder.forms.frmain import MainForm
from flightrecorder.forms.frflightdetails import FlightDetailsForm
from flightrecorder.forms.fraircraftregistration import AircraftRegistrationForm
from flightrecorder.forms.fraircraftdetails import AircraftDetailsForm
from flightrecorder.forms.frsightings import SightingForm
from flightrecorder.forms.frconfirm import ConfirmForm
from flightrecorder.forms.frdataexchange import ImportForm, ExportForm
from flightrecorder.forms.frquery import QueryByFlight, QueryByAircraft, QueryByAirline, QueryByRoute, QueryShowSighting
from flightrecorder.forms.frsetdblocation import SelectDatabaseLocationForm
import npyscreen as np
import datetime as datetime
import os


class FlightRecorderApp(np.NPSAppManaged):
    def onStart(self):
        self._config = Configuration()

        try:
            os.remove(self._config.log_file_name())
        except:
            pass

        # Configure properties
        self._control_callback_handler = None
        self._db = None
        self._properties = {KEY_FLIGHT_NUMBER: None,
                            KEY_EMBARKATION: None,
                            KEY_DESTINATION: None,
                            KEY_AIRLINE: None,
                            KEY_REGISTRATION: None,
                            KEY_SERIAL_NUMBER: None,
                            KEY_MANUFACTURER: None,
                            KEY_MODEL: None,
                            KEY_MANUFACTURED: None,
                            KEY_AGE: None,
                            KEY_ALTITUDE: None,
                            KEY_LOCATION: None,
                            KEY_DATE: None}

        # Attempt to open the database
        self.open_database()

        # Configure forms
        title = "Flight Recorder V{}".format(self._config.app_version())
        self.addForm(FRM_MAIN, MainForm, name=title)
        self.addForm(FRM_SET_DATABASE_LOCATION, SelectDatabaseLocationForm, name=title)
        self.addForm(FRM_FLIGHT_DETAILS, FlightDetailsForm, name="Flight Details")
        self.addForm(FRM_AIRCRAFT_REGISTRATION, AircraftRegistrationForm, name="Aircraft Registration Number")
        self.addForm(FRM_AIRCRAFT_DETAILS, AircraftDetailsForm, name="Aircraft Details")
        self.addForm(FRM_SIGHTING_DETAILS, SightingForm, name="Aircraft Sighting Details")
        self.addForm(FRM_CONFIRM_FORM, ConfirmForm, name="Confirm Details")
        self.addForm(FRM_IMPORT, ImportForm, name="Import Data")
        self.addForm(FRM_EXPORT, ExportForm, name="Export Data")
        self.addForm(FRM_QUERY_BY_FLIGHT, QueryByFlight, name="Query Sightings by Flight Number")
        self.addForm(FRM_QUERY_BY_AIRCRAFT, QueryByAircraft, name="Query Sightings by Aircraft")
        self.addForm(FRM_QUERY_BY_AIRLINE, QueryByAirline, name="Query Sightings by Airline")
        self.addForm(FRM_QUERY_BY_ROUTE, QueryByRoute, name="Query Sightings by Route")
        self.addForm(FRM_QUERY_SHOW_SIGHTING, QueryShowSighting, name="Selected Sighting")

    def log_debug(self, message):
        date = datetime.datetime.now().strftime("%d/%m/%Y, %H:%M:%S")
        f = open(self._config.log_file_name(), mode="at", encoding="UTF-8")
        f.write("{} : {}\n".format(date, message))
        f.close()

    def log_properties(self):
        for property_name, value in self._properties.items():
            if value is None:
                log_value = "None"
            else:
                log_value = value
            self.log_debug("{} = {}".format(property_name, log_value))

    def open_database(self):
        if self._config.database_name() is not None:
            self._db = FlightRecorderDatabase()
        else:
            self._db = None

    def set_database_location(self, location):
        self._config.set_database_location(location)
        self.open_database()

    def set_property(self, property_name, value):
        new_value = {property_name: value}
        self._properties.update(new_value)

    def get_property(self, property_name):
        return self._properties[property_name]

    def set_control_callback_handler(self, handler):
        self._control_callback_handler = handler

    def handle_control_callback(self, control, value):
        if self._control_callback_handler is not None:
            self._control_callback_handler(control, value)

    def database(self):
        return self._db

    def flight_exists(self, number):
        return self._db.flight_repo().exists(number)

    def load_existing_airlines(self):
        airlines = self._db.airline_repo().read_all()
        return [airline.name() for airline in airlines]

    def aircraft_exists(self, registration):
        return self._db.aircraft_repo().exists(registration)

    def load_existing_aircraft(self):
        aircraft = self._db.aircraft_repo().read("registration", self.get_property(KEY_REGISTRATION))
        self.set_property(KEY_SERIAL_NUMBER, aircraft.serial_number())
        self.set_property(KEY_MANUFACTURER, aircraft.model().manufacturer().name())
        self.set_property(KEY_MODEL, aircraft.model().name())
        self.set_property(KEY_MANUFACTURED, aircraft.manufactured())
        self.set_property(KEY_AGE, aircraft.age())

    def load_existing_manufacturers(self):
        manufacturers = self._db.manufacturer_repo().read_all()
        return [manufacturer.name() for manufacturer in manufacturers]

    def load_models_for_manufacturer(self, manufacturer):
        models = self._db.model_repo().read_all_for_manufacturer(manufacturer)
        return [model.name() for model in models]

    def load_existing_locations(self):
        locations = self._db.location_repo().read_all()
        return [location.name() for location in locations]

    def load_matching_flights(self, flight_number):
        flights = self._db.flight_repo().read_all("number", flight_number, "id")
        return [(flight.embarkation(), flight.destination(), flight.airline().name()) for flight in flights]

    def load_sighting(self, sighting):
        self.set_property(KEY_FLIGHT_NUMBER, sighting.flight().number())
        self.set_property(KEY_EMBARKATION, sighting.flight().embarkation())
        self.set_property(KEY_DESTINATION, sighting.flight().destination())
        self.set_property(KEY_AIRLINE, sighting.flight().airline().name())
        self.set_property(KEY_REGISTRATION, sighting.aircraft().registration())
        self.set_property(KEY_SERIAL_NUMBER, sighting.aircraft().serial_number())
        self.set_property(KEY_MANUFACTURER, sighting.aircraft().model().manufacturer().name())
        self.set_property(KEY_MODEL, sighting.aircraft().model().name())
        self.set_property(KEY_MANUFACTURED, sighting.aircraft().manufactured())
        self.set_property(KEY_AGE, sighting.aircraft().age())
        self.set_property(KEY_ALTITUDE, sighting.altitude())
        self.set_property(KEY_LOCATION, sighting.location().name())
        self.set_property(KEY_DATE, sighting.date())

    def create_record(self):
        aircraft_id = self._db.create_aircraft(self.get_property(KEY_REGISTRATION),
                                               self.get_property(KEY_SERIAL_NUMBER),
                                               self.get_property(KEY_MANUFACTURED),
                                               self.get_property(KEY_MODEL),
                                               self.get_property(KEY_MANUFACTURER)).db_id()

        flight_id = self._db.create_flight(self.get_property(KEY_FLIGHT_NUMBER),
                                           self.get_property(KEY_EMBARKATION),
                                           self.get_property(KEY_DESTINATION),
                                           self.get_property(KEY_AIRLINE)).db_id()

        location_id = self._db.create_location(self.get_property(KEY_LOCATION)).db_id()

        self._db.create_sighting(self.get_property(KEY_ALTITUDE),
                                 self.get_property(KEY_DATE),
                                 location_id,
                                 flight_id,
                                 aircraft_id)

    def import_data(self, filename, progress_callback):
        import_csv(filename, progress_callback, self._db)

    def export_data(self, filename, progress_callback):
        export_csv(filename, progress_callback, self._db)
