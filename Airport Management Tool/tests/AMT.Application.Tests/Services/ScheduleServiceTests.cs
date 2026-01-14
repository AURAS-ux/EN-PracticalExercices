using System;
using AMT.Application.Dtos;
using AMT.Application.Validators;
using AMT.Domain.Models;
using AMT.Infrastructure.Exceptions;
using AMT.Infrastructure.Interfaces;
using AMT.Infrastructure.Interfaces.Repos;
using FluentValidation;
using Moq;

namespace AMT.Application.Tests.Services;

public class ScheduleServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<Serilog.ILogger> _loggerMock;
    private readonly Mock<IFlightScheduleRepository> _flightScheduleRepositoryMock;
    private readonly IValidator<FlightSchedule> _flightScheduleValidator;
    private readonly Application.Services.ScheduleService _scheduleService;

    public ScheduleServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<Serilog.ILogger>();
        _flightScheduleRepositoryMock = new Mock<IFlightScheduleRepository>();
        _flightScheduleValidator = new ScheduleValidator();
        _scheduleService = new Application.Services.ScheduleService(_unitOfWorkMock.Object, _loggerMock.Object, _flightScheduleValidator);
    }

    [Fact]
    public async Task BulkImportSchedulesAsync_InvalidJson_ReturnsFailureResult()
    {
        // Arrange
        var invalidJson = "Invalid JSON Content";
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        await writer.WriteAsync(invalidJson);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act
        var result = await _scheduleService.BulkCreateSchedulesAsync(stream);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.ImportResults);
        foreach (var importResult in result.ImportResults.Values)
        {
            Assert.Equal(BulkImportResultDto.ImportStatus.FAILED, importResult.Status);
            Assert.False(importResult.Result!.IsSuccess);
            Assert.Contains("Invalid JSON format.", importResult.Result!.ErrorMessages![0]);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, importResult.Result!.StatusCode);
        }
    }

    [Fact]
    public async Task BulkImportSchedulesAsync_NullInputStream_ReturnsFailureResult()
    {
        // Arrange
        Stream? nullStream = null;

        // Act
        var result = await _scheduleService.BulkCreateSchedulesAsync(nullStream!);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.ImportResults);
        foreach (var importResult in result.ImportResults.Values)
        {
            Assert.Equal(BulkImportResultDto.ImportStatus.FAILED, importResult.Status);
            Assert.False(importResult.Result!.IsSuccess);
            Assert.Contains("Input data is null.", importResult.Result!.ErrorMessages![0]);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, importResult.Result!.StatusCode);
        }
    }

    [Fact]
    public async Task BulkImportSchedulesAsync_ArgumentException_ReturnsFailureResult()
    {
        // Arrange
        var streamMock = new Mock<Stream>();
        streamMock.Setup(s => s.ReadAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Test argument exception"));

        // Act
        var result = await _scheduleService.BulkCreateSchedulesAsync(streamMock.Object);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.ImportResults);
        foreach (var importResult in result.ImportResults.Values)
        {
            Assert.Equal(BulkImportResultDto.ImportStatus.FAILED, importResult.Status);
            Assert.False(importResult.Result!.IsSuccess);
            Assert.Contains("Error reading input data.", importResult.Result!.ErrorMessages![0]);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, importResult.Result!.StatusCode);
        }
    }

    [Fact]
    public async Task BulkImportSchedulesAsync_NoSchedulesInJson_ReturnsNoContent()
    {
        // Arrange
        var emptySchedulesJson = "[]";
        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream);
        await writer.WriteAsync(emptySchedulesJson);
        await writer.FlushAsync();
        stream.Position = 0;

        // Act
        var result = await _scheduleService.BulkCreateSchedulesAsync(stream);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.ImportResults);
        foreach (var importResult in result.ImportResults.Values)
        {
            Assert.Equal(BulkImportResultDto.ImportStatus.FAILED, importResult.Status);
            Assert.False(importResult.Result!.IsSuccess);
            Assert.Contains("No schedules found in the provided data.", importResult.Result!.ErrorMessages![0]);
            Assert.Equal(System.Net.HttpStatusCode.NoContent, importResult.Result!.StatusCode);
        }
    }

    [Fact]
    public async Task CreateSchedule_NoFlightFound_ReturnsNotFound()
    {
        // Arrange
        int scheduleId = 1;
        var createScheduleDto = new CreateScheduleDto
        {
            FlightId = scheduleId,
            ScheduleDepartureUtc = DateTime.UtcNow.AddHours(2).ToString(),
            ScheduleArrivalUtc = DateTime.UtcNow.AddHours(5).ToString(),
            GateCode = "00",
            AssignedAircraftTail = "N12345",
            Status = 0
        };
        _unitOfWorkMock.Setup(uow => uow.Flights.GetById(scheduleId))
            .Returns((Flight?)null);

        // Act
        var result = await _scheduleService.CreateScheduleAsync(createScheduleDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal($"Flight not found.", result.ErrorMessages![0]);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        Assert.IsType<GenericNotFound<Flight, int>>(result.Exceptions![0]);
    }
}
