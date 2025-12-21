using System;
using AMT.Domain.Models;
using AMT.Infrastructure.Data;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces.Repos;
using AMT.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace AMT.Infrastructure.Repository;

public class FlightRepository(AirportManagementContext context) : IFlightRepository
{
    public bool ActiveFlightExists(string flightNumber)
    {
        return context.Flights.Any(f => f.FlightNumber == flightNumber && (f.IsActive == null || f.IsActive == true));
    }

    public async Task AddAsync(Flight entity)
    {
        await context.AddAsync(FlightMap.ToEntity(entity));
    }

    public void Delete(int id)
    {
        var entity = context.Flights.Find(id);
        if (entity != null)
        {
            context.Flights.Remove(entity);
        }
        else
        {
            throw new GenericNotFound<Flight, int>(id);
        }
    }

    public IEnumerable<Flight> GetAll()
    {
        return context.Flights.Select(FlightMap.ToDomain);
    }

    public Flight? GetById(int id)
    {
        return context.Flights
            .Include(f => f.Airline)
            .Include(f => f.OriginAirport)
            .Include(f => f.DestinationAirport)
            .Include(f => f.DefaultAircraft)
                .ThenInclude(a => a!.OwnedByAirline)
            .Where(f => f.Id == id)
            .Select(FlightMap.ToDomain)
            .FirstOrDefault();
    }

    public Flight? GetFlightByFlightNumber(string flightNumber)
    {
        return context.Flights
            .Include(f => f.Airline)
            .Include(f => f.OriginAirport)
            .Include(f => f.DestinationAirport)
            .Include(f => f.DefaultAircraft)
                .ThenInclude(a => a!.OwnedByAirline)
            .Where(f => f.FlightNumber == flightNumber)
            .Select(FlightMap.ToDomain)
            .FirstOrDefault();
    }

    public void Update(Flight entity)
    {
        var oldEntity = context.Flights.Find(entity.Id);
        if (oldEntity != null)
        {
            context.Entry(oldEntity).CurrentValues.SetValues(FlightMap.ToEntity(entity));
        }
        else
        {
            throw new GenericNotFound<Flight, int>(entity.Id);
        }
    }
}
