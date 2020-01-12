import npyscreen as np
from flightrecorder.forms.frconstants import *
from flightrecorder.appbase.frmbase import AppBaseActionFormV2


class FlightDetailsForm(AppBaseActionFormV2):
    """Form to enter flight details"""

    def create(self):
        self.add_text("Enter the flight number and select a matching flight to reuse it:")
        self.add_text_box(KEY_FLIGHT_NUMBER, "Flight Number    : ", 20)
        self.add_select_list(KEY_EXISTING_FLIGHT, "Existing Flight  : ", [], "", 6, 20)

        self.add_text("Fill in the following details to create a new flight:")
        self.add_text_box(KEY_EMBARKATION, "Embarkation      : ", 20)
        self.add_text_box(KEY_DESTINATION, "Destination      : ", 20)
        self.add_text_box(KEY_NEW_AIRLINE, "New Airline      : ", 20)
        self.add_select_list(KEY_AIRLINE, "Existing Airline : ", [], "", 6, 20)

    def beforeEditing(self):
        self.parentApp.set_control_callback_handler(self.flight_number_changed)
        self.populate_select_list(KEY_AIRLINE, self.parentApp.load_existing_airlines(), None)
        self.display()

    def flight_number_changed(self, control, flight_number):
        """Callback to handle changes in the flight number and, if applicable, present details of existing flights
        with that number that the user can select to shortcut data entry"""

        if self.is_control(control, KEY_FLIGHT_NUMBER, AppBaseActionFormV2.CTL_TYPE_TEXT):
            existing_flights = self.parentApp.load_matching_flights(flight_number)
            if existing_flights is not None:
                values = ["{} to  {} - {}".format(embarkation, destination, airline) for embarkation, destination,
                          airline in existing_flights]
                self.populate_select_list(KEY_EXISTING_FLIGHT, existing_flights, values)
            else:
                self.clear_control(KEY_EXISTING_FLIGHT)
            self.display()

    def on_ok(self):
        flight_number = self.get_value(KEY_FLIGHT_NUMBER)

        # Extract the flight details, either from a selected existing flight or the new flight entry fields
        if self.has_value(KEY_EXISTING_FLIGHT) > 0:
            existing_flight = self.get_value(KEY_EXISTING_FLIGHT)
            embarkation = existing_flight[0]
            destination = existing_flight[1]
            airline = existing_flight[2]
        else:
            embarkation = self.get_value(KEY_EMBARKATION)
            destination = self.get_value(KEY_DESTINATION)
            airline = self.get_new_item_or_selection(KEY_NEW_AIRLINE, KEY_AIRLINE)

        if len(flight_number) > 0 and len(embarkation) > 0 and len(destination) > 0 and len(airline) > 0:
            self.clear_controls()

            # The parent application maintains a collection of properties as the user progresses through the
            # data entry wizard. Update those associated with this form
            self.parentApp.set_property(KEY_FLIGHT_NUMBER, flight_number)
            self.parentApp.set_property(KEY_EMBARKATION, embarkation)
            self.parentApp.set_property(KEY_DESTINATION, destination)
            self.parentApp.set_property(KEY_AIRLINE, airline)
            self.parentApp.switchForm(FRM_AIRCRAFT_REGISTRATION)
        else:
            np.notify_confirm("You must enter a flight number and either select an existing flight or "\
                              "enter all new flight details", title="Flight Details Incomplete", form_color='STANDOUT',
                              wrap=True, editw=0)

    def on_cancel(self):
        result = np.notify_ok_cancel("Are you sure?", title="Confirm Cancellation", form_color='STANDOUT', wrap=True,
                                     editw=0)
        if result:
            self.clear_controls()
            self.parentApp.switchForm(FRM_MAIN)