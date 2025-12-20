DELETE FROM [AirportManagement].[dbo].[Flight]
WHERE Id = 1;

INSERT INTO Gate ([AirportId], [Code])
VALUES (1, 'A1'), (1, 'A2'), (2, 'B1'), (2, 'B2');


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
