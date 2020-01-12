from sqlalchemy import Column, ForeignKey, Integer, String, DateTime
from sqlalchemy.ext.declarative import declarative_base

Base = declarative_base()


class AirlineModel(Base):
    """SQLAlchemy model for the airline entity"""

    __tablename__ = 'AIRLINE'
    id = Column(Integer, primary_key=True)
    name = Column(String(100), nullable=False)


class LocationModel(Base):
    """SQLAlchemy model for the observation location entity"""

    __tablename__ = 'LOCATION'
    id = Column(Integer, primary_key=True)
    name = Column(String(100), nullable=False)


class ManufacturerModel(Base):
    """SQLAlchemy model for the manufacturer entity"""

    __tablename__ = 'MANUFACTURER'
    id = Column(Integer, primary_key=True)
    name = Column(String(100), nullable=False)


class ModelModel(Base):
    """SQLAlchemy model for the aircraft model entity"""

    __tablename__ = 'MODEL'
    id = Column(Integer, primary_key=True)
    manufacturer_id = Column(Integer, ForeignKey('MANUFACTURER.id'))
    name = Column(String(100), nullable=False)


class AircraftModel(Base):
    """SQLAlchemy model for the aircraft entity"""

    __tablename__ = 'AIRCRAFT'
    id = Column(Integer, primary_key=True)
    model_id = Column(Integer, ForeignKey('MODEL.id'))
    registration = Column(String(50), nullable=False)
    serial_number = Column(String(50), nullable=False)
    manufactured = Column(Integer, nullable=False)


class FlightModel(Base):
    """SQLAlchemy model for the flight entity"""

    __tablename__ = 'FLIGHT'
    id = Column(Integer, primary_key=True)
    airline_id = Column(Integer, ForeignKey('AIRLINE.id'))
    number = Column(String(50), nullable=False)
    embarkation = Column(String(3), nullable=False)
    destination = Column(String(3), nullable=False)


class SightingModel(Base):
    """SQLAlchemy model for the sighting, or observation, entity"""

    __tablename__ = 'SIGHTING'
    id = Column(Integer, primary_key=True)
    location_id = Column(Integer, ForeignKey('LOCATION.id'))
    flight_id = Column(Integer, ForeignKey('FLIGHT.id'))
    aircraft_id = Column(Integer, ForeignKey('AIRCRAFT.id'))
    altitude = Column(Integer, nullable=False)
    date = Column(DateTime, nullable=False)