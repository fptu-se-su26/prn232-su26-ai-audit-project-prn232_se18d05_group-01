using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCourt.API.Authorization;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Enums;

namespace PlayCourt.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [Authorize(Policy = ApiPolicies.Player)]
        public async Task<IActionResult> Create([FromBody] CreateReviewRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<ReviewResponseDto>.Fail("Validation failed", GetModelStateErrors()));

            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized(ApiResponse<object>.Fail("Invalid token."));

            var response = await _reviewService.CreateReviewAsync(userId, request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("~/api/venues/{venueId:int}/reviews")]
        [AllowAnonymous]
        public async Task<IActionResult> GetVenueReviews(int venueId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var response = await _reviewService.GetVenueReviewsAsync(venueId, page, pageSize);
            return Ok(response);
        }

        [HttpGet("~/api/courts/{courtId:int}/reviews")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourtReviews(int courtId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var response = await _reviewService.GetCourtReviewsAsync(courtId, page, pageSize);
            return Ok(response);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = ApiPolicies.Player)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReviewRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<ReviewResponseDto>.Fail("Validation failed", GetModelStateErrors()));

            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized(ApiResponse<object>.Fail("Invalid token."));

            var response = await _reviewService.UpdateReviewAsync(userId, id, request);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized(ApiResponse<object>.Fail("Invalid token."));

            var response = await _reviewService.DeleteReviewAsync(userId, id);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("{id:int}/report")]
        [Authorize(Policy = ApiPolicies.Player)]
        public async Task<IActionResult> Report(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized(ApiResponse<object>.Fail("Invalid token."));

            var response = await _reviewService.ReportReviewAsync(userId, id);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut("~/api/admin/reviews/{id:int}/moderate")]
        [Authorize(Policy = ApiPolicies.Admin)]
        public async Task<IActionResult> Moderate(int id, [FromQuery] ReviewStatus status)
        {
            var response = await _reviewService.ModerateReviewAsync(id, status);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("{id:int}/images")]
        [Authorize(Policy = ApiPolicies.Player)]
        public async Task<IActionResult> AddImage(int id, [FromQuery] string imageUrl, [FromQuery] short displayOrder = 0)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return BadRequest(ApiResponse<object>.Fail("imageUrl không được để trống."));

            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized(ApiResponse<object>.Fail("Invalid token."));

            var response = await _reviewService.AddReviewImageAsync(userId, id, imageUrl.Trim(), displayOrder);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("{id:int}/images/{imageId:int}")]
        [Authorize(Policy = ApiPolicies.Player)]
        public async Task<IActionResult> DeleteImage(int id, int imageId)
        {
            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized(ApiResponse<object>.Fail("Invalid token."));

            var response = await _reviewService.DeleteReviewImageAsync(userId, id, imageId);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("~/api/venues/{venueId:int}/rating-stats")]
        [AllowAnonymous]
        public async Task<IActionResult> GetVenueStats(int venueId)
        {
            var response = await _reviewService.GetVenueStatsAsync(venueId);
            return Ok(response);
        }

        [HttpGet("~/api/courts/{courtId:int}/rating-stats")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourtStats(int courtId)
        {
            var response = await _reviewService.GetCourtStatsAsync(courtId);
            return Ok(response);
        }

        [HttpGet("my")]
        [Authorize(Policy = ApiPolicies.Player)]
        public async Task<IActionResult> GetMyReviews([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            if (!TryGetCurrentUserId(out var userId))
                return Unauthorized(ApiResponse<object>.Fail("Invalid token."));

            var response = await _reviewService.GetMyReviewsAsync(userId, page, pageSize);
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
