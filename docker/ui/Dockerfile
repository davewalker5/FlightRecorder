FROM mcr.microsoft.com/dotnet/aspnet:latest AS runtime
COPY flightrecorder.mvc-1.24.1.0 /opt/flightrecorder.mvc
WORKDIR /opt/flightrecorder.mvc/bin
ENTRYPOINT [ "./FlightRecorder.Mvc" ]
