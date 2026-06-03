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
            registerResponse: ApiResponse<RegisterResponseDto>.Fail("Register failed", ["Email already exists"])));

        var result = await controller.Register(CreateRequest());

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task Login_WhenServiceSucceeds_ReturnsOk()
    {
        var responseDto = new LoginResponseDto
        {
            AccessToken = "token",
            ExpiresAt = DateTime.UtcNow.AddMinutes(60),
            User = new LoginUserDto
            {
                Id = 1,
                FullName = "Nguyen Van A",
                Email = "player@example.com",
                PhoneNumber = "0909123456",
                Role = "Player",
                Status = "Active",
                IsEmailVerified = false
            }
        };
        var controller = new AuthController(new StubAuthService(
            loginResponse: ApiResponse<LoginResponseDto>.Ok(responseDto, "Login successfully")));

        var result = await controller.Login(CreateLoginRequest());

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
    }

    [Fact]
    public async Task Login_WhenServiceFails_ReturnsBadRequest()
    {
        var controller = new AuthController(new StubAuthService(
            loginResponse: ApiResponse<LoginResponseDto>.Fail("Login failed", ["Invalid email/phone or password"])));

        var result = await controller.Login(CreateLoginRequest());

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task Login_WhenModelStateInvalid_ReturnsBadRequest()
    {
        var controller = new AuthController(new StubAuthService(
            loginResponse: ApiResponse<LoginResponseDto>.Ok(new LoginResponseDto(), "Login successfully")));
        controller.ModelState.AddModelError("Identifier", "Identifier is required");

        var result = await controller.Login(CreateLoginRequest());

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task VerifyEmail_WhenServiceSucceeds_ReturnsOk()
    {
        var controller = new AuthController(new StubAuthService(
            verifyEmailResponse: ApiResponse<object>.Ok(null, "Email verified successfully.")));

        var result = await controller.VerifyEmail(new VerifyEmailRequestDto
        {
            Email = "player@example.com",
            Otp = "123456"
        });

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
    }

    [Fact]
    public async Task VerifyEmail_WhenServiceFails_ReturnsBadRequest()
    {
        var controller = new AuthController(new StubAuthService(
            verifyEmailResponse: ApiResponse<object>.Fail("Invalid verification code.")));

        var result = await controller.VerifyEmail(new VerifyEmailRequestDto
        {
            Email = "player@example.com",
            Otp = "000000"
        });

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    [Fact]
    public async Task ResendVerifyEmail_WhenServiceSucceeds_ReturnsOk()
    {
        var controller = new AuthController(new StubAuthService(
            resendVerifyEmailResponse: ApiResponse<object>.Ok(null, "Verification code has been sent to your email.")));

        var result = await controller.ResendVerifyEmail(new ResendVerifyEmailRequestDto
        {
            Email = "player@example.com"
        });

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
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

    private static LoginRequestDto CreateLoginRequest()
    {
        return new LoginRequestDto
        {
            Identifier = "player@example.com",
            Password = "123456"
        };
    }

    private sealed class StubAuthService(
        ApiResponse<RegisterResponseDto>? registerResponse = null,
        ApiResponse<LoginResponseDto>? loginResponse = null,
        ApiResponse<object>? verifyEmailResponse = null,
        ApiResponse<object>? resendVerifyEmailResponse = null) : IAuthService
    {
        public Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request)
        {
            return Task.FromResult(registerResponse
                ?? ApiResponse<RegisterResponseDto>.Ok(new RegisterResponseDto()));
        }

        public Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request)
        {
            return Task.FromResult(loginResponse
                ?? ApiResponse<LoginResponseDto>.Ok(new LoginResponseDto()));
        }

        public Task<ApiResponse<object>> VerifyEmailAsync(VerifyEmailRequestDto request)
        {
            return Task.FromResult(verifyEmailResponse
                ?? ApiResponse<object>.Ok(null));
        }

        public Task<ApiResponse<object>> ResendVerifyEmailAsync(ResendVerifyEmailRequestDto request)
        {
            return Task.FromResult(resendVerifyEmailResponse
                ?? ApiResponse<object>.Ok(null));
        }
    }
}
