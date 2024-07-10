FROM mcr.microsoft.com/dotnet/aspnet:latest AS runtime
COPY flightrecorder.mvc-1.10.0.0 /opt/flightrecorder.mvc-1.10.0.0
WORKDIR /opt/flightrecorder.mvc-1.10.0.0/bin
ENTRYPOINT [ "./FlightRecorder.Mvc" ]
