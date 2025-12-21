DELETE FROM [AirportManagement].[dbo].[Flight]
WHERE Id = 1;


SELECT TOP (1000) [Id]
      ,[AirlineId]
      ,[FlightNumber]
      ,[OriginAirportId]
      ,[DestinationAirportId]
      ,[DefaultAircraftId]
      ,[IsActive]
  FROM [AirportManagement].[dbo].[Flight]


SELECT * FROM [AirportManagement].[dbo].[Aircraft];
SELECT * FROM [AirportManagement].[dbo].[Airport];
SELECT * FROM [AirportManagement].[dbo].[Airline];
SELECT * FROM [AirportManagement].[dbo].[Gate];
SELECT * FROM [AirportManagement].[dbo].[FlightSchedule];
SELECT * FROM [AirportManagement].[dbo].[Flight];
SELECT * FROM [AirportManagement].[dbo].[Ticket];
SELECT * FROM [AirportManagement].[dbo].[Booking];

DELETE FROM [AirportManagement].[dbo].[FlightSchedule]
WHERE Id != 1;
