import npyscreen as np
import datetime as datetime
import re
from flightrecorder.appbase.constants import *


class AppBaseSelectOne(np.SelectOne):
    """Customised single select list

    This class overrides the npyscreen "SelectOne" class to allow an application-specific callback handler to
    be called when a selection is made or cleared, which does not seem to be supported by npyscreen out-of-the-box
    """
    def h_select(self, ch):
        super(AppBaseSelectOne, self).h_select(ch)

        # Check we have a value. If we do, determine its index and get the selected value
        value = ""
        if self.value is not None and len(self.value) > 0:
            index = self.value[0]
            if index is not None and index >= 0:
                value = self.values[index]

        # The parent application must supply a method called "handle_control_callback" that
        # can receive the selected value and forward it to the callback nominated by the current
        # active form
        self.find_parent_app().handle_control_callback(self, value)


class AppBaseTitleSelectOne(np.TitleMultiLine):
    """Customised multi-line single list with title

    This class overrides the default "_entry_type" class with the customised single select class that calls a
    callback registered with the parent application when a selection is made/unmade
    """
    _entry_type = AppBaseSelectOne


class AppBaseTitleText(np.TitleText):
    """Customised text-entry field with title

    This class overrides the "edit" method to ensure the value passed to the parent application value change handler
    is always a string
    """
    def edit(self):
        # This is a hack to prevent len() being called on e.g. integer input into a text field
        self.value = str(self.value)
        super(AppBaseTitleText, self).edit()
        self.find_parent_app().handle_control_callback(self, self.value)


