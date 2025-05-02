# flightrecordermvc

The [FlightRecorder](https://github.com/davewalker5/FlightRecorder) GitHub project implements a REST service and associated web-based UI for a SQL-based aircraft sightings logbook, providing facilities for recording and querying the following data:

- Manufacturer, model and aircraft registration details
- Airline and flight details
- Sighting locations
- Aircraft sightings

The flightrecordermvc image contains a build of the UI and is intended to be run as a pair with the associated web service.

## Getting Started

### Prerequisities

In order to run this image you'll need docker installed.

- [Windows](https://docs.docker.com/windows/started)
- [OS X](https://docs.docker.com/mac/started/)
- [Linux](https://docs.docker.com/linux/started/)

### Usage

#### Service Container Parameters

An instance of the flightrecorderapisqlite image must be started first in order for the UI to work. The recommended parameters are:

| Parameter | Value                              | Purpose                                              |
| --------- | ---------------------------------- | ---------------------------------------------------- |
| -d        | -                                  | Run as a background process                          |
| -v        | /local:/var/opt/flightrecorder.api | Mount the host folder containing the SQLite database |
| --name    | flightrecorderservice              | Name the service so the UI can find it               |
| --rm      | -                                  | Remove the container automatically when it stops     |

The "/local" path given to the -v argument is described, below, and should be replaced with a value appropriate for the host running the container.

The "--name" parameter is mandatory as the service URL is held in the application settings for the UI image and is expected to be:

http://flightrecorderservice:80

#### UI Container Parameters

The following "docker run" parameters are recommended when running the flightrecordermvc image:

| Parameter | Value                 | Purpose                                                 |
| --------- | --------------------- | ------------------------------------------------------- |
| -d        | -                     | Run as a background process                             |
| -p        | 5001:80               | Expose the container's port 80 as port 5001 on the host |
| --link    | flightrecorderservice | Link to the drone flight log service container          |
| --rm      | -                     | Remove the container automatically when it stops        |

For example:

```shell
docker run -d -p 5001:80 --rm --link flightrecorderservice davewalker5/flightrecordermvc:latest
```

The port number "5001" can be replaced with any available port on the host.

#### Volumes

The description of the container parameters, above, specifies that a folder containing the SQLite database file for the Drone Flight Log is mounted in the running container, using the "-v" parameter.

That folder should contain a SQLite database that has been created using the instructions in the [Flight Recorder wiki](https://github.com/davewalker5/FlightRecorderDb/wiki).

Specifically, the following should be done:

- [SQLite Database Configuration](https://github.com/davewalker5/FlightRecorder/wiki/Database#sqlite-database)

The folder containing the "flightrecorder.db" file can then be passed to the "docker run" command using the "-v" parameter.

#### Running the Image

To run the images for the service and UI, enter the following commands, substituting "/local" for the host folder containing the SQLite database, as described:

```shell
docker run -d -v  /local:/var/opt/flightrecorder.api/ --name flightrecorderservice --rm  davewalker5/flightrecorderapisqlite:latest
docker run -d -p 5001:80 --rm --link flightrecorderservice davewalker5/flightrecordermvc:latest
```

Once the container is running, browse to the following URL on the host:

http://localhost:5001

You should see the login page for the UI.

## Find Us

- [FlightRecorder on GitHub](https://github.com/davewalker5/FlightRecorder)

## Versioning

For the versions available, see the [tags on this repository](https://github.com/davewalker5/FlightRecorder/tags).

## Authors

- **Dave Walker** - _Initial work_ - [LinkedIn](https://www.linkedin.com/in/davewalker5/)

See also the list of [contributors](https://github.com/davewalker5/FlightRecorder/contributors) who
participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/davewalker5/FlightRecorder/blob/master/LICENSE) file for details.
