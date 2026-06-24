using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCourt.API.Authorization;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.CourtOwners;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Enums;

namespace PlayCourt.API.Controllers
{
    [ApiController]
    [Authorize(Policy = ApiPolicies.Admin)]
    [Route("api/court-owners")]
    public sealed class CourtOwnersController : ControllerBase
    {
        private readonly ICourtOwnerService _courtOwnerService;

        public CourtOwnersController(ICourtOwnerService courtOwnerService)
        {
            _courtOwnerService = courtOwnerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CourtOwnerVerificationStatus? status)
        {
            var response = await _courtOwnerService.GetAllAsync(status);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _courtOwnerService.GetByIdAsync(id);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPatch("{id:int}/verification-status")]
        public async Task<IActionResult> UpdateVerificationStatus(
            int id,
            [FromBody] UpdateCourtOwnerVerificationStatusRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(value => value.Errors)
                    .Select(error => error.ErrorMessage);

                return BadRequest(ApiResponse<CourtOwnerDetailDto>.Fail("Validation failed", errors));
            }

            var response = await _courtOwnerService.UpdateVerificationStatusAsync(id, request);

            if (!response.Success)
            {
                return response.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
                    ? NotFound(response)
                    : BadRequest(response);
            }

            return Ok(response);
        }
    }
}