class AppBaseActionFormV2(np.ActionFormV2):
    """Customised version of the npyscreen action form

    1. Each control is uniquely identified
    2. The collection of controls on the form is searchable by identifier to allow reaction to events on specific
       controls
    3. Backing data is stored for lists and grids to allow easy identification of the source data for a selection
       or grid row in cases where the displayed data differs from the source data
    """

    CTL_TYPE_LIST = 1
    CTL_TYPE_TEXT = 2
    CTL_TYPE_FILE = 3

    CTL_TUPLE_CONTROL = 0
    CTL_TUPLE_TYPE = 1
    CTL_TUPLE_NAME = 2

    def __init__(self, name=None, parentApp=None, framed=None, help=None, color='FORMDEFAULT',
                 widget_list=None, cycle_widgets=False, *args, **keywords):
        self._controls = {}
        self._raw_data = {}
        super(AppBaseActionFormV2, self).__init__(name, parentApp, framed, help, color, widget_list,
                                                  cycle_widgets, args, keywords)

    def add_control(self, key, control, control_type):
        """Add a named control to the form

        :param key: The (unique) identifier of the control
        :param control: The control object
        :param control_type: The type of control
        :return: None
        """
        self._controls.update({key: (control, control_type, control.name)})

    def add_text(self, text, blank_before=False, blank_after=False):
        """Add static text to the form

        :param text: The text to add
        :param blank_before: If True, a blank line will be added before the static text
        :param blank_after: If true, a blank line will be added after the static text
        :return: None
        """
        if blank_before:
            self.add(np.FixedText, value="", editable=False)
        self.add(np.FixedText, value=text, editable=False)
        if blank_after:
            self.add(np.FixedText, value="", editable=False)

    def add_text_box(self, key, name, indent, editable=True):
        """Add a titled text box to the form

        :param key: The (unique) identifier for the text box
        :param name: The title text
        :param indent: The left-indent at which data entry starts
        :param editable: If True, the text box is editable, if False it can only be set programatically
        :return: None
        """
        control = self.add(AppBaseTitleText, name=name, value="", use_two_lines=False, begin_entry_at=indent,
                           editable=editable)
        self.add_control(key, control, AppBaseActionFormV2.CTL_TYPE_TEXT)

    def add_select_list(self, key, name, values, footer, lines, indent):
        """Add a titled select list to the form

        :param key: The (unique) identifier for the text box
        :param name: The title text
        :param values: The collection of source values used to derive the select list items
        :param footer: The footer text for the list
        :param lines: The number of visible lines to display
        :param indent: The left-indent at which data entry starts
        :return: None
        """
        control = self.add(AppBaseTitleSelectOne, name=name, values=values, footer=footer, slow_scroll=False,
                           max_height=lines, use_two_lines=False, begin_entry_at=indent)
        self.add_control(key, control, AppBaseActionFormV2.CTL_TYPE_LIST)

    def add_file_selector(self, key, name, must_exist, select_dir):
        """Add a file selection control to the form"""
        control = self.add(np.TitleFilenameCombo, name=name, select_dir=select_dir, must_exist=must_exist)
        self.add_control(key, control, AppBaseActionFormV2.CTL_TYPE_FILE)

    def get_control(self, key):
        """Return a control given its identifier

        :param key: Unique control identifier
        :return: Control object instance
        """
        return self._controls[key][0]

    def get_control_type(self, key):
        """Return the type of a specified control

        :param key: Unique control identifier
        :return: The type for the specified control
        """
        return self._controls[key][1]

    def get_control_key(self, control, control_type):
        """Return the identifier for the specified control object

        :param control: Control object instance
        :param control_type: Type of the control
        :return: The unique identifier for the control
        """
        for key, value in self._controls.items():
            if value[AppBaseActionFormV2.CTL_TUPLE_TYPE] == control_type:
                if value[AppBaseActionFormV2.CTL_TUPLE_CONTROL] == control:
                    return key
                elif value[AppBaseActionFormV2.CTL_TUPLE_NAME] == control.name:
                    return key
        return None

    def is_control(self, control, key, control_type):
        """Identify if a control has the specified identifier and is of the specified type

        :param control: Control object instance
        :param key: Unique control identifier
        :param control_type: Type of the control
        :return: Return True if a con
        """
        return key == self.get_control_key(control, control_type)

    def populate_select_list(self, key, values, formatted_values):
        """Given a collection of values, populate an existing select list

        :param key: Unique control identifier
        :param values: Raw source values to add to the backing data
        :param formatted_values: Formatted values to add to the select list
        :return: None
        """
        control = self.get_control(key)
        control.value = None
        if formatted_values is None:
            control.values = values
            self._raw_data.update({key: values})
        else:
            control.values = formatted_values
            self._raw_data.update({key: values})

    def set_value(self, key, value):
        """Set the value for a specified control

        :param key: Unique control identifier
        :param value: Value to set
        :return: None
        """
        control = self.get_control(key)
        control.value = value

    def populate_text_box(self, key, value):
        """Set the value of a text box

        :param key: Unique control identifier
        :param value: Value to set, that is converted to a string on assignment to the control
        :return: None
        """
        control = self.get_control(key)
        control.value = str(value)

    def has_value(self, key):
        """Determine if a control has a current value or selection

        :param key: Unique control identifier
        :return: True if the control has a value, False if not
        """
        control = self.get_control(key)
        if self.get_control_type(key) == AppBaseActionFormV2.CTL_TYPE_LIST:
            return len(control.value) > 0
        else:
            return control.value is not None and control.value != ""

    def get_value(self, key):
        """Return the value for the control with the specified key

        :param key: Unique control identifier
        :return: For a select list, the text for the selected entry. Otherwise, the control value
        """
        control = self.get_control(key)
        if self.get_control_type(key) == AppBaseActionFormV2.CTL_TYPE_LIST:
            options = self._raw_data[key]
            if options is not None and len(options) > 0:
                return self._raw_data[key][control.value[0]]
            else:
                return None
        else:
            return control.value

    @staticmethod
    def convert_to_integer(value):
        """Convert the specified value to an integer

        :param value: The value to convert
        :return: An integer value if the value is a valid integer, otherwise None
        """
        integer_value = None
        if value is not None:
            value = value.strip()
            integer_regex = re.compile("^[0-9]+$")
            match = integer_regex.match(value)
            if match:
                integer_value = int(value)
        return integer_value

    def get_integer_value(self, key):
        """Return the value for a specified control as an integer

        :param key: Unique control identifier
        :return: An integer value if the control's value is a valid integer, otherwise None
        """
        value = self.get_value(key)
        return self.convert_to_integer(value)

    def get_date_value(self, key):
        """Return the value for a specified control as a date

        :param key: Unique control identifier
        :return: A date value if the control's value is a valid date, otherwise None
        """
        date_string = self.get_value(key)
        if date_string is None or len(date_string) == 0:
            date = datetime.datetime.now()
        else:
            date_regex = re.compile("^[0-9]{2}/[0-9]{2}/[0-9]{4}$")
            match = date_regex.match(date_string)
            if match:
                date = datetime.datetime.strptime(date_string, DATE_FORMAT)
            else:
                date = None
        return date

    def get_new_item_or_selection(self, key_new, key_existing):
        """Return the value from a linked text box/select list pair

        In some instances, a select list is presented to select an existing value along with a text box to enter
        a new value, if the required value is not in the select list. This method returns either a new value, from
        the text box, or if there isn't one an existing value from the select list (in that order of priority)

        :param key_new: Unique identifier for a control used to enter new values
        :param key_existing: Unique identifier for a control used to select existing values
        :return: The entered or selected value (in that order of priority)
        """
        value = self.get_value(key_new)
        if (value is None or value == "") and self.has_value(key_existing):
            value = self.get_value(key_existing)
        return value

    @staticmethod
    def get_selected_row_index(grid):
        """Return the index for the selected row in a grid

        :param grid: Grid control object
        :return: The index for the row that has the current selection, as an integer, or None if there is no selection
        """
        try:
            row = grid.selected_row()
            if row is not None:
                for i in range(0, len(grid.values)):
                    if grid.values[i] is row:
                        return i
            return None
        except IndexError:
            return None

    def clear_control(self, key):
        """Clear the selection and all backing data associated with the specified control

        This clears the current value, clears the values collection (on lists) and removes any backing
        data associated with the control

        :param key: The unique identifier for the control to clear
        :return: None
        """
        control = self.get_control(key)
        control_type = self.get_control_type(key)
        if control_type == AppBaseActionFormV2.CTL_TYPE_LIST:
            control.value = None
            control.values = None
            if key in self._raw_data:
                del self._raw_data[key]
        elif control_type == AppBaseActionFormV2.CTL_TYPE_FILE:
            control.value = None
        else:
            control.value = ""

    def clear_controls(self):
        """Clear all controls on the form"""
        self.parentApp.set_control_callback_handler(None)
        for key, _ in self._controls.items():
            self.clear_control(key)


