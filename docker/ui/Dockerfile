FROM mcr.microsoft.com/dotnet/aspnet:latest AS runtime
COPY flightrecorder.mvc-1.9.1.0 /opt/flightrecorder.mvc-1.9.1.0
WORKDIR /opt/flightrecorder.mvc-1.9.1.0/bin
ENTRYPOINT [ "./FlightRecorder.Mvc" ]
