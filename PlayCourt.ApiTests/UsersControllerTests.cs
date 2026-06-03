using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlayCourt.API.Controllers;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Users;
using PlayCourt.Application.Interfaces;

namespace PlayCourt.ApiTests;

public sealed class UsersControllerTests
{
    [Fact]
    public async Task GetMe_WhenServiceSucceeds_ReturnsOk()
    {
        var controller = CreateControllerWithUser(new StubUserService(
            getResponse: ApiResponse<UserProfileResponseDto>.Ok(CreateProfile(), "User profile retrieved successfully.")));

        var result = await controller.GetMe();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
    }

    [Fact]
    public async Task GetMe_WhenServiceFails_ReturnsBadRequest()
    {
        var controller = CreateControllerWithUser(new StubUserService(
            getResponse: ApiResponse<UserProfileResponseDto>.Fail("User profile not found.")));

        var result = await controller.GetMe();

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task GetMe_WhenUserIdClaimMissing_ReturnsUnauthorized()
    {
        var controller = CreateControllerWithUser(new StubUserService(), userId: null);

        var result = await controller.GetMe();

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(401, unauthorized.StatusCode);
    }

    [Fact]
    public async Task UpdateMe_WhenServiceSucceeds_ReturnsOk()
    {
        var controller = CreateControllerWithUser(new StubUserService(
            updateResponse: ApiResponse<UserProfileResponseDto>.Ok(CreateProfile(), "User profile updated successfully.")));

        var result = await controller.UpdateMe(CreateUpdateRequest());

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
    }

    [Fact]
    public async Task UpdateMe_WhenServiceFails_ReturnsBadRequest()
    {
        var controller = CreateControllerWithUser(new StubUserService(
            updateResponse: ApiResponse<UserProfileResponseDto>.Fail("Update user profile failed.", ["FullName is required."])));

        var result = await controller.UpdateMe(CreateUpdateRequest());

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task UpdateMe_WhenUserIdClaimMissing_ReturnsUnauthorized()
    {
        var controller = CreateControllerWithUser(new StubUserService(), userId: null);

        var result = await controller.UpdateMe(CreateUpdateRequest());

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(401, unauthorized.StatusCode);
    }

    [Fact]
    public async Task UpdateMe_WhenInvalidUserIdClaim_ReturnsUnauthorized()
    {
        var controller = CreateControllerWithUser(new StubUserService(), userId: "not-an-id");

        var result = await controller.UpdateMe(CreateUpdateRequest());

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(401, unauthorized.StatusCode);
    }

    private static UsersController CreateControllerWithUser(IUserService userService, string? userId = "1")
    {
        var claims = new List<Claim>();

        if (userId is not null)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
        }

        return new UsersController(userService)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
                }
            }
        };
    }

    private static UserProfileResponseDto CreateProfile()
    {
        return new UserProfileResponseDto
        {
            UserId = 1,
            ProfileId = 1,
            Email = "player@example.com",
            Role = "Player",
            Status = "Active",
            FullName = "Nguyen Van A",
            IsEmailVerified = true
        };
    }

    private static UpdateUserProfileRequestDto CreateUpdateRequest()
    {
        return new UpdateUserProfileRequestDto
        {
            FullName = "Nguyen Phan Huy",
            AvatarUrl = "https://example.com/avatar.png",
            DateOfBirth = new DateTime(2004, 5, 29),
            Gender = 0,
            Address = "Da Nang",
            City = "Da Nang",
            Country = "Vietnam"
        };
    }

    private sealed class StubUserService(
        ApiResponse<UserProfileResponseDto>? getResponse = null,
        ApiResponse<UserProfileResponseDto>? updateResponse = null) : IUserService
    {
        public Task<ApiResponse<UserProfileResponseDto>> GetCurrentUserProfileAsync(int userId)
        {
            return Task.FromResult(getResponse
                ?? ApiResponse<UserProfileResponseDto>.Ok(CreateProfile()));
        }

        public Task<ApiResponse<UserProfileResponseDto>> UpdateCurrentUserProfileAsync(
            int userId,
            UpdateUserProfileRequestDto request)
        {
            return Task.FromResult(updateResponse
                ?? ApiResponse<UserProfileResponseDto>.Ok(CreateProfile()));
        }
    }
}
