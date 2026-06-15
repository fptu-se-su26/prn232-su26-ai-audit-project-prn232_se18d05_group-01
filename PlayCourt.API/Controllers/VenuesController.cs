using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCourt.API.Authorization;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Venues;
using PlayCourt.Application.Interfaces;

namespace PlayCourt.API.Controllers
{
    [ApiController]
    [Authorize(Policy = ApiPolicies.CourtOwner)]
    [Route("api/[controller]")]
    public sealed class VenuesController : ControllerBase
    {
        private readonly IVenueService _venueService;

        public VenuesController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVenueRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<VenueResponseDto>.Fail("Validation failed", GetModelStateErrors()));
            }

            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid authentication token."));
            }

            var response = await _venueService.CreateVenueAsync(userId, request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return CreatedAtAction(nameof(GetById), new { id = response.Data!.Id }, response);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMy()
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid authentication token."));
            }

            var response = await _venueService.GetMyVenuesAsync(userId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid authentication token."));
            }

            var response = await _venueService.GetVenueByIdAsync(userId, id);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateVenueRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<VenueResponseDto>.Fail("Validation failed", GetModelStateErrors()));
            }

            if (!TryGetCurrentUserId(out var userId))
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid authentication token."));
            }

            var response = await _venueService.UpdateVenueAsync(userId, id, request);

            if (!response.Success)
            {
                return response.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                    ? NotFound(response)
                    : BadRequest(response);
            }

            return Ok(response);
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdClaim = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out userId);
        }

        private IEnumerable<string> GetModelStateErrors()
        {
            return ModelState.Values
                .SelectMany(value => value.Errors)
                .Select(error => error.ErrorMessage);
        }
    }
}
