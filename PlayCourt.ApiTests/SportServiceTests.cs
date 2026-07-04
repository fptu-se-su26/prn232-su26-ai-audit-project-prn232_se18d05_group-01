using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.DTOs.Sports;
using PlayCourt.Domain.Entities;
using PlayCourt.Infrastructure.Data;
using PlayCourt.Infrastructure.Services;

namespace PlayCourt.ApiTests;

public sealed class SportServiceTests
{
    [Fact]
    public async Task GetAllAsync_WhenIsActiveProvided_ReturnsOnlyMatchingSportsOrderedByName()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Sports.AddRange(
            CreateSport("TENNIS", "Tennis", true),
            CreateSport("BADMINTON", "Badminton", true),
            CreateSport("OLD", "Old Sport", false));
        await dbContext.SaveChangesAsync();
        var service = new SportService(dbContext);

        var response = await service.GetAllAsync(isActive: true);

        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(["Badminton", "Tennis"], response.Data.Select(sport => sport.Name));
    }

    [Fact]
    public async Task GetByIdAsync_WhenSportMissing_ReturnsFailure()
    {
        await using var dbContext = CreateDbContext();
        var service = new SportService(dbContext);

        var response = await service.GetByIdAsync(999);

        Assert.False(response.Success);
        Assert.Contains("không tìm thấy", response.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_NormalizesCodeAndDescription()
    {
        await using var dbContext = CreateDbContext();
        var service = new SportService(dbContext);

        var response = await service.CreateAsync(new CreateSportRequestDto
        {
            Code = " badminton ",
            Name = " Cầu lông ",
            Description = "  ",
            PlayerCount = 2
        });

        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal("BADMINTON", response.Data.Code);
        Assert.Equal("Cầu lông", response.Data.Name);
        Assert.Null(response.Data.Description);
        Assert.True(response.Data.IsActive);
    }

    [Fact]
    public async Task CreateAsync_WhenCodeOrNameExists_ReturnsFailure()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Sports.Add(CreateSport("BADMINTON", "Cầu lông"));
        await dbContext.SaveChangesAsync();
        var service = new SportService(dbContext);

        var response = await service.CreateAsync(new CreateSportRequestDto
        {
            Code = "badminton",
            Name = "Cầu lông",
            PlayerCount = 2
        });

        Assert.False(response.Success);
        Assert.Contains(response.Errors, error => error.Contains("mã", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(response.Errors, error => error.Contains("tên", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task CreateAsync_WhenPlayerCountIsNotPositive_ReturnsFailure()
    {
        await using var dbContext = CreateDbContext();
        var service = new SportService(dbContext);

        var response = await service.CreateAsync(new CreateSportRequestDto
        {
            Code = "INVALID",
            Name = "Invalid Sport",
            PlayerCount = 0
        });

        Assert.False(response.Success);
        Assert.Contains(response.Errors, error => error.Contains("lớn hơn 0", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task UpdateAsync_WhenValidRequest_UpdatesSport()
    {
        await using var dbContext = CreateDbContext();
        var sport = CreateSport("BADMINTON", "Cầu lông");
        dbContext.Sports.Add(sport);
        await dbContext.SaveChangesAsync();
        var service = new SportService(dbContext);

        var response = await service.UpdateAsync(sport.Id, new UpdateSportRequestDto
        {
            Code = " tennis ",
            Name = " Tennis ",
            Description = " Môn tennis ",
            PlayerCount = 2,
            IsActive = false
        });

        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal("TENNIS", response.Data.Code);
        Assert.Equal("Tennis", response.Data.Name);
        Assert.Equal("Môn tennis", response.Data.Description);
        Assert.False(response.Data.IsActive);
    }

    [Fact]
    public async Task UpdateAsync_WhenCodeBelongsToAnotherSport_ReturnsFailure()
    {
        await using var dbContext = CreateDbContext();
        var badminton = CreateSport("BADMINTON", "Cầu lông");
        var tennis = CreateSport("TENNIS", "Tennis");
        dbContext.Sports.AddRange(badminton, tennis);
        await dbContext.SaveChangesAsync();
        var service = new SportService(dbContext);

        var response = await service.UpdateAsync(tennis.Id, new UpdateSportRequestDto
        {
            Code = "badminton",
            Name = "Tennis",
            PlayerCount = 2,
            IsActive = true
        });

        Assert.False(response.Success);
        Assert.Contains(response.Errors, error => error.Contains("mã", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task ToggleActiveAsync_WhenSportExists_FlipsIsActive()
    {
        await using var dbContext = CreateDbContext();
        var sport = CreateSport("BADMINTON", "Cầu lông", isActive: true);
        dbContext.Sports.Add(sport);
        await dbContext.SaveChangesAsync();
        var service = new SportService(dbContext);

        var response = await service.ToggleActiveAsync(sport.Id);

        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.False(response.Data.IsActive);
    }

    private static PlayCourtDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<PlayCourtDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PlayCourtDbContext(options);
    }

    private static Sport CreateSport(string code, string name, bool isActive = true)
    {
        return new Sport
        {
            Code = code,
            Name = name,
            PlayerCount = 2,
            IsActive = isActive
        };
    }
}
