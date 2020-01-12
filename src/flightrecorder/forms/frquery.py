import npyscreen as np
import curses
from flightrecorder.appbase.constants import *
from flightrecorder.forms.frconstants import *
from flightrecorder.appbase.frmbase import AppBaseActionFormV2


class QueryForm(AppBaseActionFormV2):
    """Base form for querying the flight recorder database"""

    def create(self):
        self.add_text("")
        col_titles = ["Flight", "Airline", "Route", "Aircraft", "Date"]
        self._grid = self.add(np.GridColTitles, col_titles=col_titles, column_width=14)
        self._grid.add_handlers({curses.ascii.NL: self.grid_row_selected})

    def populate_query_results(self, results):
        """Given a collection of Sighting instances, populate the grid with their details. Note that this form
        does not use the superclass backing data"""

        # TODO : This form should use the superclass form's backing data
        self._results = []
        self._grid.values = []
        for sighting in results:
            if sighting is not None:
                self._results.append(sighting)
                row = [sighting.flight().number(),
                       sighting.flight().airline().name(),
                       sighting.flight().embarkation() + " - " + sighting.flight().destination(),
                       sighting.aircraft().registration(),
                       sighting.date().strftime(DATE_FORMAT)]
                self._grid.values.append(row)
        self._grid.h_show_beginning(None)
        self.display()

    def grid_row_selected(self, input):
        """When a row in the grid is selected, show a form containing the full sighting detail (the grid only
        has room for a subset)"""
        try:
            index = self.get_selected_row_index(self._grid)
            if index is not None:
                selected_sighting = self._results[index]
                self.parentApp.load_sighting(selected_sighting)
                self.parentApp.switchForm(FRM_QUERY_SHOW_SIGHTING)

        except IndexError:
            pass

    def on_ok(self):
        self.clear_controls()
        self.parentApp.switchForm(FRM_MAIN)

    def on_cancel(self):
        self.clear_controls()
        self.parentApp.switchForm(FRM_MAIN)


class QueryByFlight(QueryForm):
    """Form allowing the use to query the database by flight number"""

    def create(self):
        self.add_text_box(KEY_FLIGHT_NUMBER, "Flight Number : ", 20)
        super(QueryByFlight, self).create()

    def beforeEditing(self):
        self.parentApp.set_control_callback_handler(self.query_filter_changed)

    def query_filter_changed(self, control, flight_number):
        if self.is_control(control, KEY_FLIGHT_NUMBER, AppBaseActionFormV2.CTL_TYPE_TEXT):
            results = self.parentApp.database().query_sightings_by_flight(flight_number)
            self.populate_query_results(results)


class QueryByAircraft(QueryForm):
    """Form allowing the use to query the database by aircraft (specified by the registration number)"""

    def create(self):
        self.add_text_box(KEY_REGISTRATION, "Registration : ", 20)
        super(QueryByAircraft, self).create()

    def beforeEditing(self):
        self.parentApp.set_control_callback_handler(self.query_filter_changed)

    def query_filter_changed(self, control, registration):
        if self.is_control(control, KEY_REGISTRATION, AppBaseActionFormV2.CTL_TYPE_TEXT):
            results = self.parentApp.database().query_sightings_by_aircraft(registration)
            self.populate_query_results(results)


class QueryByAirline(QueryForm):
    """Form allowing the use to query the database by airline"""

    def create(self):
        self.add_select_list(KEY_AIRLINE, "Airline : ", [], "", 6, 20)
        super(QueryByAirline, self).create()

    def beforeEditing(self):
        self.parentApp.set_control_callback_handler(self.query_filter_changed)
        self.populate_select_list(KEY_AIRLINE, self.parentApp.load_existing_airlines(), None)
        self.display()

    def query_filter_changed(self, control, airline):
        if self.is_control(control, KEY_AIRLINE, AppBaseActionFormV2.CTL_TYPE_LIST):
            results = self.parentApp.database().query_sightings_by_airline(airline)
            self.populate_query_results(results)


