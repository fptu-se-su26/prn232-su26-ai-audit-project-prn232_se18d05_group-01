using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Users;
using PlayCourt.Application.Interfaces;

namespace PlayCourt.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public sealed class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid authentication token."));
            }

            var response = await _userService.GetCurrentUserProfileAsync(userId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateUserProfileRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(value => value.Errors)
                    .Select(error => error.ErrorMessage);

                return BadRequest(ApiResponse<UserProfileResponseDto>.Fail("Validation failed", errors));
            }

            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid authentication token."));
            }

            var response = await _userService.UpdateCurrentUserProfileAsync(userId, request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("me/sports")]
        public async Task<IActionResult> GetMySports()
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid authentication token."));
            }

            var response = await _userService.GetCurrentUserSportsAsync(userId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("me/sports")]
        public async Task<IActionResult> AddMySport([FromBody] AddPlayerSportRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(value => value.Errors)
                    .Select(error => error.ErrorMessage);

                return BadRequest(ApiResponse<PlayerSportResponseDto>.Fail(
                    "Validation failed",
                    errors));
            }

            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid authentication token."));
            }

            var response = await _userService.AddCurrentUserSportAsync(userId, request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("me/sports/{sportId:int}")]
        public async Task<IActionResult> UpdateMySport(
            int sportId,
            [FromBody] UpdatePlayerSportRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(value => value.Errors)
                    .Select(error => error.ErrorMessage);

                return BadRequest(ApiResponse<PlayerSportResponseDto>.Fail(
                    "Validation failed",
                    errors));
            }

            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid authentication token."));
            }

            var response = await _userService.UpdateCurrentUserSportAsync(
                userId,
                sportId,
                request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("me/sports/{sportId:int}")]
        public async Task<IActionResult> RemoveMySport(int sportId)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid authentication token."));
            }

            var response = await _userService.RemoveCurrentUserSportAsync(userId, sportId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdClaim = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out userId);
        }
    }
}
