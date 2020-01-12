from flightrecorder.appbase.constants import *
from flightrecorder.db.dbdatabase import FlightRecorderDatabase
import csv
import sys
import datetime


COLUMN_TITLES = ["Flight",
                 "Airline",
                 "Registration",
                 "Serial Number",
                 "Manufacturer",
                 "Type",
                 "Age",
                 "From",
                 "To",
                 "Height",
                 "Date",
                 "Location"]

CI_FLIGHT_NUMBER = 0
CI_AIRLINE = 1
CI_REGISTRATION = 2
CI_SERIAL_NUMBER = 3
CI_MANUFACTURER = 4
CI_MODEL = 5
CI_AGE = 6
CI_EMBARKATION = 7
CI_DESTINATION = 8
CI_ALTITUDE = 9
CI_DATE = 10
CI_LOCATION = 11


def sighting_to_csv(sighting):
    """Construct and return a CSV-formatted string representing a Sighting object instance

    :param sighting: The Sighting object instance
    :return: CSV-formatted string representing the sighting
    """
    return [sighting.flight().number(),
            sighting.flight().airline().name(),
            sighting.aircraft().registration(),
            sighting.aircraft().serial_number(),
            sighting.aircraft().model().manufacturer().name(),
            sighting.aircraft().model().name(),
            sighting.aircraft().age(),
            sighting.flight().embarkation(),
            sighting.flight().destination(),
            sighting.altitude(),
            sighting.date().strftime(DATE_FORMAT),
            sighting.location().name()]


def console_progress(record, flight_number, date, registration, manufacturer, model, status):
    """Write import/export progress to the console"""
    if record is not None:
        print("{:0>4} : Processing sighting of flight {} on {}".format(record, flight_number, date))


def import_csv(filename, progress, db):
    """Import a CSV file

    :param filename: String containing the absolute path to the CSV file to import
    :param progress: Progress reporting callback
    :param db: An instance of the FlightRecorderDatabase object
    :return: None
    """
    with open(filename, mode="rt", newline="") as csvfile:
        reader = csv.reader(csvfile, delimiter=",")

        i = 0
        for row in reader:
            # First row is a header
            if i > 0:
                if progress is not None:
                    progress(i, row[CI_FLIGHT_NUMBER], row[CI_DATE], row[CI_REGISTRATION],
                             row[CI_MANUFACTURER], row[CI_MODEL], "In Progress")

                # The CSV file contains an age. This is converted to a manufactured date as the age will
                # increase over time so what is written to the database needs to be the manufactured date in
                # order that the age is always reported correctly
                manufactured = datetime.datetime.now().year - int(row[CI_AGE])
                aircraft_id = db.create_aircraft(row[CI_REGISTRATION], row[CI_SERIAL_NUMBER], manufactured,
                                                 row[CI_MODEL], row[CI_MANUFACTURER]).db_id()

                flight_id = db.create_flight(row[CI_FLIGHT_NUMBER], row[CI_EMBARKATION], row[CI_DESTINATION],
                                             row[CI_AIRLINE]).db_id()

                location_id = db.create_location(row[CI_LOCATION]).db_id()

                date = datetime.datetime.strptime(row[CI_DATE], DATE_FORMAT)
                db.create_sighting(row[CI_ALTITUDE], date, location_id, flight_id, aircraft_id)
            i += 1

        if progress is not None:
            progress(None, None, None, None, None, None, "Completed")


def export_csv(filename, progress, db):
    """Export the database to a CSV file

    :param filename: String containing the absolute path to the CSV file to export to
    :param progress: Progress reporting callback
    :param db: An instance of the FlightRecorderDatabase object
    :return: None
    """
    with open(filename, mode="wt", newline="") as csvfile:
        writer = csv.writer(csvfile, delimiter=",")

        # First row is a header
        writer.writerow(COLUMN_TITLES)

        i = 1
        for sighting in db.sighting_repo().read_all():
            row = sighting_to_csv(sighting)

            if progress is not None:
                progress(i, row[CI_FLIGHT_NUMBER], row[CI_DATE], row[CI_REGISTRATION],
                         row[CI_MANUFACTURER], row[CI_MODEL], "In Progress")

            writer.writerow(row)
            i += 1

        if progress is not None:
            progress(None, None, None, None, None, None, "Completed")


if __name__ == "__main__":
    database = FlightRecorderDatabase()
    import_csv(sys.argv[1], console_progress, database)
