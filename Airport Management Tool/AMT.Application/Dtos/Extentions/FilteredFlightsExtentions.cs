using System;
using AMT.Domain.Models;

namespace AMT.Application.Dtos.Extentions;

public static class FilteredFlightsExtentions
{
    public static FilteredFlightsDto ToFilteredFlightsDto(this FlightSchedule flightSchedule)
    {
        return new FilteredFlightsDto
        {
            FlightNumber = flightSchedule.Flight.FlightNumber,
            Origin = flightSchedule.Flight.OriginAirport.IATACode,
            Destination = flightSchedule.Flight.DestinationAirport.IATACode,
            DepartureTime = flightSchedule.ScheduledDepartureUtc,
            ArrivalTime = flightSchedule.ScheduledArrivalUtc
        };
    }
}
