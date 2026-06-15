using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCourt.API.Authorization;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.CourtSchedules;
using PlayCourt.Application.Interfaces;
using System.Security.Claims;

namespace PlayCourt.API.Controllers
{
    [ApiController]
    public sealed class CourtSchedulesController : ControllerBase
    {
        private readonly ICourtScheduleService _courtScheduleService;

        public CourtSchedulesController(ICourtScheduleService courtScheduleService)
        {
            _courtScheduleService = courtScheduleService;
        }

        // GET /api/courts/{courtId}/schedules
        [HttpGet("api/courts/{courtId:int}/schedules")]
        public async Task<IActionResult> GetByCourt(int courtId)
        {
            var response = await _courtScheduleService.GetByCourtAsync(courtId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // POST /api/courts/{courtId}/schedules
        [HttpPost("api/courts/{courtId:int}/schedules")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> Create(int courtId, [FromBody] CreateCourtScheduleRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return BadRequest(ApiResponse<CourtScheduleDto>.Fail("Validation failed", errors));
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId is null)
            {
                return Unauthorized(ApiResponse<CourtScheduleDto>.Fail("Không xác định được danh tính người dùng."));
            }

            var response = await _courtScheduleService.CreateAsync(courtId, currentUserId.Value, request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // DELETE /api/court-schedules/{id}
        [HttpDelete("api/court-schedules/{id:int}")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId is null)
            {
                return Unauthorized(ApiResponse<object>.Fail("Không xác định được danh tính người dùng."));
            }

            var response = await _courtScheduleService.DeleteAsync(id, currentUserId.Value);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var userId) ? userId : null;
        }
    }
}
