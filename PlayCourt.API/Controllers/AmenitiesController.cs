using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCourt.Application.DTOs.Amenities;
using PlayCourt.Application.Interfaces;
using PlayCourt.API.Authorization;

namespace PlayCourt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AmenitiesController : ControllerBase
{
    private readonly IAmenityService _amenityService;

    public AmenitiesController(IAmenityService amenityService)
    {
        _amenityService = amenityService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var response = await _amenityService.GetAllAmenitiesAsync();
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _amenityService.GetAmenityByIdAsync(id);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Policy = ApiPolicies.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateAmenityRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest();
        var response = await _amenityService.CreateAmenityAsync(request);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = ApiPolicies.Admin)]
    public async Task<IActionResult> Update(int id, [FromBody] CreateAmenityRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest();
        var response = await _amenityService.UpdateAmenityAsync(id, request);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = ApiPolicies.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _amenityService.DeleteAmenityAsync(id);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }
}