class AppBasePopup(np.ActionPopup):
    """Customised version of the npyscreen action popup

    1. Each control is uniquely identified
    2. The collection of controls on the form is searchable by identifier to allow reaction to events on specific
       controls
    """

    CTL_TYPE_TEXT = 1

    DEFAULT_LINES = 20
    DEFAULT_COLUMNS = 60

    SHOW_ATX = 10
    SHOW_ATY = 2

    def __init__(self, name=None, parentApp=None, framed=None, help=None, color='FORMDEFAULT',
                 widget_list=None, cycle_widgets=False, *args, **keywords):
        self._controls = {}
        super(AppBasePopup, self).__init__(name, parentApp, framed, help, color, widget_list, cycle_widgets, args, keywords)

    def add_control(self, key, control, control_type):
        """Add a named control to the form

        :param key: The (unique) identifier of the control
        :param control: The control object
        :param control_type: The type of control
        :return: None
        """
        self._controls.update({key: (control, control_type, control.name)})

    def add_text(self, text, blank_before=False, blank_after=False):
        """Add static text to the form

        :param text: The text to add
        :param blank_before: If True, a blank line will be added before the static text
        :param blank_after: If true, a blank line will be added after the static text
        :return: None
        """
        if blank_before:
            self.add(np.FixedText, value="", editable=False)
        self.add(np.FixedText, value=text, editable=False)
        if blank_after:
            self.add(np.FixedText, value="", editable=False)

    def add_text_box(self, key, name, indent, editable=True):
        """Add a titled text box to the form

        :param key: The (unique) identifier for the text box
        :param name: The title text
        :param indent: The left-indent at which data entry starts
        :param editable: If True, the text box is editable, if False it can only be set programatically
        :return: None
        """
        control = self.add(AppBaseTitleText, name=name, value="", use_two_lines=False, begin_entry_at=indent,
                           editable=editable)
        self.add_control(key, control, AppBasePopup.CTL_TYPE_TEXT)

    def get_control(self, key):
        """Return a control given its identifier

        :param key: Unique control identifier
        :return: Control object instance
        """
        return self._controls[key][0]

    def set_value(self, key, value):
        """Set the value for a specified control

        :param key: Unique control identifier
        :param value: Value to set
        :return: None
        """
        control = self.get_control(key)
        control.value = value

    def on_ok(self):
        """Callback handler for the OK button. This returns to the previous form"""
        self.parentApp.switchFormPrevious()

    def on_cancel(self):
        """Callback handler for the OK button. This returns to the previous form"""
        self.parentApp.switchFormPrevious()
