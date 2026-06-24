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

    [Fact]
    public async Task GetMySports_WhenServiceSucceeds_ReturnsOk()
    {
        var controller = CreateControllerWithUser(new StubUserService(
            getSportsResponse: ApiResponse<List<PlayerSportResponseDto>>.Ok([CreatePlayerSport()])));

        var result = await controller.GetMySports();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetMySports_WhenServiceFails_ReturnsBadRequest()
    {
        var controller = CreateControllerWithUser(new StubUserService(
            getSportsResponse: ApiResponse<List<PlayerSportResponseDto>>.Fail("User profile not found.")));

        var result = await controller.GetMySports();

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetMySports_WhenUserIdClaimMissing_ReturnsUnauthorized()
    {
        var controller = CreateControllerWithUser(new StubUserService(), userId: null);

        var result = await controller.GetMySports();

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task AddMySport_WhenServiceSucceeds_ReturnsOk()
    {
        var controller = CreateControllerWithUser(new StubUserService(
            addSportResponse: ApiResponse<PlayerSportResponseDto>.Ok(CreatePlayerSport())));

        var result = await controller.AddMySport(new AddPlayerSportRequestDto
        {
            SportId = 1,
            SkillLevel = 1
        });

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task AddMySport_WhenServiceFails_ReturnsBadRequest()
    {
        var controller = CreateControllerWithUser(new StubUserService(
            addSportResponse: ApiResponse<PlayerSportResponseDto>.Fail(
                "Sport already exists in user profile.")));

        var result = await controller.AddMySport(new AddPlayerSportRequestDto
        {
            SportId = 1,
            SkillLevel = 1
        });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task AddMySport_WhenModelStateIsInvalid_ReturnsBadRequest()
    {
        var controller = CreateControllerWithUser(new StubUserService());
        controller.ModelState.AddModelError("SportId", "Invalid");

        var result = await controller.AddMySport(new AddPlayerSportRequestDto());

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task AddMySport_WhenUserIdClaimMissing_ReturnsUnauthorized()
    {
        var controller = CreateControllerWithUser(new StubUserService(), userId: null);

        var result = await controller.AddMySport(new AddPlayerSportRequestDto());

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task UpdateMySport_WhenServiceSucceeds_ReturnsOk()
    {
        var controller = CreateControllerWithUser(new StubUserService(
            updateSportResponse: ApiResponse<PlayerSportResponseDto>.Ok(CreatePlayerSport())));

        var result = await controller.UpdateMySport(
            1,
            new UpdatePlayerSportRequestDto { SkillLevel = 2 });

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task UpdateMySport_WhenServiceFails_ReturnsBadRequest()
    {
        var controller = CreateControllerWithUser(new StubUserService(
            updateSportResponse: ApiResponse<PlayerSportResponseDto>.Fail(
                "Sport is not in user profile.")));

        var result = await controller.UpdateMySport(
            1,
            new UpdatePlayerSportRequestDto { SkillLevel = 2 });

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateMySport_WhenModelStateIsInvalid_ReturnsBadRequest()
    {
        var controller = CreateControllerWithUser(new StubUserService());
        controller.ModelState.AddModelError("SkillLevel", "Invalid");

        var result = await controller.UpdateMySport(1, new UpdatePlayerSportRequestDto());

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateMySport_WhenUserIdClaimMissing_ReturnsUnauthorized()
    {
        var controller = CreateControllerWithUser(new StubUserService(), userId: null);

        var result = await controller.UpdateMySport(1, new UpdatePlayerSportRequestDto());

        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task RemoveMySport_WhenServiceSucceeds_ReturnsOk()
    {
        var controller = CreateControllerWithUser(new StubUserService(
            removeSportResponse: ApiResponse<PlayerSportResponseDto>.Ok(CreatePlayerSport())));

        var result = await controller.RemoveMySport(1);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task RemoveMySport_WhenServiceFails_ReturnsBadRequest()
    {
        var controller = CreateControllerWithUser(new StubUserService(
            removeSportResponse: ApiResponse<PlayerSportResponseDto>.Fail(
                "Sport is not in user profile.")));

        var result = await controller.RemoveMySport(1);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task RemoveMySport_WhenUserIdClaimMissing_ReturnsUnauthorized()
    {
        var controller = CreateControllerWithUser(new StubUserService(), userId: null);

        var result = await controller.RemoveMySport(1);

        Assert.IsType<UnauthorizedObjectResult>(result);
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

    private static PlayerSportResponseDto CreatePlayerSport()
    {
        return new PlayerSportResponseDto
        {
            Id = 1,
            SportId = 1,
            SportCode = "BADMINTON",
            SportName = "Badminton",
            SkillLevel = "Intermediate"
        };
    }

    private sealed class StubUserService(
        ApiResponse<UserProfileResponseDto>? getResponse = null,
        ApiResponse<UserProfileResponseDto>? updateResponse = null,
        ApiResponse<List<PlayerSportResponseDto>>? getSportsResponse = null,
        ApiResponse<PlayerSportResponseDto>? addSportResponse = null,
        ApiResponse<PlayerSportResponseDto>? updateSportResponse = null,
        ApiResponse<PlayerSportResponseDto>? removeSportResponse = null) : IUserService
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

        public Task<ApiResponse<List<PlayerSportResponseDto>>> GetCurrentUserSportsAsync(int userId)
        {
            return Task.FromResult(getSportsResponse
                ?? ApiResponse<List<PlayerSportResponseDto>>.Ok([]));
        }

        public Task<ApiResponse<PlayerSportResponseDto>> AddCurrentUserSportAsync(
            int userId,
            AddPlayerSportRequestDto request)
        {
            return Task.FromResult(addSportResponse
                ?? ApiResponse<PlayerSportResponseDto>.Ok(CreatePlayerSport()));
        }

        public Task<ApiResponse<PlayerSportResponseDto>> UpdateCurrentUserSportAsync(
            int userId,
            int sportId,
            UpdatePlayerSportRequestDto request)
        {
            return Task.FromResult(updateSportResponse
                ?? ApiResponse<PlayerSportResponseDto>.Ok(CreatePlayerSport()));
        }

        public Task<ApiResponse<PlayerSportResponseDto>> RemoveCurrentUserSportAsync(
            int userId,
            int sportId)
        {
            return Task.FromResult(removeSportResponse
                ?? ApiResponse<PlayerSportResponseDto>.Ok(CreatePlayerSport()));
        }
    }
}
