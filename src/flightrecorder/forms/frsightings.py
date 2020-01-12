import npyscreen as np
from flightrecorder.forms.frconstants import *
from flightrecorder.appbase.frmbase import AppBaseActionFormV2


class SightingForm(AppBaseActionFormV2):
    """Form to capture the details of a new sighting"""

    def create(self):
        self.add_text("Enter the sighting Details. Leave the date blank to use today's date or enter")
        self.add_text("a date in DD/MM/YYYY format:", False, True)
        self.add_text_box(KEY_DATE, "Date         : ", 15)
        self.add_text_box(KEY_ALTITUDE, "Altitude     : ", 15)
        self.add_text("")
        self.add_select_list(KEY_LOCATION, "Location     : ", [], "", 10, 15)
        self.add_text_box(KEY_NEW_LOCATION, "New Location : ", 15)

    def beforeEditing(self):
        self.populate_select_list(KEY_LOCATION, self.parentApp.load_existing_locations(), None)
        self.display()

    def on_ok(self):
        date = self.get_date_value(KEY_DATE)
        altitude = self.get_value(KEY_ALTITUDE)
        location = self.get_new_item_or_selection(KEY_NEW_LOCATION, KEY_LOCATION)
        if len(location) > 0 and date is not None and altitude.isdigit():
            self.clear_controls()
            self.parentApp.set_property(KEY_LOCATION, location)
            self.parentApp.set_property(KEY_ALTITUDE, int(altitude))
            self.parentApp.set_property(KEY_DATE, date)
            self.parentApp.switchForm(FRM_CONFIRM_FORM)
        else:
            np.notify_confirm("You must enter a location and altitude and the altitude must be 0 or a positive integer",
                              title="Sighting Details Incomplete or Incorrect", form_color='STANDOUT', wrap=True,
                              editw=0)
            self.edit()

    def on_cancel(self):
        result = np.notify_ok_cancel("Are you sure?", title="Confirm Cancellation", form_color='STANDOUT', wrap=True,
                                     editw=0)
        if result:
            self.clear_controls()
            self.parentApp.switchForm(FRM_MAIN)