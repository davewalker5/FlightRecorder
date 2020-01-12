import npyscreen as np
from flightrecorder.appbase.constants import *
from flightrecorder.forms.frconstants import *
from flightrecorder.appbase.frmbase import AppBaseActionFormV2


class ConfirmForm(AppBaseActionFormV2):
    """Form to display the details for a new sighting and confirm they are correct before requesting they are
    committed"""

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
        self.populate_text_box(KEY_AGE, self.parentApp.get_property(KEY_AGE))
        self.populate_text_box(KEY_ALTITUDE, self.parentApp.get_property(KEY_ALTITUDE))
        self.populate_text_box(KEY_LOCATION, self.parentApp.get_property(KEY_LOCATION))
        self.populate_text_box(KEY_DATE, self.parentApp.get_property(KEY_DATE).strftime(DATE_FORMAT))
        self.display()

    def on_ok(self):
        self.parentApp.create_record()

        message = "Your sighting of flight {}, aircraft {} {}, at {} on {} has been added to the database"\
                  .format(self.parentApp.get_property(KEY_FLIGHT_NUMBER),
                          self.parentApp.get_property(KEY_MANUFACTURER),
                          self.parentApp.get_property(KEY_MODEL),
                          self.parentApp.get_property(KEY_LOCATION),
                          self.parentApp.get_property(KEY_DATE).strftime(DATE_FORMAT))
        np.notify_wait(message, title="Sighting Successfully Added", form_color='STANDOUT', wrap=True, wide=False)

        self.clear_controls()
        self.parentApp.switchForm(FRM_MAIN)

    def on_cancel(self):
        result = np.notify_ok_cancel("Are you sure?", title="Confirm Cancellation", form_color='STANDOUT', wrap=True,
                                     editw=0)
        if result:
            self.clear_controls()
            self.parentApp.switchForm(FRM_MAIN)