FROM mcr.microsoft.com/dotnet/core/aspnet:latest
ARG VERSION
COPY flightrecorder.api-${VERSION}.0 /opt/flightrecorder.api
WORKDIR /opt/flightrecorder.api/bin
ENTRYPOINT [ "./FlightRecorder.Api" ]
