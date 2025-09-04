FROM mcr.microsoft.com/dotnet/core/aspnet:latest
COPY flightrecorder.api-1.21.0.0 /opt/flightrecorder.api
WORKDIR /opt/flightrecorder.api/bin
ENTRYPOINT [ "./FlightRecorder.Api" ]
