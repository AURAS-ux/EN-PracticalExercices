-- CREATE DATABASE AirportManagement;

CREATE TABLE Airport(
    Id INT PRIMARY KEY IDENTITY(1,1),
    IATACode NCHAR(3) NOT NULL UNIQUE,
    Name NVARCHAR(120) NOT NULL,
    City NVARCHAR(80),
    Country NVARCHAR(80),
    Timezone NVARCHAR(64)
);

-- Sample data for Airport
CREATE TABLE Gate(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Code NVARCHAR(10) NOT NULL,
    AirportId INT NOT NULL,
    FOREIGN KEY (AirportId) REFERENCES Airport(Id)
);

CREATE TABLE Airline(
    Id INT PRIMARY KEY IDENTITY(1,1),
    IATACode NCHAR(2) NOT NULL UNIQUE,
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Aircraft(
    Id INT PRIMARY KEY IDENTITY(1,1),
    TailNumber NVARCHAR(10) NOT NULL UNIQUE,
    Model NVARCHAR(60) NOT NULL,
    SeatCapacity INT NOT NULL,
    OwnedByAirlineId INT NOT NULL,
    FOREIGN KEY (OwnedByAirlineId) REFERENCES Airline(Id)
);

CREATE TABLE Flight(
    Id INT PRIMARY KEY IDENTITY(1,1),
    AirlineId INT NOT NULL,
    FlightNumber NVARCHAR(8) NOT NULL,
    OriginAirportId INT NOT NULL,
    DestinationAirportId INT NOT NULL,
    DefaultAircraftId INT NULL,
    IsActive BIT DEFAULT 1,
    FOREIGN KEY (AirlineId) REFERENCES Airline(Id),
    FOREIGN KEY (DefaultAircraftId) REFERENCES Aircraft(Id),
    FOREIGN KEY (OriginAirportId) REFERENCES Airport(Id),
    FOREIGN KEY (DestinationAirportId) REFERENCES Airport(Id)
);

CREATE TABLE Ticket(
    Id INT PRIMARY KEY IDENTITY(1,1),
    FlightId INT NOT NULL,
    FareClass NVARCHAR(2) NOT NULL,
    BasePrice DECIMAL(10,2) NOT NULL,
    Taxes DECIMAL(10,2) NOT NULL,
    TotalPrice AS (BasePrice + Taxes) PERSISTED, -- wont appear in the domain entity
    Currency NVARCHAR(3) NOT NULL,
    IsRefundable BIT DEFAULT 0,
    SeatInventory INT NOT NULL,
    FOREIGN KEY (FlightId) REFERENCES Flight(Id)
);

CREATE TABLE Booking(
    Id INT PRIMARY KEY IDENTITY(1,1),
    FlightId INT NOT NULL,
    TicketId INT NOT NULL,
    PassagerFullName NVARCHAR(120) NOT NULL,
    PassagerEmail NVARCHAR(120) NOT NULL,
    ConfirmationCode NVARCHAR(8) NOT NULL UNIQUE,
    Quantity INT NOT NULL,
    Status TINYINT NOT NULL,
    CreatedUtc DATETIME2 DEFAULT SYSUTCDATETIME(),
    FOREIGN KEY (FlightId) REFERENCES Flight(Id),
    FOREIGN KEY (TicketId) REFERENCES Ticket(Id)
);

CREATE TABLE FlightSchedule(
    Id INT PRIMARY KEY IDENTITY(1,1),
    FlightId INT NOT NULL,
    ScheduledDepartureUtc DATETIME2 NOT NULL,
    ScheduledArrivalUtc DATETIME2 NOT NULL,
    GateId INT NULL,
    AssignedAircraftId INT NULL,
    Status TINYINT NOT NULL,
    FOREIGN KEY (FlightId) REFERENCES Flight(Id),
    FOREIGN KEY (GateId) REFERENCES Gate(Id),
    FOREIGN KEY (AssignedAircraftId) REFERENCES Aircraft(Id)
);