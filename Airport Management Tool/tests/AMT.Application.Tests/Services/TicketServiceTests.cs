using AMT.Domain.Models;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces;
using AMT.Infrastructure.Interfaces.Repos;
using FluentValidation;
using Moq;
using ValidationException = AMT.Infrastructure.Exceptions.ValidationException;

namespace AMT.Application.Tests.Services;

public class TicketServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<Serilog.ILogger> _loggerMock;
    private readonly Mock<ITicketRepository> _ticketRepositoryMock;
    private readonly IValidator<Ticket> _ticketValidator;
    private readonly Application.Services.TicketService _ticketService;

    public TicketServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<Serilog.ILogger>();
        _ticketRepositoryMock = new Mock<ITicketRepository>();
        _ticketValidator = new Validators.TicketValidator();
        _ticketService = new Application.Services.TicketService(_unitOfWorkMock.Object, _loggerMock.Object, _ticketValidator);
    }

    [Fact]
    public async Task CreateTicket_NullFlight_ReturnsNotFoundResult()
    {
        // Arrange
        var createTicketDto = new Dtos.CreateTicketDto
        {
            FlightId = 1,
            FareClass = "F",
            BasePrice = 100,
            Taxes = 20,
            Currency = "USD",
            SeatInventory = 50,
            IsRefoundable = true
        };

        _unitOfWorkMock.Setup(u => u.Flights.GetById(createTicketDto.FlightId)).Returns((Flight?)null);

        // Act
        var result = await _ticketService.CreateTicket(createTicketDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        Assert.Null(result.Value);
        Assert.Single(result.ErrorMessages!);
        Assert.Equal($"Flight with ID {createTicketDto.FlightId} not found.", result.ErrorMessages![0]);
        Assert.IsType<KeyNotFoundException>(result.Exceptions![0]);
    }

    [Fact]
    public async Task CreateTicket_WrongFareClass_ReturnsBadRequest()
    {
        // Arrange
        var createTicketDto = new Dtos.CreateTicketDto
        {
            FlightId = 1,
            FareClass = "InvalidClass",
            BasePrice = 100,
            Taxes = 20,
            Currency = "USD",
            SeatInventory = 50,
            IsRefoundable = true
        };

        var defaultAircraft = new Aircraft
        {
            Id = 1,
            Model = "Boeing 737",
            SeatCapacity = 200
        };
        var flight = new Flight { Id = createTicketDto.FlightId, DefaultAircraft = defaultAircraft };

        _unitOfWorkMock.Setup(u => u.Flights.GetById(createTicketDto.FlightId)).Returns(flight);

        // Act
        var result = await _ticketService.CreateTicket(createTicketDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Null(result.Value);
        Assert.Single(result.ErrorMessages!);
        Assert.Equal("FareClass must be either 'Y', 'J', 'M', or 'F'.", result.ErrorMessages![0]);
        Assert.IsType<ValidationException>(result.Exceptions![0]);
    }

    [Fact]
    public async Task CreateTicket_CurrencyNameTooShort_ReturnsBadRequest()
    {
        // Arrange
        var createTicketDto = new Dtos.CreateTicketDto
        {
            FlightId = 1,
            FareClass = "F",
            BasePrice = 100,
            Taxes = 20,
            Currency = "US",
            SeatInventory = 50,
            IsRefoundable = true
        };

        var defaultAircraft = new Aircraft
        {
            Id = 1,
            Model = "Boeing 737",
            SeatCapacity = 200
        };
        var flight = new Flight { Id = createTicketDto.FlightId, DefaultAircraft = defaultAircraft };

        _unitOfWorkMock.Setup(u => u.Flights.GetById(createTicketDto.FlightId)).Returns(flight);

        // Act
        var result = await _ticketService.CreateTicket(createTicketDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Null(result.Value);
        Assert.Single(result.ErrorMessages!);
        Assert.Equal("Currency code must be exactly 3 characters.", result.ErrorMessages![0]);
        Assert.IsType<ValidationException>(result.Exceptions![0]);
    }

    [Fact]
    public async Task CreateTicket_CurrencyNameTooLong_ReturnsBadRequest()
    {
        // Arrange
        var createTicketDto = new Dtos.CreateTicketDto
        {
            FlightId = 1,
            FareClass = "F",
            BasePrice = 100,
            Taxes = 20,
            Currency = "USDA",
            SeatInventory = 50,
            IsRefoundable = true
        };

        var defaultAircraft = new Aircraft
        {
            Id = 1,
            Model = "Boeing 737",
            SeatCapacity = 200
        };
        var flight = new Flight { Id = createTicketDto.FlightId, DefaultAircraft = defaultAircraft };

        _unitOfWorkMock.Setup(u => u.Flights.GetById(createTicketDto.FlightId)).Returns(flight);

        // Act
        var result = await _ticketService.CreateTicket(createTicketDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Null(result.Value);
        Assert.Single(result.ErrorMessages!);
        Assert.Equal("Currency code must be exactly 3 characters.", result.ErrorMessages![0]);
        Assert.IsType<ValidationException>(result.Exceptions![0]);
    }

    [Fact]
    public async Task CreateTicket_BasePriceNegative_ReturnsBadRequest()
    {
        // Arrange
        var createTicketDto = new Dtos.CreateTicketDto
        {
            FlightId = 1,
            FareClass = "F",
            BasePrice = -100,
            Taxes = 20,
            Currency = "USD",
            SeatInventory = 50,
            IsRefoundable = true
        };

        var defaultAircraft = new Aircraft
        {
            Id = 1,
            Model = "Boeing 737",
            SeatCapacity = 200
        };
        var flight = new Flight { Id = createTicketDto.FlightId, DefaultAircraft = defaultAircraft };

        _unitOfWorkMock.Setup(u => u.Flights.GetById(createTicketDto.FlightId)).Returns(flight);

        // Act
        var result = await _ticketService.CreateTicket(createTicketDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Null(result.Value);
        Assert.Single(result.ErrorMessages!);
        Assert.Equal("BasePrice must be greater than 0.", result.ErrorMessages![0]);
        Assert.IsType<ValidationException>(result.Exceptions![0]);
    }

    [Fact]
    public async Task CreateTicket_BasePriceZero_ReturnsBadRequest()
    {
        // Arrange
        var createTicketDto = new Dtos.CreateTicketDto
        {
            FlightId = 1,
            FareClass = "F",
            BasePrice = 0,
            Taxes = 20,
            Currency = "USD",
            SeatInventory = 50,
            IsRefoundable = true
        };

        var defaultAircraft = new Aircraft
        {
            Id = 1,
            Model = "Boeing 737",
            SeatCapacity = 200
        };
        var flight = new Flight { Id = createTicketDto.FlightId, DefaultAircraft = defaultAircraft };

        _unitOfWorkMock.Setup(u => u.Flights.GetById(createTicketDto.FlightId)).Returns(flight);

        // Act
        var result = await _ticketService.CreateTicket(createTicketDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Null(result.Value);
        Assert.Single(result.ErrorMessages!);
        Assert.Equal("BasePrice must be greater than 0.", result.ErrorMessages![0]);
        Assert.IsType<ValidationException>(result.Exceptions![0]);
    }

    [Fact]
    public async Task CreateTicket_TaxesNegative_ReturnsBadRequest()
    {
        // Arrange
        var createTicketDto = new Dtos.CreateTicketDto
        {
            FlightId = 1,
            FareClass = "F",
            BasePrice = 100,
            Taxes = -20,
            Currency = "USD",
            SeatInventory = 50,
            IsRefoundable = true
        };

        var defaultAircraft = new Aircraft
        {
            Id = 1,
            Model = "Boeing 737",
            SeatCapacity = 200
        };
        var flight = new Flight { Id = createTicketDto.FlightId, DefaultAircraft = defaultAircraft };

        _unitOfWorkMock.Setup(u => u.Flights.GetById(createTicketDto.FlightId)).Returns(flight);

        // Act
        var result = await _ticketService.CreateTicket(createTicketDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Null(result.Value);
        Assert.Single(result.ErrorMessages!);
        Assert.Equal("Taxes must be greater than or equal to 0.", result.ErrorMessages![0]);
        Assert.IsType<ValidationException>(result.Exceptions![0]);
    }

    [Fact]
    public async Task CreateTicket_SeatInventoryNegative_ReturnsBadRequest()
    {
        // Arrange
        var createTicketDto = new Dtos.CreateTicketDto
        {
            FlightId = 1,
            FareClass = "F",
            BasePrice = 100,
            Taxes = 20,
            Currency = "USD",
            SeatInventory = -50,
            IsRefoundable = true
        };

        var defaultAircraft = new Aircraft
        {
            Id = 1,
            Model = "Boeing 737",
            SeatCapacity = 200
        };
        var flight = new Flight { Id = createTicketDto.FlightId, DefaultAircraft = defaultAircraft };

        _unitOfWorkMock.Setup(u => u.Flights.GetById(createTicketDto.FlightId)).Returns(flight);

        // Act
        var result = await _ticketService.CreateTicket(createTicketDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Null(result.Value);
        Assert.Single(result.ErrorMessages!);
        Assert.Equal("SeatInventory must be greater than or equal to 0.", result.ErrorMessages![0]);
        Assert.IsType<ValidationException>(result.Exceptions![0]);
    }

    [Fact]
    public async Task CreateTicket_SeatInventorySmallerThanSeatCapacity_ReturnsBadRequest()
    {
        // Arrange
        var createTicketDto = new Dtos.CreateTicketDto
        {
            FlightId = 1,
            FareClass = "F",
            BasePrice = 100,
            Taxes = 20,
            Currency = "USD",
            SeatInventory = 500,
            IsRefoundable = true
        };

        var defaultAircraft = new Aircraft
        {
            Id = 1,
            Model = "Boeing 737",
            SeatCapacity = 300
        };
        var flight = new Flight { Id = createTicketDto.FlightId, DefaultAircraft = defaultAircraft };

        _unitOfWorkMock.Setup(u => u.Flights.GetById(createTicketDto.FlightId)).Returns(flight);

        // Act
        var result = await _ticketService.CreateTicket(createTicketDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Null(result.Value);
        Assert.Single(result.ErrorMessages!);
        Assert.Equal("SeatInventory must not exceed the seat capacity of the default aircraft.", result.ErrorMessages![0]);
        Assert.IsType<ValidationException>(result.Exceptions![0]);
    }

    [Fact]
    public async Task CreateTicket_ReturnsSuccess()
    {
        // Arrange
        var createTicketDto = new Dtos.CreateTicketDto
        {
            FlightId = 1,
            FareClass = "F",
            BasePrice = 100,
            Taxes = 20,
            Currency = "USD",
            SeatInventory = 200,
            IsRefoundable = true
        };

        var defaultAircraft = new Aircraft
        {
            Id = 1,
            Model = "Boeing 737",
            SeatCapacity = 300
        };
        var flight = new Flight { Id = createTicketDto.FlightId, DefaultAircraft = defaultAircraft };
        
        _unitOfWorkMock.Setup(u => u.Tickets).Returns(_ticketRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Flights.GetById(createTicketDto.FlightId)).Returns(flight);

        // Act
        var result = await _ticketService.CreateTicket(createTicketDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);  
        Assert.Equal(createTicketDto.FareClass, result.Value!.FareClass);
        Assert.Equal(createTicketDto.BasePrice, result.Value!.BasePrice);
        Assert.Equal(createTicketDto.Taxes, result.Value!.Taxes);
        Assert.Equal(createTicketDto.Currency, result.Value!.Currency);
        Assert.Equal(createTicketDto.SeatInventory, result.Value!.SeatInventory);
        Assert.Equal(createTicketDto.IsRefoundable, result.Value!.IsRefundable);
    }

    [Fact]
    public async Task DeleteTicketAsync_TicketHasBookings_ReturnsBadRequest()
    {
        // Arrange
        int ticketId = 1;
        _unitOfWorkMock.Setup(u => u.Bookings.BookingExistsForTicket(ticketId)).Returns(true);

        // Act
        var result = await _ticketService.DeleteTicketAsync(ticketId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Null(result.Value);
        Assert.Single(result.ErrorMessages!);
        Assert.Equal($"Cannot delete ticket ID {ticketId} as it has associated bookings.", result.ErrorMessages![0]);
        Assert.IsType<InvalidOperationException>(result.Exceptions![0]);
    }

    [Fact]
    public async Task DeleteTicketAsync_TicketDeletedSuccessfully_ReturnsSuccessMessage()
    {
        // Arrange
        int ticketId = 1;
        _unitOfWorkMock.Setup(u => u.Bookings.BookingExistsForTicket(ticketId)).Returns(false);
        _unitOfWorkMock.Setup(u => u.Tickets).Returns(_ticketRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Tickets.Delete(ticketId));

        // Act
        var result = await _ticketService.DeleteTicketAsync(ticketId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal($"Ticket ID {ticketId} deleted successfully.", result.Value);
        Assert.Null(result.ErrorMessages);
        Assert.Null(result.Exceptions);
    }

    [Fact]
    public async Task DeleteTicketAsync_TicketDeletionThrowsGenericNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        int ticketId = 1;
        _unitOfWorkMock.Setup(u => u.Bookings.BookingExistsForTicket(ticketId)).Returns(false);
        _unitOfWorkMock.Setup(u => u.Tickets).Returns(_ticketRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Tickets.Delete(ticketId))
            .Throws(new GenericNotFound<Ticket, int>(ticketId));

        // Act
        var result = await _ticketService.DeleteTicketAsync(ticketId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        Assert.Null(result.Value);
        Assert.Single(result.ErrorMessages!);
        Assert.Equal($"Ticket with ID {ticketId} not found.", result.ErrorMessages![0]);
        Assert.IsType<GenericNotFound<Ticket, int>>(result.Exceptions![0]);
    }

    [Fact]
    public void GetTicketByFlightId_NonExistingFlightId_ReturnsNotFoundResult()
    {
        // Arrange
        int flightId = 1;
        _unitOfWorkMock.Setup(u => u.Tickets.GetByFlightId(flightId)).Returns((Ticket?)null);

        // Act
        var result = _ticketService.GetTicketByFlightId(flightId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        Assert.Null(result.Value);
        Assert.Single(result.ErrorMessages!);
        Assert.Equal($"Ticket for flight ID {flightId} not found.", result.ErrorMessages![0]);
        Assert.IsType<KeyNotFoundException>(result.Exceptions![0]);
    }

    [Fact]
    public void GetTicketByFlightId_ExistingFlightId_ReturnsSuccessResult()
    {
        // Arrange
        int flightId = 1;
        var flight = new Flight
        {
            Id = flightId
        };
        var ticket = new Ticket
        {
            Id = 1,
            Flight = flight,
            FareClass = "F",
            BasePrice = 100,
            Taxes = 20,
            Currency = "USD",
            SeatInventory = 200,
            IsRefundable = true
        };
        _unitOfWorkMock.Setup(u => u.Tickets.GetByFlightId(flightId)).Returns(ticket);

        // Act
        var result = _ticketService.GetTicketByFlightId(flightId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(result.Value);
        Assert.Equal(ticket, result.Value);
        Assert.Null(result.ErrorMessages);
        Assert.Null(result.Exceptions);
    }

    [Fact]
    public async Task UpdateTicket_NonExistingTicket_ReturnsNotFoundResultAsync()
    {
        // Arrange
        int ticketId = 1;
        var updateTicketDto = new Dtos.UpdateTicketInventoryDto
        { TicketId = ticketId, NewSeatInventory = 150 };
        _unitOfWorkMock.Setup(u => u.Tickets.GetById(ticketId)).Returns((Ticket?)null);
        // Act
        var result = await _ticketService.UpdateTicketInventory(updateTicketDto);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        Assert.Null(result.Value);
        Assert.Single(result.ErrorMessages!);
        Assert.Equal($"Ticket with ID {ticketId} not found.", result.ErrorMessages![0]);
        Assert.IsType<GenericNotFound<Ticket, int>>(result.Exceptions![0]);
    }

    [Fact]
    public async Task UpdateTicket_ExistingTicket_ReturnsSuccessResultAsync()
    {
        // Arrange
        int ticketId = 1;
        var existingTicket = new Ticket
        {
            Id = ticketId,
            SeatInventory = 200
        };
        var updateTicketDto = new Dtos.UpdateTicketInventoryDto
        {
            TicketId = ticketId,
            NewSeatInventory = 150
        };
        _unitOfWorkMock.Setup(u => u.Tickets.GetById(ticketId)).Returns(existingTicket);
        _unitOfWorkMock.Setup(u => u.Tickets.Update(It.IsAny<Ticket>()));

        // Act
        var result = await _ticketService.UpdateTicketInventory(updateTicketDto);
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(updateTicketDto.NewSeatInventory, result.Value!.SeatInventory);
        Assert.Null(result.ErrorMessages);
        Assert.Null(result.Exceptions);
    }
}
