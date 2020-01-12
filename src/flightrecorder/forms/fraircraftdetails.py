import npyscreen as np
import datetime as datetime
from flightrecorder.forms.frconstants import *
from flightrecorder.appbase.frmbase import AppBaseActionFormV2


class AircraftDetailsForm(AppBaseActionFormV2):
    """Form to capture the details for a new aircraft"""

    def create(self):
        self.add_text("Aircraft details:")
        self.add_text_box(KEY_REGISTRATION, "Registration     : ", 20, False)
        self.add_text_box(KEY_SERIAL_NUMBER, "Serial Number    :", 20)
        self.add_text_box(KEY_AGE, "Age :", 20)

        self.add_text("Manufacturer:", True)
        self.add_select_list(KEY_MANUFACTURER, "Manufacturer     :", [], "", 5, 20)
        self.add_text_box(KEY_NEW_MANUFACTURER, "New Manufacturer :", 20)

        self.add_text("Model number:",  True)
        self.add_select_list(KEY_MODEL, "Model            :", [], "", 5, 20)
        self.add_text_box(KEY_NEW_MODEL, "New Model        :", 20)

    def beforeEditing(self):
        self.parentApp.set_control_callback_handler(self.manufacturer_selected)
        self.populate_text_box(KEY_REGISTRATION, self.parentApp.get_property(KEY_REGISTRATION))
        self.populate_select_list(KEY_MANUFACTURER, self.parentApp.load_existing_manufacturers(), None)
        self.display()

    def manufacturer_selected(self, control, manufacturer):
        if self.is_control(control, KEY_MANUFACTURER, AppBaseActionFormV2.CTL_TYPE_LIST):
            self.populate_select_list(KEY_MODEL, self.parentApp.load_models_for_manufacturer(manufacturer), None)
            self.display()

    def on_ok(self):
        registration = self.get_value(KEY_REGISTRATION)
        serial_number = self.get_value(KEY_SERIAL_NUMBER)
        age = self.get_value(KEY_AGE)
        manufacturer = self.get_new_item_or_selection(KEY_NEW_MANUFACTURER, KEY_MANUFACTURER)
        model = self.get_new_item_or_selection(KEY_NEW_MODEL, KEY_MODEL)

        if len(registration) > 0 and len(serial_number) > 0 and len(manufacturer) > 0 and len(model) > 0 and \
           age.isdigit():
            self.clear_controls()

            # The parent application maintains a collection of properties as the user progresses through the
            # data entry wizard. Update those associated with this form
            self.parentApp.set_property(KEY_REGISTRATION, registration)
            self.parentApp.set_property(KEY_SERIAL_NUMBER, serial_number)
            self.parentApp.set_property(KEY_MANUFACTURED, datetime.datetime.now().year - int(age))
            self.parentApp.set_property(KEY_AGE, int(age))
            self.parentApp.set_property(KEY_MANUFACTURER, manufacturer)
            self.parentApp.set_property(KEY_MODEL, model)
            self.parentApp.switchForm(FRM_SIGHTING_DETAILS)
        else:
            np.notify_confirm("You must enter all aircraft details and the age must be 0 or a positive integer",
                              title="Aircraft Details Incomplete or Incorrect", form_color='STANDOUT', wrap=True,
                              editw=0)

    def on_cancel(self):
        result = np.notify_ok_cancel("Are you sure?", title="Confirm Cancellation", form_color='STANDOUT', wrap=True,
                                     editw=0)
        if result:
            self.clear_controls()
            self.parentApp.switchForm(FRM_MAIN)
