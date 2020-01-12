from flightrecorder.config.configuration import Configuration


if __name__ == "__main__":
    # Make sure the database location has been blanked
    config = Configuration()
    config.set_database_location("")