class QueryByRoute(QueryForm):
    """Form allowing the use to query the database by route (specified using the 3-letter codes for the embarkation and
    destination airports"""

    def create(self):
        self.add_text_box(KEY_EMBARKATION, "Embarkation : ", 20)
        self.add_text_box(KEY_DESTINATION, "Destination : ", 20)
        super(QueryByRoute, self).create()

    def beforeEditing(self):
        self.parentApp.set_control_callback_handler(self.query_filter_changed)

    def query_filter_changed(self, control, airport):
        if self.is_control(control, KEY_EMBARKATION, AppBaseActionFormV2.CTL_TYPE_TEXT):
            embarkation = airport
            destination = self.get_value(KEY_DESTINATION)
        elif self.is_control(control, KEY_DESTINATION, AppBaseActionFormV2.CTL_TYPE_TEXT):
            destination = airport
            embarkation = self.get_value(KEY_EMBARKATION)
        else:
            embarkation = None
            destination = None

        if embarkation is not None and len(embarkation) > 0 and destination is not None and len(destination) > 0:
            results = self.parentApp.database().query_sightings_by_route(embarkation, destination)
            self.populate_query_results(results)


class QueryShowSighting(AppBaseActionFormV2):
    """Form to show the full details for a sighting"""

    def create(self):
        self.add_text("FLIGHT DETAILS:")
        self.add_text_box(KEY_FLIGHT_NUMBER, "Flight Number : ", 17, False)
        self.add_text_box(KEY_EMBARKATION, "Embarkation   : ", 17, False)
        self.add_text_box(KEY_DESTINATION, "Destination   : ", 17, False)
        self.add_text_box(KEY_AIRLINE, "Airline       : ", 17, False)

        self.add_text("AIRCRAFT DETAILS:", True, False)
        self.add_text_box(KEY_REGISTRATION, "Registration  : ", 17, False)
        self.add_text_box(KEY_SERIAL_NUMBER, "Serial Number : ", 17, False)
        self.add_text_box(KEY_MANUFACTURER, "Manufacturer  : ", 17, False)
        self.add_text_box(KEY_MODEL, "Model         : ", 17, False)
        self.add_text_box(KEY_MANUFACTURED, "Manufactured  : ", 17, False)
        self.add_text_box(KEY_AGE, "Age           : ", 17, False)

        self.add_text("SIGHTING DETAILS:", True, False)
        self.add_text_box(KEY_ALTITUDE, "Altitude      : ", 17, False)
        self.add_text_box(KEY_LOCATION, "Location      : ", 17, False)
        self.add_text_box(KEY_DATE, "Date          : ", 17, False)

    def beforeEditing(self):
        self.populate_text_box(KEY_FLIGHT_NUMBER, self.parentApp.get_property(KEY_FLIGHT_NUMBER))
        self.populate_text_box(KEY_EMBARKATION, self.parentApp.get_property(KEY_EMBARKATION))
        self.populate_text_box(KEY_DESTINATION, self.parentApp.get_property(KEY_DESTINATION))
        self.populate_text_box(KEY_AIRLINE, self.parentApp.get_property(KEY_AIRLINE))
        self.populate_text_box(KEY_REGISTRATION, self.parentApp.get_property(KEY_REGISTRATION))
        self.populate_text_box(KEY_SERIAL_NUMBER, self.parentApp.get_property(KEY_SERIAL_NUMBER))
        self.populate_text_box(KEY_MANUFACTURER, self.parentApp.get_property(KEY_MANUFACTURER))
        self.populate_text_box(KEY_MODEL, self.parentApp.get_property(KEY_MODEL))
        self.populate_text_box(KEY_MANUFACTURED, self.parentApp.get_property(KEY_MANUFACTURED))
        self.populate_text_box(KEY_AGE, self.parentApp.get_property(KEY_AGE))
        self.populate_text_box(KEY_ALTITUDE, self.parentApp.get_property(KEY_ALTITUDE))
        self.populate_text_box(KEY_LOCATION, self.parentApp.get_property(KEY_LOCATION))
        self.populate_text_box(KEY_DATE, self.parentApp.get_property(KEY_DATE).strftime(DATE_FORMAT))
        self.display()

    def on_ok(self):
        self.parentApp.switchFormPrevious()

    def on_cancel(self):
        self.parentApp.switchFormPrevious()