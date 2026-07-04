using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCourt.API.Authorization;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Payments;
using PlayCourt.Application.Interfaces;

namespace PlayCourt.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("bookings/{bookingId:int}/payos")]
        [Authorize(Policy = ApiPolicies.Player)]
        public async Task<IActionResult> CreatePayOsPaymentLink(int bookingId)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _paymentService.CreatePayOsPaymentLinkAsync(userId, bookingId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("bookings/{bookingId:int}/sync-payos")]
        [Authorize(Policy = ApiPolicies.Player)]
        public async Task<IActionResult> SyncPayOsPayment(int bookingId)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _paymentService.SyncPayOsPaymentAsync(userId, bookingId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("bookings/{bookingId:int}")]
        [Authorize]
        public async Task<IActionResult> GetBookingPayments(int bookingId)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _paymentService.GetBookingPaymentsAsync(userId, bookingId);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("payos/webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> HandlePayOsWebhook([FromBody] JsonElement webhookBody)
        {
            var response = await _paymentService.HandlePayOsWebhookAsync(webhookBody);
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
    }
}
