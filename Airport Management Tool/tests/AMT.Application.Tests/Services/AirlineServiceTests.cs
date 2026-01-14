using System;
using AMT.Application.Dtos;
using AMT.Application.Services;
using AMT.Application.Services.Interfaces;
using AMT.Domain.Models;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces;
using AMT.Infrastructure.Interfaces.Repos;
using Moq;
using Serilog;

namespace AMT.Application.Tests.Services;

public class AirlineServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAirlineRepository> _airlineRepositoryMock;
    private readonly Mock<ILogger> _logggerMock;
    private readonly IAirlineService _airlineService;
    public AirlineServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _logggerMock = new Mock<ILogger>();
        _airlineRepositoryMock = new Mock<IAirlineRepository>();
        _airlineService = new AirlineService(_unitOfWorkMock.Object, _logggerMock.Object);
    }

    [Fact]
    public async Task CreateAirlineAsync_ReturnsSuccess()
    {
        //Arrange
        _unitOfWorkMock.Setup(uow => uow.Airlines).Returns(_airlineRepositoryMock.Object);
        var airlineCreateRequestDto = new AirlineCreateRequestDto()
        {
            Iatacode = "AA",
            Name = "American Airlines"
        };
        //Act
        var result = await _airlineService.CreateAirlineAsync(airlineCreateRequestDto);
        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(airlineCreateRequestDto.Name, result.Value!.Name);
        Assert.Equal(airlineCreateRequestDto.Iatacode, result.Value!.Iatacode);
        _airlineRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Domain.Models.Airline>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public void GetAirlineByIdAsync_ReturnsSuccess()
    {
        //Arrange
        var airlineId = 1;
        var airline = new Airline
        {
            Id = airlineId,
            Name = "American Airlines",
            Iatacode = "AA"
        };
        _airlineRepositoryMock.Setup(repo => repo.GetById(airlineId)).Returns(airline);
        _unitOfWorkMock.Setup(uow => uow.Airlines).Returns(_airlineRepositoryMock.Object);
        //Act
        var result = _airlineService.GetAirlineByIdAsync(airlineId);
        //Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(airlineId, result.Value!.Id);
        _airlineRepositoryMock.Verify(repo => repo.GetById(airlineId), Times.Once);
    }

    [Fact]
    public void GetAirlineByIdAsync_ReturnsFailure_WhenNotFound()
    {
        //Arrange
        var airlineId = 1;
        _airlineRepositoryMock.Setup(repo => repo.GetById(airlineId)).Returns((Airline?)null);
        _unitOfWorkMock.Setup(uow => uow.Airlines).Returns(_airlineRepositoryMock.Object);
        //Act
        var result = _airlineService.GetAirlineByIdAsync(airlineId);
        //Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.NotEmpty(result.ErrorMessages!);
        _airlineRepositoryMock.Verify(repo => repo.GetById(airlineId), Times.Once);
        Assert.IsType<GenericNotFound<Airline, int>>(result.Exceptions!.First());
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        Assert.Equal("Airline not found.", result.ErrorMessages!.First());
    }
}
