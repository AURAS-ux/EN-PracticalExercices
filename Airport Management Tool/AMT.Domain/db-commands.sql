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


---------------------------------------- INDEX CREATION ----------------------------------------
-- Gate(AirportId, Code) unique index
IF NOT EXISTS (
  SELECT 1 FROM sys.indexes
  WHERE name = 'UX_Gate_Airport_Code'
    AND object_id = OBJECT_ID('dbo.Gate')
)
BEGIN
  CREATE UNIQUE INDEX UX_Gate_Airport_Code ON dbo.Gate (AirportId, Code);
END
GO

-- Flight(AirlineId, FlightNumber) nonclustered index
IF NOT EXISTS (
  SELECT 1 FROM sys.indexes
  WHERE name = 'IX_Flight_Airline_FlightNumber'
    AND object_id = OBJECT_ID('dbo.Flight')
)
BEGIN
  CREATE NONCLUSTERED INDEX IX_Flight_Airline_FlightNumber
  ON dbo.Flight (AirlineId, FlightNumber);
END
GO

-- Optional: filtered index for active flights only
IF NOT EXISTS (
  SELECT 1 FROM sys.indexes
  WHERE name = 'IX_Flight_Airline_FlightNumber_Active'
    AND object_id = OBJECT_ID('dbo.Flight')
)
BEGIN
  CREATE NONCLUSTERED INDEX IX_Flight_Airline_FlightNumber_Active
  ON dbo.Flight (AirlineId, FlightNumber)
  WHERE IsActive = 1;
END
GO

-- Flight(OriginAirportId, DestinationAirportId) nonclustered index
IF NOT EXISTS (
  SELECT 1 FROM sys.indexes
  WHERE name = 'IX_Flight_Origin_Destination'
    AND object_id = OBJECT_ID('dbo.Flight')
)
BEGIN
  CREATE NONCLUSTERED INDEX IX_Flight_Origin_Destination
  ON dbo.Flight (OriginAirportId, DestinationAirportId);
END
GO

-- FlightSchedule(FlightId, ScheduledDepartureUtc) nonclustered index
IF NOT EXISTS (
  SELECT 1 FROM sys.indexes
  WHERE name = 'IX_FlightSchedule_Flight_Departure'
    AND object_id = OBJECT_ID('dbo.FlightSchedule')
)
BEGIN
  CREATE NONCLUSTERED INDEX IX_FlightSchedule_Flight_Departure
  ON dbo.FlightSchedule (FlightId, ScheduledDepartureUtc);
END
GO

-- Gate overlap prevention basics:
-- 1) Check constraint: Arrival after Departure
IF NOT EXISTS (
  SELECT 1
  FROM sys.check_constraints
  WHERE name = 'CK_FlightSchedule_DepartureBeforeArrival'
    AND parent_object_id = OBJECT_ID('dbo.FlightSchedule')
)
BEGIN
  ALTER TABLE dbo.FlightSchedule
  ADD CONSTRAINT CK_FlightSchedule_DepartureBeforeArrival
  CHECK (ScheduledArrivalUtc > ScheduledDepartureUtc);
END
GO

-- 2) Optional: prevent exact duplicate departures at same gate
IF NOT EXISTS (
  SELECT 1 FROM sys.indexes
  WHERE name = 'UX_FlightSchedule_Gate_Departure'
    AND object_id = OBJECT_ID('dbo.FlightSchedule')
)
BEGIN
  CREATE UNIQUE INDEX UX_FlightSchedule_Gate_Departure
  ON dbo.FlightSchedule (GateId, ScheduledDepartureUtc)
  WHERE GateId IS NOT NULL;
END
GO

-- Ticket(FlightId, FareClass) nonclustered index
IF NOT EXISTS (
  SELECT 1 FROM sys.indexes
  WHERE name = 'IX_Ticket_Flight_FareClass'
    AND object_id = OBJECT_ID('dbo.Ticket')
)
BEGIN
  CREATE NONCLUSTERED INDEX IX_Ticket_Flight_FareClass
  ON dbo.Ticket (FlightId, FareClass);
END
GO

-- Note: Airport(IATACode), Airline(IATACode), Booking(ConfirmationCode)
-- already defined as UNIQUE in table definitions; no extra index needed.