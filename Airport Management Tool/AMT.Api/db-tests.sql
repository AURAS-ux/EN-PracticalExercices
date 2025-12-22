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

SELECT * FROM [AirportManagement].[dbo].[AspNetUsers];
SELECT * FROM [AirportManagement].[dbo].[AspNetRoles];
SELECT * FROM [AirportManagement].[dbo].[AspNetUserRoles];

INSERT INTO [AirportManagement].[dbo].[AspNetUserRoles] (UserId, RoleId)
VALUES ('5f451d86-cc1e-43ef-8530-e26e749f5a18', 'F969ED41-632B-42CA-86DD-61AD1C8362E3');
