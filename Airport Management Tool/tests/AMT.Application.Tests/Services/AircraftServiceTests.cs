using System.Threading.Tasks;
using AMT.Application.Services;
using AMT.Application.Services.Interfaces;
using AMT.Domain.Models;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces;
using AMT.Infrastructure.Interfaces.Repos;
using Moq;
using Serilog;

namespace AMT.Application.Tests.Services;

public class AircraftServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<ILogger> _logger;
    private readonly IAircraftService _aircraftService;

    public AircraftServiceTests()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _logger = new Mock<ILogger>();
        _aircraftService = new AircraftService(_unitOfWork.Object, _logger.Object);
    }

    [Fact]
    public async Task CreateAircraftAsync_UnitOfWork_NoOwningAirling_ReturnsNotFound()
    {
        //Arrange
        int ownedByAirlineId = 1;
        string errorMessage= $"Airline with ID {ownedByAirlineId} not found.";
        _unitOfWork.Setup(uow => uow.Airlines.GetById(It.IsAny<int>())).Returns((Domain.Models.Airline?)null);
        _logger.Setup(logger => logger.Error(errorMessage));
        
        //Act
        var createResult = await _aircraftService.CreateAircraftAsync(new Dtos.AircraftCreateRequestDto
        {
            Model = "Boeing 737",
            TailNumber = "N12345",
            SeatCapacity = 180,
            OwnedByAirlineId = ownedByAirlineId
        });

        //Assert
        Assert.Null(createResult.Value);
        Assert.False(createResult.IsSuccess);
        Assert.Equal(errorMessage, createResult.ErrorMessages!.First());
        _logger.Verify(logger => logger.Error(errorMessage), Times.Once);
        Assert.IsType<GenericNotFound<Airline, int>>(createResult.Exceptions!.First());
        Assert.Equal(System.Net.HttpStatusCode.NotFound, createResult.StatusCode);
    }

    [Fact]
    public async Task CreateAircraftAsync_ReturnsSuccess()
    {
        //Arrange
        var airlineRepo = new Mock<IAirlineRepository>();
        var aircraftRepo = new Mock<IAircraftRepository>();

        _unitOfWork.Setup(uow => uow.Airlines).Returns(airlineRepo.Object);
        _unitOfWork.Setup(uow => uow.Aircraft).Returns(aircraftRepo.Object);

        int ownedByAirlineId = 1;
        string errorMessage= $"Airline with ID {ownedByAirlineId} not found.";
        var airline = new Airline { Id = ownedByAirlineId, Name = "Test Airline" };
        _unitOfWork.Setup(uow => uow.Airlines.GetById(It.IsAny<int>())).Returns(airline);
        _logger.Setup(logger => logger.Error(errorMessage));
        var addedAircraft = new Aircraft
        {
            Id = 0,
            Model = "Boeing 737",
            TailNumber = "N12345",
            SeatCapacity = 180,
            OwnedByAirline = airline
        };
        
        //Act
        var createResult = await _aircraftService.CreateAircraftAsync(new Dtos.AircraftCreateRequestDto
        {
            Model = "Boeing 737",
            TailNumber = "N12345",
            SeatCapacity = 180,
            OwnedByAirlineId = ownedByAirlineId
        });

        //Assert
        Assert.NotNull(createResult.Value);
        Assert.Equal(addedAircraft, createResult.Value);
        Assert.True(createResult.IsSuccess);
        Assert.Null(createResult.ErrorMessages);
        _logger.Verify(logger => logger.Error(It.IsAny<string>()), Times.Never);
        _logger.VerifyNoOtherCalls();
        Assert.Null(createResult.Exceptions);
    }

    [Fact]
    public void GetAircraftByIdAsync_AircraftNotFound_ReturnsNotFound()
    {
        //Arrange
        int aircraftId = 1;
        string errorMessage= $"Aircraft with ID {aircraftId} not found.";
        _unitOfWork.Setup(uow => uow.Aircraft.GetById(It.IsAny<int>())).Returns((Aircraft?)null);
        _logger.Setup(logger => logger.Error(errorMessage));
        
        //Act
        var getResult = _aircraftService.GetAircraftByIdAsync(aircraftId);

        //Assert
        Assert.Null(getResult.Value);
        Assert.False(getResult.IsSuccess);
        Assert.Equal(errorMessage, getResult.ErrorMessages!.First());
        _logger.Verify(logger => logger.Error(errorMessage), Times.Once);
        Assert.IsType<GenericNotFound<Aircraft, int>>(getResult.Exceptions!.First());
        Assert.Equal(System.Net.HttpStatusCode.NotFound, getResult.StatusCode);
    }

    [Fact]
    public void GetAircraftByIdAsync_ReturnsSuccess()
    {
        //Arrange
        int aircraftId = 1;
        var airline = new Airline { Id = 1, Name = "Test Airline" };
        var aircraft = new Aircraft
        {
            Id = aircraftId,
            Model = "Boeing 737",
            TailNumber = "N12345",
            SeatCapacity = 180,
            OwnedByAirline = airline
        };
        _unitOfWork.Setup(uow => uow.Aircraft.GetById(It.IsAny<int>())).Returns(aircraft);
        
        //Act
        var getResult = _aircraftService.GetAircraftByIdAsync(aircraftId);

        //Assert
        Assert.NotNull(getResult.Value);
        Assert.Equal(aircraft, getResult.Value);
        Assert.True(getResult.IsSuccess);
        Assert.Null(getResult.ErrorMessages);
        _logger.Verify(logger => logger.Error(It.IsAny<string>()), Times.Never);
        _logger.VerifyNoOtherCalls();
        Assert.Null(getResult.Exceptions);
    }

    [Fact]
    public void GetAllAircrafts_ReturnsSuccess()
    {
        //Arrange
        var airline = new Airline { Id = 1, Name = "Test Airline" };
        var aircrafts = new List<Aircraft>
        {
            new Aircraft
            {
                Id = 1,
                Model = "Boeing 737",
                TailNumber = "N12345",
                SeatCapacity = 180,
                OwnedByAirline = airline
            },
            new Aircraft
            {
                Id = 2,
                Model = "Airbus A320",
                TailNumber = "N54321",
                SeatCapacity = 150,
                OwnedByAirline = airline
            }
        };
        _unitOfWork.Setup(uow => uow.Aircraft.GetAll()).Returns(aircrafts);
        
        //Act
        var getAllResult = _aircraftService.GetAllAircrafts();

        //Assert
        Assert.NotNull(getAllResult.Value);
        Assert.Equal(aircrafts, getAllResult.Value);
        Assert.True(getAllResult.IsSuccess);
        Assert.Equal(2, getAllResult.Value.ToList().Count);
        Assert.Null(getAllResult.ErrorMessages);
        _logger.Verify(logger => logger.Error(It.IsAny<string>()), Times.Never);
        _logger.VerifyNoOtherCalls();
        Assert.Null(getAllResult.Exceptions);
    }
}
