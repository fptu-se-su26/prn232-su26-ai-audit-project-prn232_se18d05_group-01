using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Notifications;
using PlayCourt.Application.Interfaces;

namespace PlayCourt.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications([FromQuery] NotificationQueryDto query)
        {
            if (!ModelState.IsValid)
            {
                return ValidationError<IReadOnlyCollection<NotificationDto>>();
            }

            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _notificationService.GetMyNotificationsAsync(userId, query);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(response);
        }

        [HttpPatch("{id:int}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _notificationService.MarkAsReadAsync(userId, id);
            if (response.Success)
            {
                return Ok(response);
            }

            return NotFound(response);
        }

        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _notificationService.DeleteAsync(userId, id);
            if (response.Success)
            {
                return Ok(response);
            }

            return NotFound(response);
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
