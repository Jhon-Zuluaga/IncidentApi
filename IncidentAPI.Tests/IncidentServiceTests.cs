using Moq;
using IncidentAPI.Api.DTOs.Incident;
using IncidentAPI.Api.Models;
using IncidentAPI.Api.Repositories.Interfaces;
using IncidentAPI.Api.Services.Implementations;
using Microsoft.Extensions.Logging;

namespace IncidentAPI.Tests;

public class IncidentServiceTests
{
   private readonly Mock<IIncidentRepository> _mockRepo;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<ICategoryRepository> _mockCategoryRepo;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly IncidentService _service;
    private readonly Mock<ILogger<IncidentService>> _mockLogger;

    public IncidentServiceTests()
    {
        _mockRepo         = new Mock<IIncidentRepository>();
        _mockUserRepo     = new Mock<IUserRepository>();
        _mockCategoryRepo = new Mock<ICategoryRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _mockLogger = new Mock<ILogger<IncidentService>>();

        _service = new IncidentService(
            _mockRepo.Object,
            _mockUserRepo.Object,
            _mockCategoryRepo.Object,
            _mockEmailService.Object,
            _mockLogger.Object
        );
    }

    [Fact]
public async Task CreateAsync_VaidadData_ReturnsIncident()
{
    var dto = new CreateIncidentDto
    {
        Title = "Error en el servidor",
        Status = "abierto",
        UserId = 1,
        CategoryId = 1
    };

    var createdIncident = new Incident
    {
        Id = 1,
        Title = dto.Title,
        Status = dto.Status,
        UserId = dto.UserId,
        CategoryId = dto.CategoryId,
        User = new User { Id = 1, Name = "Admin", Email = "admin@est.com" },
        Category = new Category { Id = 1, Name = "Hardware" }
    };


    _mockUserRepo.Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(new User { Id = 1, Name = "Admin", Email = "admin@est.com" });

    _mockCategoryRepo.Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(new Category { Id = 1, Name = "Hardware" });
   

    _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Incident>()))
        .ReturnsAsync(createdIncident);

    _mockRepo.Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(createdIncident);

    var result = await _service.CreateAsync(dto);

    Assert.NotNull(result);
    Assert.Equal("Error en el servidor", result.Title);
    Assert.Equal("abierto", result.Status);
    Assert.Equal(1, result.UserId);
    }

    [Fact]
    public async Task UpdateAsync_IncidentNotFound_ReturnsNull()
    {
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<int>(), It.IsAny<Incident>()))
            .ReturnsAsync((Incident?)null);
        
        var dto = new UpdateIncidentDto
        {
            Title = "Nuevo titulo",
            Status = "Cerrado"
        };

        var result = await _service.UpdateAsync(999, dto);

        Assert.Null(result);

    }

    [Fact]
    public async Task UpdateAsync_InvalidStatus_ReturnsNull()
    {
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<int>(), It.IsAny<Incident>()))
            .ReturnsAsync((Incident?)null);
        
        var dto = new UpdateIncidentDto
        {
            Status = "estado_invalido"
        };

        var result = await _service.UpdateAsync(1, dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsIncidentDto()
    {
        var incident = new Incident
        {
            Id = 1, 
            Title = "Error de red",
            Status = "Abierto",
            UserId = 1,
            CategoryId = 1,
            User = new User { Id = 1, Name = "Admin", Email = "admin@test.com"},
            Category = new Category { Id = 1, Name = "Red"}
        };

        _mockRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(incident);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Error de red", result.Title);

    }

    [Fact]
    public async Task GetByIdAsync_NotExistingId_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Incident?)null);

        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }


}

