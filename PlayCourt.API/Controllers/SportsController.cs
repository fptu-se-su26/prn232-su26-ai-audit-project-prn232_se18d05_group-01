using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCourt.API.Authorization;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Sports;
using PlayCourt.Application.Interfaces;

namespace PlayCourt.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class SportsController : ControllerBase
    {
        private readonly ISportService _sportService;

        public SportsController(ISportService sportService)
        {
            _sportService = sportService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? isActive)
        {
            var response = await _sportService.GetAllAsync(isActive);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _sportService.GetByIdAsync(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Policy = ApiPolicies.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateSportRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(value => value.Errors)
                    .Select(error => error.ErrorMessage);

                return BadRequest(ApiResponse<SportDto>.Fail("Validation failed", errors));
            }

            var response = await _sportService.CreateAsync(request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = ApiPolicies.Admin)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSportRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(value => value.Errors)
                    .Select(error => error.ErrorMessage);

                return BadRequest(ApiResponse<SportDto>.Fail("Validation failed", errors));
            }

            var response = await _sportService.UpdateAsync(id, request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPatch("{id:int}/toggle-active")]
        [Authorize(Policy = ApiPolicies.Admin)]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var response = await _sportService.ToggleActiveAsync(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
