using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCourt.API.Authorization;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Courts;
using PlayCourt.Application.Interfaces;
using System.Security.Claims;

namespace PlayCourt.API.Controllers
{
    [ApiController]
    public sealed class CourtsController : ControllerBase
    {
        private readonly ICourtService _courtService;

        public CourtsController(ICourtService courtService)
        {
            _courtService = courtService;
        }

        // GET /api/venues/{venueId}/courts
        // Public — không cần đăng nhập.
        [HttpGet("api/venues/{venueId:int}/courts")]
        public async Task<IActionResult> GetByVenue(int venueId)
        {
            var response = await _courtService.GetByVenueAsync(venueId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // GET /api/courts/{id}
        // Public — không cần đăng nhập.
        [HttpGet("api/courts/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _courtService.GetByIdAsync(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // POST /api/venues/{venueId}/courts
        // Chỉ CourtOwner mới được tạo court. Verify ownership trong service.
        [HttpPost("api/venues/{venueId:int}/courts")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> Create(int venueId, [FromBody] CreateCourtRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return BadRequest(ApiResponse<CourtDto>.Fail("Validation failed", errors));
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId is null)
            {
                return Unauthorized(ApiResponse<CourtDto>.Fail("Không xác định được danh tính người dùng."));
            }

            var response = await _courtService.CreateAsync(venueId, currentUserId.Value, request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // PUT /api/courts/{id}
        // Chỉ CourtOwner sở hữu court mới được cập nhật. Verify ownership trong service.
        [HttpPut("api/courts/{id:int}")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCourtRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return BadRequest(ApiResponse<CourtDto>.Fail("Validation failed", errors));
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId is null)
            {
                return Unauthorized(ApiResponse<CourtDto>.Fail("Không xác định được danh tính người dùng."));
            }

            var response = await _courtService.UpdateAsync(id, currentUserId.Value, request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // DELETE /api/courts/{id}
        // Chỉ CourtOwner sở hữu court mới được xóa mềm. Verify ownership trong service.
        [HttpDelete("api/courts/{id:int}")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId is null)
            {
                return Unauthorized(ApiResponse<bool>.Fail("Không xác định được danh tính người dùng."));
            }

            var response = await _courtService.DeleteAsync(id, currentUserId.Value);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // ─── PRIVATE HELPERS ─────────────────────────────────────────────────────

        // Lấy userId từ JWT claim (ClaimTypes.NameIdentifier = user.Id.ToString()).
        // Trả về null nếu claim không tồn tại hoặc không parse được.
        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var userId) ? userId : null;
        }
    }
}
