from flightrecorder.forms.frconstants import *
from flightrecorder.appbase.frmbase import AppBaseActionFormV2


class SelectDatabaseLocationForm(AppBaseActionFormV2):
    """Form to select the database location and write it to the configuration file"""

    def create(self):
        self.add_text("Please select the folder containing the database, or where you'd like the database to be")
        self.add_text("created:", False, True)
        self.add_file_selector(KEY_DATABASE_LOCATION, "Folder : ", True, True)
        super(SelectDatabaseLocationForm, self).create()

    def beforeEditing(self):
        database = self.parentApp.database()
        if database is not None:
            self.set_value(KEY_DATABASE_LOCATION, database.location())
        else:
            self.set_value(KEY_DATABASE_LOCATION, None)
        self.display()

    def on_ok(self):
        location = self.get_value(KEY_DATABASE_LOCATION)
        self.parentApp.set_database_location(location)
        self.parentApp.switchFormPrevious()

    def on_cancel(self):
        self.parentApp.switchFormPrevious()
