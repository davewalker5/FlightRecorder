FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
COPY flightrecorder.mvc-1.0.0.3 /opt/flightrecorder.mvc-1.0.0.3
WORKDIR /opt/flightrecorder.mvc-1.0.0.3/bin
ENTRYPOINT [ "./FlightRecorder.Mvc" ]
