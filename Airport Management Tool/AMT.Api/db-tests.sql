SELECT TOP (1000) [Id]
      ,[AirlineId]
      ,[FlightNumber]
      ,[OriginAirportId]
      ,[DestinationAirportId]
      ,[DefaultAircraftId]
      ,[IsActive]
  FROM [AirportManagement].[dbo].[Flight]

DELETE FROM [AirportManagement].[dbo].[Flight]
WHERE Id = 1;

SELECT * FROM [AirportManagement].[dbo].[Aircraft];
SELECT * FROM [AirportManagement].[dbo].[Airport];
SELECT * FROM [AirportManagement].[dbo].[Airline];
