import yaml
import os


class AppBaseConfiguration:
    """Configuration file access, providing access to base configuration keys"""

    CONFIG_FILE = "config.yml"
    KEY_APPLICATION = "application"
    KEY_DATABASE = "database"
    KEY_DEBUG = "debug"

    def __init__(self):
        # The project structure is:
        #
        # . -- app
        #          |
        #          +-- appbase
        #          |     |
        #          |     +-- configbase.py
        #          |
        #          +-- config
        #          |     |
        #          |     +-- configuration.py
        #          |
        #          +-- config.yml
        #
        # The application is required to override this base configuration class with it's
        # own version, conforming to the above structure.
        #
        # The following code gets the absolute path to the config.yml file relative to the
        # current executing module (which is configuration.py)

        self._app_root = os.path.dirname(os.path.dirname(os.path.realpath(__file__)))
        self._config_filename = os.path.join(self._app_root, AppBaseConfiguration.CONFIG_FILE)
        self._config = yaml.safe_load(open(self._config_filename))

    def _get_full_file_path(self, section_key, filename_key, relative_to_app_root):
        """Get the fill path to a configuration file

        Note that this method assumes the configuration section specified in the parameters will contain a "path"
        key

        :param section_key: Name of the configuration section containing the filename and path
        :param filename_key: Key for the filename
        :param relative_to_app_root: True if the path is relative to the application base path
        :return:
        """
        path = self._config[section_key]["path"]
        filename = self._config[section_key][filename_key]
        if relative_to_app_root:
            full_path = os.path.join(self._app_root, path, filename)
        else:
            full_path = os.path.join(path, filename)
        return os.path.abspath(full_path)

    def app_root(self):
        """Return the root folder for the application"""
        return self._app_root

    def app_version(self):
        """Return the application version"""
        return self._config[AppBaseConfiguration.KEY_APPLICATION]["version"]

    def database_name(self):
        """Return the absolute path to the database file"""

        # Get the relative path to the database. If it's not set, the database path isn't set
        relative_path = self._config[AppBaseConfiguration.KEY_DATABASE]["path"]
        if relative_path is not None and len(relative_path) > 0:
            # Append the application root path to it
            if relative_path[0] != os.sep:
                relative_path = self.app_root() + "/" + relative_path

            # Construct the full path to the database file
            path = os.path.abspath(relative_path)
            filename = self._config[AppBaseConfiguration.KEY_DATABASE]["filename"]
            database_name = os.path.join(path, filename)
        else:
            database_name = None

        return database_name

    def set_database_location(self, path):
        """Set the absolute path to the database file"""

        self._config[AppBaseConfiguration.KEY_DATABASE]["path"] = path
        with open(self._config_filename, 'w') as f:
            yaml.dump(self._config, f)

    def log_file_name(self):
        """Return the absolute path to the debug log file"""
        return self._get_full_file_path(AppBaseConfiguration.KEY_DEBUG, "logfile", False)

    def print_config(self):
        print("App Root:\t", self.app_root())
        print("Version:\t", self.app_version())
        print("Database:\t", self.database_name())
        print("Log File:\t", self.log_file_name())
