using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCourt.API.Authorization;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Venues;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Enums;

namespace PlayCourt.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class VenuesController : ControllerBase
    {
        private readonly IVenueService _venueService;

        public VenuesController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] VenueSearchRequestDto request)
        {
            var response = await _venueService.GetAllVenuesAsync(request);
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _venueService.GetPublicVenueByIdAsync(id);
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("{id:int}/availability")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailability(int id, [FromQuery] DateOnly date)
        {
            if (date == default)
                return BadRequest(ApiResponse<VenueAvailabilityResponseDto>.Fail("Date must use YYYY-MM-DD format."));

            var response = await _venueService.GetAvailabilityAsync(id, date);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("admin")]
        [Authorize(Policy = ApiPolicies.Admin)]
        public async Task<IActionResult> GetAllForAdmin([FromQuery] VenueStatus? status = null)
        {
            var response = await _venueService.GetAllVenuesForAdminAsync(status);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("admin/{id:int}")]
        [Authorize(Policy = ApiPolicies.Admin)]
        public async Task<IActionResult> GetByIdForAdmin(int id)
        {
            var response = await _venueService.GetVenueForAdminByIdAsync(id);
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("admin/change-requests")]
        [Authorize(Policy = ApiPolicies.Admin)]
        public async Task<IActionResult> GetChangeRequestsForAdmin([FromQuery] VenueChangeRequestStatus? status = null)
        {
            var response = await _venueService.GetVenueChangeRequestsForAdminAsync(status);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpPatch("admin/change-requests/{changeRequestId:int}/status")]
        [Authorize(Policy = ApiPolicies.Admin)]
        public async Task<IActionResult> UpdateChangeRequestStatus(
            int changeRequestId,
            [FromBody] UpdateVenueChangeRequestStatusRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<VenueChangeRequestResponseDto>.Fail("Validation failed", GetModelStateErrors()));
            var response = await _venueService.UpdateVenueChangeRequestStatusAsync(changeRequestId, request);
            if (!response.Success) return HandleVenueChangeRequestStatusError(response);
            return Ok(response);
        }

        [HttpPatch("{id:int}/status")]
        [Authorize(Policy = ApiPolicies.Admin)]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateVenueStatusRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<VenueResponseDto>.Fail("Validation failed", GetModelStateErrors()));
            var response = await _venueService.UpdateVenueStatusAsync(id, request);
            if (!response.Success) return HandleVenueStatusError(response);
            return Ok(response);
        }

        [HttpPost]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> Create([FromBody] CreateVenueRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<VenueResponseDto>.Fail("Validation failed", GetModelStateErrors()));
            if (!TryGetCurrentUserId(out var userId)) return Unauthorized(ApiResponse<object>.Fail("Invalid token."));
            var response = await _venueService.CreateVenueAsync(userId, request);
            if (!response.Success) return BadRequest(response);
            // Trỏ tới Owner endpoint vì venue mới tạo có Status=Pending, public endpoint GetById yêu cầu Approved
            return CreatedAtAction(nameof(GetMyById), new { id = response.Data!.Id }, response);
        }

        [HttpGet("my")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> GetMy()
        {
            if (!TryGetCurrentUserId(out var userId)) return Unauthorized(ApiResponse<object>.Fail("Invalid token."));
            var response = await _venueService.GetMyVenuesAsync(userId);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateVenueRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<VenueResponseDto>.Fail("Validation failed", GetModelStateErrors()));
            if (!TryGetCurrentUserId(out var userId)) return Unauthorized(ApiResponse<object>.Fail("Invalid token."));
            var response = await _venueService.UpdateVenueAsync(userId, id, request);
            if (!response.Success) return response.Message.Contains("not found", StringComparison.OrdinalIgnoreCase) ? NotFound(response) : BadRequest(response);
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> Delete(int id)
        {
            if (!TryGetCurrentUserId(out var userId)) return Unauthorized(ApiResponse<object>.Fail("Invalid token."));
            var response = await _venueService.DeleteVenueAsync(userId, id);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        // --- Images ---

        [HttpPost("{id:int}/images")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> AddImage(int id, [FromBody] AddVenueImageRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ApiResponse<VenueImageDto>.Fail("Validation failed", GetModelStateErrors()));
            if (!TryGetCurrentUserId(out var userId)) return Unauthorized(ApiResponse<object>.Fail("Invalid token."));
            var response = await _venueService.AddImageAsync(userId, id, request);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpDelete("{id:int}/images/{imageId:int}")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> DeleteImage(int id, int imageId)
        {
            if (!TryGetCurrentUserId(out var userId)) return Unauthorized(ApiResponse<object>.Fail("Invalid token."));
            var response = await _venueService.DeleteImageAsync(userId, id, imageId);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpPatch("{id:int}/images/{imageId:int}/set-cover")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> SetCoverImage(int id, int imageId)
        {
            if (!TryGetCurrentUserId(out var userId)) return Unauthorized(ApiResponse<object>.Fail("Invalid token."));
            var response = await _venueService.SetCoverImageAsync(userId, id, imageId);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        // --- Amenities ---

        [HttpPost("{id:int}/amenities/{amenityId:int}")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> AddAmenity(int id, int amenityId)
        {
            if (!TryGetCurrentUserId(out var userId)) return Unauthorized(ApiResponse<object>.Fail("Invalid token."));
            var response = await _venueService.AddVenueAmenityAsync(userId, id, amenityId);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpDelete("{id:int}/amenities/{amenityId:int}")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> RemoveAmenity(int id, int amenityId)
        {
            if (!TryGetCurrentUserId(out var userId)) return Unauthorized(ApiResponse<object>.Fail("Invalid token."));
            var response = await _venueService.RemoveVenueAmenityAsync(userId, id, amenityId);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        // --- Opening Hours ---

        [HttpGet("{id:int}/opening-hours")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOpeningHours(int id)
        {
            var response = await _venueService.GetOpeningHoursAsync(id);
            return Ok(response);
        }

        [HttpPut("{id:int}/opening-hours")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> UpdateOpeningHours(int id, [FromBody] UpdateOpeningHoursRequestDto request)
        {
            if (!TryGetCurrentUserId(out var userId)) return Unauthorized(ApiResponse<object>.Fail("Invalid token."));
            var response = await _venueService.UpdateOpeningHoursAsync(userId, id, request);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        // --- Owner Detail ---

        [HttpGet("my/{id:int}")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> GetMyById(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized(ApiResponse<object>.Fail("Invalid token."));
            var response = await _venueService.GetMyVenueByIdAsync(userId, id);
            if (!response.Success)
                return NotFound(response);
            return Ok(response);
        }

        // --- Favorites ---

        [HttpPost("{id:int}/favorites")]
        [Authorize]
        public async Task<IActionResult> AddFavorite(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized(ApiResponse<object>.Fail("Invalid token."));
            var response = await _venueService.AddFavoriteAsync(userId, id);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpDelete("{id:int}/favorites")]
        [Authorize]
        public async Task<IActionResult> RemoveFavorite(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized(ApiResponse<object>.Fail("Invalid token."));
            var response = await _venueService.RemoveFavoriteAsync(userId, id);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("favorites/my")]
        [Authorize]
        public async Task<IActionResult> GetMyFavorites()
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized(ApiResponse<object>.Fail("Invalid token."));
            var response = await _venueService.GetMyFavoritesAsync(userId);
            return Ok(response);
        }

        // --- Stats ---

        [HttpGet("stats")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> GetOwnerStats()
        {
            if (!TryGetCurrentUserId(out var userId)) return Unauthorized(ApiResponse<object>.Fail("Invalid token."));
            var response = await _venueService.GetOwnerStatsAsync(userId);
            if (!response.Success) return BadRequest(response);
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

        private IActionResult HandleVenueStatusError(ApiResponse<VenueResponseDto> response)
        {
            if (response.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(response);

            if (response.Message.Contains("Cannot transition", StringComparison.OrdinalIgnoreCase))
                return Conflict(response);

            return BadRequest(response);
        }

        private IActionResult HandleVenueChangeRequestStatusError(ApiResponse<VenueChangeRequestResponseDto> response)
        {
            if (response.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(response);

            if (response.Message.Contains("already been processed", StringComparison.OrdinalIgnoreCase))
                return Conflict(response);

            return BadRequest(response);
        }
    }
}
