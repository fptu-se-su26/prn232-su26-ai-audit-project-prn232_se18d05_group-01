using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayCourt.API.Authorization;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Matches;
using PlayCourt.Application.Interfaces;

namespace PlayCourt.API.Controllers
{
    [ApiController]
    [Authorize(Policy = ApiPolicies.Player)]
    [Route("api/[controller]")]
    public sealed class MatchesController : ControllerBase
    {
        private readonly IMatchService _matchService;

        public MatchesController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] MatchSearchRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(PagedResponse<List<MatchResponseDto>>.Fail(
                    "Validation failed",
                    GetModelStateErrors()));
            }

            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _matchService.SearchAsync(userId, request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("recommended")]
        public async Task<IActionResult> GetRecommended([FromQuery] int limit = 20)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _matchService.GetRecommendedAsync(userId, limit);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _matchService.GetByIdAsync(userId, id);
            return response.Success ? Ok(response) : HandleFailure(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMatchRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationError<MatchResponseDto>();
            }

            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _matchService.CreateAsync(userId, request);
            if (!response.Success)
            {
                return HandleFailure(response);
            }

            return CreatedAtAction(nameof(GetById), new { id = response.Data!.Id }, response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMatchRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationError<MatchResponseDto>();
            }

            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _matchService.UpdateAsync(userId, id, request);
            return response.Success ? Ok(response) : HandleFailure(response);
        }

        [HttpPatch("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _matchService.CancelAsync(userId, id);
            return response.Success ? Ok(response) : HandleFailure(response);
        }

        [HttpPost("{id:int}/join-requests")]
        public async Task<IActionResult> RequestToJoin(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _matchService.RequestToJoinAsync(userId, id);
            return response.Success ? Ok(response) : HandleFailure(response);
        }

        [HttpDelete("{id:int}/join-requests/me")]
        public async Task<IActionResult> CancelMyJoinRequest(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _matchService.CancelJoinRequestAsync(userId, id);
            return response.Success ? Ok(response) : HandleFailure(response);
        }

        [HttpGet("{id:int}/join-requests")]
        public async Task<IActionResult> GetJoinRequests(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _matchService.GetJoinRequestsAsync(userId, id);
            return response.Success ? Ok(response) : HandleFailure(response);
        }

        [HttpPatch("{id:int}/join-requests/{requestId:int}")]
        public async Task<IActionResult> RespondToJoinRequest(
            int id,
            int requestId,
            [FromBody] RespondJoinRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationError<MatchJoinRequestDto>();
            }

            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _matchService.RespondToJoinRequestAsync(
                userId,
                id,
                requestId,
                request);
            return response.Success ? Ok(response) : HandleFailure(response);
        }

        [HttpDelete("{id:int}/participants/me")]
        public async Task<IActionResult> Leave(int id)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _matchService.LeaveAsync(userId, id);
            return response.Success ? Ok(response) : HandleFailure(response);
        }

        [HttpGet("{id:int}/candidates")]
        public async Task<IActionResult> GetCandidates(int id, [FromQuery] int limit = 20)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _matchService.GetCandidatesAsync(userId, id, limit);
            return response.Success ? Ok(response) : HandleFailure(response);
        }

        [HttpPost("{id:int}/invitations")]
        public async Task<IActionResult> Invite(
            int id,
            [FromBody] CreateMatchInvitationDto request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationError<MatchInvitationDto>();
            }

            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _matchService.InviteAsync(userId, id, request);
            return response.Success ? Ok(response) : HandleFailure(response);
        }

        [HttpGet("invitations/me")]
        public async Task<IActionResult> GetMyInvitations()
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _matchService.GetMyInvitationsAsync(userId);
            return response.Success ? Ok(response) : HandleFailure(response);
        }

        [HttpPatch("invitations/{invitationId:int}")]
        public async Task<IActionResult> RespondToInvitation(
            int invitationId,
            [FromBody] RespondMatchInvitationDto request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationError<MatchInvitationDto>();
            }

            if (!TryGetCurrentUserId(out var userId))
            {
                return InvalidToken();
            }

            var response = await _matchService.RespondToInvitationAsync(
                userId,
                invitationId,
                request);
            return response.Success ? Ok(response) : HandleFailure(response);
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
        }

        private IActionResult InvalidToken()
        {
            return Unauthorized(ApiResponse<object>.Fail("Invalid authentication token."));
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

        private IActionResult HandleFailure<T>(ApiResponse<T> response)
        {
            if (response.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(response);
            }

            if (response.Message.StartsWith("Only the host", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(StatusCodes.Status403Forbidden, response);
            }

            if (response.Message.Contains("already", StringComparison.OrdinalIgnoreCase) ||
                response.Message.Contains("no longer", StringComparison.OrdinalIgnoreCase) ||
                response.Message.Contains("cannot leave", StringComparison.OrdinalIgnoreCase))
            {
                return Conflict(response);
            }

            return BadRequest(response);
        }
    }
}
