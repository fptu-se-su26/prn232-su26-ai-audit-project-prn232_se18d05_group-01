using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCourt.API.Authorization;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs;
using PlayCourt.Application.Interfaces;

namespace PlayCourt.API.Controllers
{
    [ApiController]
    [Route("api/venues/{venueId:int}/staff")]
    public sealed class VenueStaffsController : ControllerBase
    {
        private readonly IVenueStaffService _staffService;

        public VenueStaffsController(IVenueStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpPost]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> AddStaff(int venueId, [FromBody] AddVenueStaffRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<VenueStaffResponseDto>.Fail("Validation failed", GetModelStateErrors()));

            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized(ApiResponse<object>.Fail("Invalid token."));

            var response = await _staffService.AddStaffAsync(userId, venueId, request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet]
        [Authorize] // Can be CourtOwner or Staff member of the venue. Authorization is checked in the service.
        public async Task<IActionResult> GetStaff(int venueId)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized(ApiResponse<object>.Fail("Invalid token."));

            var response = await _staffService.GetVenueStaffAsync(userId, venueId);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("{staffId:int}")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> RemoveStaff(int venueId, int staffId)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized(ApiResponse<object>.Fail("Invalid token."));

            var response = await _staffService.RemoveStaffAsync(userId, venueId, staffId);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdStr, out userId);
        }

        private List<string> GetModelStateErrors()
        {
            return ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
        }
    }
}
