using Microsoft.AspNetCore.Mvc;
using PlayCourt.API.Controllers;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Sports;
using PlayCourt.Application.Interfaces;

namespace PlayCourt.ApiTests;

public sealed class SportsControllerTests
{
    [Fact]
    public async Task GetAll_WhenServiceSucceeds_ReturnsOk()
    {
        var controller = new SportsController(new StubSportService(
            getAllResponse: ApiResponse<List<SportDto>>.Ok([CreateSportDto()])));

        var result = await controller.GetAll(isActive: true);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenServiceFails_ReturnsBadRequest()
    {
        var controller = new SportsController(new StubSportService(
            getByIdResponse: ApiResponse<SportDto>.Fail("Không tìm thấy môn thể thao.")));

        var result = await controller.GetById(999);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task Create_WhenModelStateInvalid_ReturnsBadRequest()
    {
        var controller = new SportsController(new StubSportService());
        controller.ModelState.AddModelError("Code", "Code is required.");

        var result = await controller.Create(new CreateSportRequestDto());

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task Update_WhenServiceSucceeds_ReturnsOk()
    {
        var controller = new SportsController(new StubSportService(
            updateResponse: ApiResponse<SportDto>.Ok(CreateSportDto())));

        var result = await controller.Update(1, new UpdateSportRequestDto
        {
            Code = "BADMINTON",
            Name = "Cầu lông",
            PlayerCount = 2,
            IsActive = true
        });

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
    }

    [Fact]
    public async Task ToggleActive_WhenServiceFails_ReturnsBadRequest()
    {
        var controller = new SportsController(new StubSportService(
            toggleResponse: ApiResponse<SportDto>.Fail("Không tìm thấy môn thể thao.")));

        var result = await controller.ToggleActive(999);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    private static SportDto CreateSportDto()
    {
        return new SportDto
        {
            Id = 1,
            Code = "BADMINTON",
            Name = "Cầu lông",
            PlayerCount = 2,
            IsActive = true,
            CreatedAt = DateTimeOffset.Now
        };
    }

    private sealed class StubSportService(
        ApiResponse<List<SportDto>>? getAllResponse = null,
        ApiResponse<SportDto>? getByIdResponse = null,
        ApiResponse<SportDto>? createResponse = null,
        ApiResponse<SportDto>? updateResponse = null,
        ApiResponse<SportDto>? toggleResponse = null) : ISportService
    {
        public Task<ApiResponse<List<SportDto>>> GetAllAsync(bool? isActive = null)
        {
            return Task.FromResult(getAllResponse
                ?? ApiResponse<List<SportDto>>.Ok([CreateSportDto()]));
        }

        public Task<ApiResponse<SportDto>> GetByIdAsync(int id)
        {
            return Task.FromResult(getByIdResponse
                ?? ApiResponse<SportDto>.Ok(CreateSportDto()));
        }

        public Task<ApiResponse<SportDto>> CreateAsync(CreateSportRequestDto request)
        {
            return Task.FromResult(createResponse
                ?? ApiResponse<SportDto>.Ok(CreateSportDto()));
        }

        public Task<ApiResponse<SportDto>> UpdateAsync(int id, UpdateSportRequestDto request)
        {
            return Task.FromResult(updateResponse
                ?? ApiResponse<SportDto>.Ok(CreateSportDto()));
        }

        public Task<ApiResponse<SportDto>> ToggleActiveAsync(int id)
        {
            return Task.FromResult(toggleResponse
                ?? ApiResponse<SportDto>.Ok(CreateSportDto()));
        }
    }
}
