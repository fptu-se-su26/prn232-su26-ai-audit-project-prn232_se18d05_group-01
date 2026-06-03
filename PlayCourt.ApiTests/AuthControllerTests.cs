using Microsoft.AspNetCore.Mvc;
using PlayCourt.API.Controllers;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Auth;
using PlayCourt.Application.Interfaces;

namespace PlayCourt.ApiTests;

public sealed class AuthControllerTests
{
    [Fact]
    public async Task Register_WhenServiceSucceeds_ReturnsCreated()
    {
        var responseDto = new RegisterResponseDto
        {
            Id = 1,
            FullName = "Nguyen Van A",
            Email = "player@example.com",
            PhoneNumber = "0909123456",
            Role = "Player",
            Status = "Active",
            IsEmailVerified = false
        };
        var controller = new AuthController(new StubAuthService(
            ApiResponse<RegisterResponseDto>.Ok(responseDto, "Register successfully")));

        var result = await controller.Register(CreateRequest());

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(AuthController.Register), created.ActionName);
        Assert.Equal(201, created.StatusCode);
    }

    [Fact]
    public async Task Register_WhenServiceFails_ReturnsBadRequest()
    {
        var controller = new AuthController(new StubAuthService(
            ApiResponse<RegisterResponseDto>.Fail("Register failed", ["Email already exists"])));

        var result = await controller.Register(CreateRequest());

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    private static RegisterRequestDto CreateRequest()
    {
        return new RegisterRequestDto
        {
            FullName = "Nguyen Van A",
            Email = "player@example.com",
            PhoneNumber = "0909123456",
            Password = "123456",
            Role = "Player"
        };
    }

    private sealed class StubAuthService(ApiResponse<RegisterResponseDto> response) : IAuthService
    {
        public Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request)
        {
            return Task.FromResult(response);
        }
    }
}
