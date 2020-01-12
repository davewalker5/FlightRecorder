import npyscreen as np
from flightrecorder.forms.frconstants import *
from flightrecorder.appbase.frmbase import AppBaseActionFormV2


class AircraftRegistrationForm(AppBaseActionFormV2):
    """Form to enter the aircraft registration number and:

    1. Jump to the aircraft details form if this aircraft hasn't been seen before
    2. Jump to the sighting details form if this is an existing aircraft
    """

    def create(self):
        self.add_text("Enter the aircraft registration. You will be prompted for further details")
        self.add_text("if there is no existing record for it:")
        self.add_text("")
        self.add_text_box(KEY_REGISTRATION, "Registration : ", 16)

    def on_ok(self):
        registration = self.get_value(KEY_REGISTRATION).upper()
        if len(registration) > 0:
            self.clear_controls()

            # The parent application maintains a collection of properties as the user progresses through the
            # data entry wizard. Update those associated with this form
            self.parentApp.set_property(KEY_REGISTRATION, registration)

            # If the aircraft exists, we can jump straight to the sighting details form. Otherwise, we need to
            # collect (new) aircraft details, first
            if self.parentApp.aircraft_exists(registration):
                self.parentApp.load_existing_aircraft()
                self.parentApp.switchForm(FRM_SIGHTING_DETAILS)
            else:
                self.parentApp.switchForm(FRM_AIRCRAFT_DETAILS)
        else:
            np.notify_confirm("You must enter an aircraft registration number", title="Aircraft Details Incomplete",
                              form_color='STANDOUT', wrap=True, editw=0)

    def on_cancel(self):
        result = np.notify_ok_cancel("Are you sure?", title="Confirm Cancellation", form_color='STANDOUT', wrap=True,
                                     editw=0)
        if result:
            self.clear_controls()
            self.parentApp.switchForm(FRM_MAIN)
