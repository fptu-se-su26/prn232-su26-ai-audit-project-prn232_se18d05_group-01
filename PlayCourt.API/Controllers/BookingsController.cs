using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCourt.API.Authorization;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Bookings;
using PlayCourt.Application.Interfaces;

namespace PlayCourt.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        [Authorize(Policy = ApiPolicies.Player)]
        public async Task<IActionResult> Create([FromBody] CreateBookingRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationError<BookingResponseDto>();
            }

            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _bookingService.CreateAsync(userId, request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _bookingService.GetByIdAsync(userId, id);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("me")]
        [Authorize(Policy = ApiPolicies.Player)]
        public async Task<IActionResult> GetMyBookings([FromQuery] BookingQueryDto query)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _bookingService.GetMyBookingsAsync(userId, query);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("~/api/venues/{venueId:int}/bookings")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> GetVenueBookings(int venueId, [FromQuery] BookingQueryDto query)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _bookingService.GetVenueBookingsAsync(userId, venueId, query);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("~/api/courts/{courtId:int}/bookings")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> GetCourtBookings(int courtId, [FromQuery] BookingQueryDto query)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _bookingService.GetCourtBookingsAsync(userId, courtId, query);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("~/api/courts/{courtId:int}/availability")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckAvailability(
            int courtId,
            [FromQuery] BookingAvailabilityRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationError<BookingAvailabilityResponseDto>();
            }

            var response = await _bookingService.CheckAvailabilityAsync(courtId, request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPatch("{id:int}/cancel")]
        [Authorize(Policy = ApiPolicies.Player)]
        public async Task<IActionResult> Cancel(int id, [FromBody] UpdateBookingStatusRequestDto request)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _bookingService.CancelByPlayerAsync(userId, id, request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPatch("{id:int}/confirm")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> Confirm(int id, [FromBody] UpdateBookingStatusRequestDto request)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _bookingService.ConfirmByOwnerAsync(userId, id, request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPatch("{id:int}/reject")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> Reject(int id, [FromBody] UpdateBookingStatusRequestDto request)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _bookingService.RejectByOwnerAsync(userId, id, request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPatch("{id:int}/complete")]
        [Authorize(Policy = ApiPolicies.CourtOwner)]
        public async Task<IActionResult> Complete(int id, [FromBody] UpdateBookingStatusRequestDto request)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _bookingService.CompleteByOwnerAsync(userId, id, request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
        }

        private IActionResult InvalidToken()
        {
            return Unauthorized(ApiResponse<object>.Fail("Invalid token."));
        }

        private IActionResult ValidationError<T>()
        {
            return BadRequest(ApiResponse<T>.Fail("Validation failed", GetModelStateErrors()));
        }

        private IEnumerable<string> GetModelStateErrors()
        {
            return ModelState.Values
                .SelectMany(value => value.Errors)
                .Select(error => error.ErrorMessage);
        }
    }
}
