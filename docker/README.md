# flightrecorderapisqlite

The [FlightRecorderDb](https://github.com/davewalker5/FlightRecorderDb) and [FlightRecorder.Api](https://github.com/davewalker5/FlightRecorder.Api) GitHub projects implement the entities, business logic and a REST service for a SQL-based aircraft sightings logbook, providing facilities for recording and querying the following data:

- Manufacturer, model and aircraft registration details
- Airline and flight details
- Sighting locations
- Aircraft sightings

The flightrecorderapisqlite image contains a build of the logic and REST service for a SQLite database.

## Getting Started

### Prerequisities

In order to run this image you'll need docker installed.

- [Windows](https://docs.docker.com/windows/started)
- [OS X](https://docs.docker.com/mac/started/)
- [Linux](https://docs.docker.com/linux/started/)

### Usage

#### Container Parameters

The following "docker run" parameters are recommended when running the flightrecorderapisqlite image:

| Parameter | Value                                      | Purpose                                                 |
| --------- | ------------------------------------------ | ------------------------------------------------------- |
| -d        | -                                          | Run as a background process                             |
| -v        | /local:/var/opt/flightrecorder.api-1.0.0.0 | Mount the host folder containing the SQLite database    |
| -p        | 5001:80                                    | Expose the container's port 80 as port 5001 on the host |
| --rm      | -                                          | Remove the container automatically when it stops        |

For example:

```shell
docker run -d -v  /local:/var/opt/flightrecorder.api-1.0.0.0 -p 5001:80 --rm  davewalker5/flightrecorderapisqlite:latest
```

The "/local" path given to the -v argument is described, below, and should be replaced with a value appropriate for the host running the container. Similarly, the port number "5001" can be replaced with any available port on the host.

#### Volumes

The description of the container parameters, above, specifies that a folder containing the SQLite database file for the Flight Recorder database is mounted in the running container, using the "-v" parameter.

That folder should contain a SQLite database that has been created using the instructions in the [Flight Recorder wiki](https://github.com/davewalker5/FlightRecorderDb/wiki).

Specifically, the following should be done:

- [Create the SQLite database](https://github.com/davewalker5/FlightRecorderDb/wiki/Using-a-SQLite-Database)
- [Add a user to the database](https://github.com/davewalker5/FlightRecorderDb/wiki/REST-API)

The folder containing the "flightrecorder.db" file can then be passed to the "docker run" command using the "-v" parameter.

#### Running the Image

To run the image, enter the following command, substituting "/local" for the host folder containing the SQLite database, as described:

```shell
docker run -d -v  /local:/var/opt/flightrecorder.api-1.0.0.0/ -p 5001:80 --rm  davewalker5/flightrecorderapisqlite:latest
```

Once the container is running, browse to the following URL on the host:

http://localhost:5001

You should see the Swagger API documentation for the API.

## Built With

The flightrecorderapisqlite image was been built with the following:

| Aspect         | Version                |
| -------------- | ---------------------- |
| .NET Core CLI  | 3.1.101                |
| Target Runtime | linux-x64              |
| Docker Desktop | 19.03.5, build 633a0ea |

## Find Us

- [FlightRecorderDb on GitHub](https://github.com/davewalker5/FlightRecorderDb)
- [FlightRecorder.Api on GitHub](https://github.com/davewalker5/FlightRecorder.Api)

## Versioning

For the versions available, see the [tags on this repository](https://github.com/davewalker5/FlightRecorder.Api/tags).

## Authors

- **Dave Walker** - _Initial work_ - [LinkedIn](https://github.com/)

See also the list of [contributors](https://github.com/davewalker5/FlightRecorder.Api/contributors) who
participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/davewalker5/FlightRecorder.Api/blob/master/LICENSE) file for details.
