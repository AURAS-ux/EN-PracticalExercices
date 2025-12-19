using System;
using System.Globalization;
using AMT.Domain.Models;

namespace AMT.Application.Dtos.Extentions;

public static class ScheduleExtention
{
    /// <summary>
    /// Maps <see cref="CreateScheduleDto"/> to <see cref="FlightSchedule"/>.
    /// Requires resolved related entities: <see cref="Flight"/>, <see cref="Gate"/>, <see cref="Aircraft"/>.
    /// Throws when date strings cannot be parsed to UTC timestamps.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when a provided date string is null.</exception>
    /// <exception cref="FormatException">Thrown when a provided date string is not a valid date/time format.</exception>
    /// <exception cref="ArgumentException">Thrown when an argument is otherwise invalid for parsing.</exception>
	public static FlightSchedule ToFlightSchedule(
		this CreateScheduleDto dto,
		Flight flight,
		Gate gate,
		Aircraft assignedAircraft)
	{
        try{
            var departure = DateTime.Parse(dto.ScheduleDepartureUtc, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
            var arrival = DateTime.Parse(dto.ScheduleArrivalUtc, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
        

            return new FlightSchedule
            {
                Flight = flight,
                ScheduledDepartureUtc = departure,
                ScheduledArrivalUtc = arrival,
                Gate = gate,
                AssignedAircraft = assignedAircraft,
                Status = (FlightSchedule.ScheduleStatus)dto.Status
            };
        }catch(ArgumentNullException ex){
            throw new ArgumentNullException("DateTime parse error in ScheduleExtention", ex);
        }catch(FormatException ex){
            throw new FormatException("DateTime parse error in ScheduleExtention", ex);
        }catch(ArgumentException ex){
            throw new ArgumentException("DateTime parse error in ScheduleExtention", ex);
        }
	}
}
