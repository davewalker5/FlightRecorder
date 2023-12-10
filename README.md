# FlightRecorder

[![Build Status](https://github.com/davewalker5/FlightRecorder/workflows/.NET%20Core%20CI%20Build/badge.svg)](https://github.com/davewalker5/FlightRecorder/actions)
[![GitHub issues](https://img.shields.io/github/issues/davewalker5/FlightRecorder)](https://github.com/davewalker5/FlightRecorder/issues)
[![Coverage Status](https://coveralls.io/repos/github/davewalker5/FlightRecorder/badge.svg?branch=master)](https://coveralls.io/github/davewalker5/FlightRecorder?branch=master)
[![Releases](https://img.shields.io/github/v/release/davewalker5/FlightRecorder.svg?include_prereleases)](https://github.com/davewalker5/FlightRecorder/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/davewalker5/FlightRecorder/blob/master/LICENSE)
[![Language](https://img.shields.io/badge/language-c%23-blue.svg)](https://github.com/davewalker5/FlightRecorder/)
[![Language](https://img.shields.io/badge/database-SQLite-blue.svg)](https://github.com/davewalker5/FlightRecorder/)
[![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/davewalker5/FlightRecorder)](https://github.com/davewalker5/FlightRecorder/)

## About

FlightRecorder implements a SQL-based aircraft sightings logbook. An ASP.NET WebAPI provides access to the business logic and data access layer while an ASP.NET MVC UI provides the user interface.

### Sightings
Each sighting consists of the following data:

- Flight details
  - Flight number
  - Embarkation airport IATA code
  - Destination IATA code
  - Airline
- Aircraft details
  - Registration details
  - Manufacturer
  - Model
- Sighting details
  - Date
  - Altitude when sighted
  - Location

A register of airport codes by country is used to validate airport IATA codes.

### Search

Sightings may be searched by:

- Route, specified as airport IATA codes
- Flight number
- Airline name
- Aircraft registration number
- Date

### Reporting
The following reports can be generated and exported to CSV format files:

- Airline statistics
- Flights by month
- Location statistics
- Manufacturer statistics
- Aircraft model statistics

### Data Export
The following data can be exported in CSV format:

- Sightings
- Airport code and country list

## Getting Started

Please see the [Wiki](https://github.com/davewalker5/FlightRecorder/wiki) for configuration details and the user guide.

## Authors

- **Dave Walker** - _Initial work_ - [LinkedIn](https://www.linkedin.com/in/davewalker5/)

## Credits

Implementation of authentication using JWT in the REST API is based on the following tutorial:

- https://github.com/cornflourblue/aspnet-core-3-jwt-authentication-api
- https://jasonwatmore.com/post/2019/10/11/aspnet-core-3-jwt-authentication-tutorial-with-example-api#users-controller-cs

The Flight Recorder MVC project uses the Gijgo JavaScript controls library:

- https://gijgo.com

## Feedback

To file issues or suggestions, please use the [Issues](https://github.com/davewalker5/FlightRecorder/issues) page for this project on GitHub.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
