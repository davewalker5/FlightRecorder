import npyscreen as np
from flightrecorder.forms.frconstants import *
from flightrecorder.appbase.frmbase import AppBaseActionFormV2


class DataExchangeForm(AppBaseActionFormV2):
    """Base data import/export form for data exchange with CSV files"""

    def create(self):
        # These controls are used to report progress details as the data exchange proceeds
        self.add_text("Current Record Details: ", True, False)
        self.add_text_box(KEY_RECORD, "Record : ", 17, False)
        self.add_text_box(KEY_FLIGHT_NUMBER, "Flight Number : ", 17, False)
        self.add_text_box(KEY_REGISTRATION, "Registration : ", 17, False)
        self.add_text_box(KEY_MANUFACTURER, "Manufacturer : ", 17, False)
        self.add_text_box(KEY_MODEL, "Model : ", 17, False)
        self.add_text_box(KEY_DATE, "Date : ", 17, False)

        # This reports the current status of the data exchange operation
        self.add(np.FixedText, value="", use_two_lines=False, editable=False)
        self.add_text_box(KEY_STATUS, "Status : ", 17, False)
        self.populate_text_box(KEY_STATUS, "Idle")

    def on_ok(self):
        self.clear_controls()

    def on_cancel(self):
        self.clear_controls()
        self.parentApp.switchForm(FRM_MAIN)

    def progress_callback(self, record, flight_number, date, registration, manufacturer, model, status):
        """This is specified as the progress update to the data exchange method and is called as each record is
        processed to update the on-screen record properties"""

        if record is not None:
            self.populate_text_box(KEY_RECORD, record)
            self.populate_text_box(KEY_FLIGHT_NUMBER, flight_number)
            self.populate_text_box(KEY_DATE, date)
            self.populate_text_box(KEY_REGISTRATION, registration)
            self.populate_text_box(KEY_MANUFACTURER, manufacturer)
            self.populate_text_box(KEY_MODEL, model)
        else:
            self.clear_control(KEY_FILENAME)
        self.populate_text_box(KEY_STATUS, status)
        self.display()


class ImportForm(DataExchangeForm):
    """Form implementing CSV import"""
    def create(self):
        self.add_file_selector(KEY_FILENAME, "Filename : ", True, False)
        super(ImportForm, self).create()

    def on_ok(self):
        filename = self.get_value(KEY_FILENAME)
        if filename is not None:
            self.parentApp.import_data(filename, self.progress_callback)
        else:
            np.notify_confirm("You must enter the path to an existing file to import from",
                              title="Filename Not Specified", form_color='STANDOUT', wrap=True,
                              editw=0)


class ExportForm(DataExchangeForm):
    """Form implementing CSV export"""
    def create(self):
        self.add_file_selector(KEY_FILENAME, "Filename : ", False, False)
        super(ExportForm, self).create()

    def on_ok(self):
        filename = self.get_value(KEY_FILENAME)
        if filename is not None:
            self.parentApp.export_data(filename, self.progress_callback)
        else:
            np.notify_confirm("You must enter the path to a file to export to",
                              title="Filename Not Specified", form_color='STANDOUT', wrap=True,
                              editw=0)
