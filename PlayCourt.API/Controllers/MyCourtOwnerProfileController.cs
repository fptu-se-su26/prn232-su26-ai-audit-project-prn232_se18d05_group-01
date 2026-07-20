using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCourt.API.Authorization;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.CourtOwners;
using PlayCourt.Application.Interfaces;

namespace PlayCourt.API.Controllers
{
    [ApiController]
    [Authorize(Policy = ApiPolicies.CourtOwner)]
    [Route("api/court-owners/me")]
    public sealed class MyCourtOwnerProfileController : ControllerBase
    {
        private readonly ICourtOwnerService _courtOwnerService;

        public MyCourtOwnerProfileController(ICourtOwnerService courtOwnerService)
        {
            _courtOwnerService = courtOwnerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyProfile()
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid authentication token."));
            }

            var response = await _courtOwnerService.GetMyProfileAsync(userId);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateMyProfile(
            [FromBody] UpdateCourtOwnerProfileRequestDto request)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid authentication token."));
            }

            var response = await _courtOwnerService.UpdateMyProfileAsync(userId, request);

            if (!response.Success)
            {
                return response.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                    ? NotFound(response)
                    : Conflict(response);
            }

            return Ok(response);
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitMyProfile()
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid authentication token."));
            }

            var response = await _courtOwnerService.SubmitMyProfileAsync(userId);

            if (!response.Success)
            {
                if (response.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(response);
                }

                return Conflict(response);
            }

            return Ok(response);
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            userId = 0;
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out userId);
        }
    }
}
