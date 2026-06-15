using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCourt.API.Authorization;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.PricingRules;
using PlayCourt.Application.Interfaces;
using System.Security.Claims;

namespace PlayCourt.API.Controllers
{
    [ApiController]
    public sealed class PricingRulesController : ControllerBase
    {
        private readonly IPricingRuleService _pricingRuleService;

        public PricingRulesController(IPricingRuleService pricingRuleService)
        {
            _pricingRuleService = pricingRuleService;
        }

        // GET /api/courts/{courtId}/pricing-rules
        // Public — không cần đăng nhập.
        [HttpGet("api/courts/{courtId:int}/pricing-rules")]
        public async Task<IActionResult> GetByCourt(int courtId)
        {
            var response = await _pricingRuleService.GetByCourtAsync(courtId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // POST /api/courts/{courtId}/pricing-rules
        // Chỉ CourtOwner sở hữu court mới được tạo.
        [HttpPost("api/courts/{courtId:int}/pricing-rules")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> Create(int courtId, [FromBody] CreatePricingRuleRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return BadRequest(ApiResponse<PricingRuleDto>.Fail("Validation failed", errors));
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId is null)
            {
                return Unauthorized(ApiResponse<PricingRuleDto>.Fail("Không xác định được danh tính người dùng."));
            }

            var response = await _pricingRuleService.CreateAsync(courtId, currentUserId.Value, request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // PUT /api/pricing-rules/{id}
        // Chỉ CourtOwner sở hữu court mới được cập nhật.
        [HttpPut("api/pricing-rules/{id:int}")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePricingRuleRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return BadRequest(ApiResponse<PricingRuleDto>.Fail("Validation failed", errors));
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId is null)
            {
                return Unauthorized(ApiResponse<PricingRuleDto>.Fail("Không xác định được danh tính người dùng."));
            }

            var response = await _pricingRuleService.UpdateAsync(id, currentUserId.Value, request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // DELETE /api/pricing-rules/{id}
        // Chỉ CourtOwner sở hữu court mới được xóa. Hard-delete.
        [HttpDelete("api/pricing-rules/{id:int}")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId is null)
            {
                return Unauthorized(ApiResponse<object>.Fail("Không xác định được danh tính người dùng."));
            }

            var response = await _pricingRuleService.DeleteAsync(id, currentUserId.Value);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // ─── PRIVATE HELPERS ─────────────────────────────────────────────────────

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var userId) ? userId : null;
        }
    }
}
