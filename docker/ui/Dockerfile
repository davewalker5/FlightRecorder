FROM mcr.microsoft.com/dotnet/aspnet:latest AS runtime
ARG VERSION
COPY flightrecorder.mvc-${VERSION}.0 /opt/flightrecorder.mvc
WORKDIR /opt/flightrecorder.mvc/bin
ENTRYPOINT [ "./FlightRecorder.Mvc" ]
