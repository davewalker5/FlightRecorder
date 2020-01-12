from flightrecorder.appbase.configbase import AppBaseConfiguration


class Configuration(AppBaseConfiguration):
    def __init__(self):
        super(Configuration, self).__init__()


if __name__ == "__main__":
    config = Configuration()
    config.print_config()
