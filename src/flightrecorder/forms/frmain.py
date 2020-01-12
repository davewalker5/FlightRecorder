import npyscreen as np
from flightrecorder.forms.frconstants import *
from flightrecorder.appbase.frmbase import AppBaseActionFormV2


class MainForm(AppBaseActionFormV2):
    """Application main form"""

    def create(self):
        self.add_text_box(KEY_DATABASE_LOCATION, "DB Location    : ", 20, False)
        self.add_text("")
        self.add_select_list(KEY_DATA_ENTRY_OPTIONS, "Data entry     : ", [], "", 4, 20)
        self.add_select_list(KEY_DATA_QUERY_OPTIONS, "Queries        : ", [], "", 5, 20)
        self.add_select_list(KEY_DATA_MAINTENANCE_OPTIONS, "Maintenance    : ", [], "", 4, 20)

    def beforeEditing(self):
        # Display the database location
        database = self.parentApp.database()
        if database is not None:
            location = self.parentApp.database().location()
            if len(location) > 45:
                location = "... " + location[-45:]
        else:
            location = None
        self.set_value(KEY_DATABASE_LOCATION, location)

        # Data entry options are in three groups - data entry, data queries and data maintenance
        data_entry_options = [OPT_CREATE_SIGHTING, OPT_IMPORT, OPT_EXPORT]
        self.populate_select_list(KEY_DATA_ENTRY_OPTIONS, data_entry_options, None)

        data_query_options = [OPT_QUERY_FLIGHT, OPT_QUERY_AIRCRAFT, OPT_QUERY_AIRLINE, OPT_QUERY_ROUTE]
        self.populate_select_list(KEY_DATA_QUERY_OPTIONS, data_query_options, None)

        data_maintenance_options = [OPT_MAINTAIN_DATABASE]
        self.populate_select_list(KEY_DATA_MAINTENANCE_OPTIONS, data_maintenance_options, None)

        self.parentApp.set_control_callback_handler(self.option_selected)

    def option_not_implemented(self, category, option):
        message = "{} option '{}' has not been implemented yet".format(category, option)
        np.notify_wait(message, title="Option Not Implemented", form_color='STANDOUT', wrap=True, wide=False)

    def option_selected(self, control, manufacturer):
        # When an option is selected in one of the three data entry groups, the options in the others must be
        # deselected
        if self.is_control(control, KEY_DATA_ENTRY_OPTIONS, AppBaseActionFormV2.CTL_TYPE_LIST):
            self.set_value(KEY_DATA_QUERY_OPTIONS, None)
            self.set_value(KEY_DATA_MAINTENANCE_OPTIONS, None)
        elif self.is_control(control, KEY_DATA_QUERY_OPTIONS, AppBaseActionFormV2.CTL_TYPE_LIST):
            self.set_value(KEY_DATA_ENTRY_OPTIONS, None)
            self.set_value(KEY_DATA_MAINTENANCE_OPTIONS, None)
        elif self.is_control(control, KEY_DATA_MAINTENANCE_OPTIONS, AppBaseActionFormV2.CTL_TYPE_LIST):
            self.set_value(KEY_DATA_QUERY_OPTIONS, None)
            self.set_value(KEY_DATA_ENTRY_OPTIONS, None)
        self.display()

    def handle_data_entry_selection(self):
        if self.parentApp.database() is not None:
            data_entry_selection = self.get_value(KEY_DATA_ENTRY_OPTIONS)
            if data_entry_selection == OPT_CREATE_SIGHTING:
                self.parentApp.switchForm(FRM_FLIGHT_DETAILS)
            elif data_entry_selection == OPT_IMPORT:
                self.parentApp.switchForm(FRM_IMPORT)
            elif data_entry_selection == OPT_EXPORT:
                self.parentApp.switchForm(FRM_EXPORT)
            else:
                self.option_not_implemented("Data entry", data_entry_selection)
                self.edit()
        else:
            np.notify_wait("You must select a database location first", title="Database Location Not Set",
                           form_color='STANDOUT', wrap=True, wide=False)
            self.edit()

    def handle_data_query_selection(self):
        if self.parentApp.database() is not None:
            data_query_selection = self.get_value(KEY_DATA_QUERY_OPTIONS)
            if data_query_selection == OPT_QUERY_FLIGHT:
                self.parentApp.switchForm(FRM_QUERY_BY_FLIGHT)
            elif data_query_selection == OPT_QUERY_AIRCRAFT:
                self.parentApp.switchForm(FRM_QUERY_BY_AIRCRAFT)
            elif data_query_selection == OPT_QUERY_AIRLINE:
                self.parentApp.switchForm(FRM_QUERY_BY_AIRLINE)
            elif data_query_selection == OPT_QUERY_ROUTE:
                self.parentApp.switchForm(FRM_QUERY_BY_ROUTE)
            else:
                self.option_not_implemented("Data query", data_query_selection)
                self.edit()
        else:
            np.notify_wait("You must select a database location first", title="Database Location Not Set",
                           form_color='STANDOUT', wrap=True, wide=False)
            self.edit()

    def handle_data_maintenance_selection(self):
        data_maintenance_selection = self.get_value(KEY_DATA_MAINTENANCE_OPTIONS)
        if data_maintenance_selection == OPT_MAINTAIN_DATABASE:
            self.parentApp.setNextForm(FRM_SET_DATABASE_LOCATION)
        elif self.parentApp.database() is not None:
            if data_maintenance_selection == OPT_MAINTAIN_AIRLINES:
                self.option_not_implemented("Data maintenance", data_maintenance_selection)
                self.edit()
            elif data_maintenance_selection == OPT_MAINTAIN_FLIGHTS:
                self.option_not_implemented("Data maintenance", data_maintenance_selection)
                self.edit()
            elif data_maintenance_selection == OPT_MAINTAIN_LOCATIONS:
                self.option_not_implemented("Data maintenance", data_maintenance_selection)
                self.edit()
            else:
                self.option_not_implemented("Data maintenance", data_maintenance_selection)
                self.edit()
        else:
            np.notify_wait("You must select a database location first", title="Database Location Not Set",
                           form_color='STANDOUT', wrap=True, wide=False)
            self.edit()

    def on_ok(self):
        if self.has_value(KEY_DATA_ENTRY_OPTIONS):
            self.handle_data_entry_selection()
        elif self.has_value(KEY_DATA_QUERY_OPTIONS):
            self.handle_data_query_selection()
        elif self.has_value(KEY_DATA_MAINTENANCE_OPTIONS):
            self.handle_data_maintenance_selection()
        else:
            np.notify_wait("You must select a data entry or query option", title="Option Not Selected",
                           form_color='STANDOUT', wrap=True, wide=False)
            self.edit()

    def on_cancel(self):
        self.parentApp.setNextForm(None)