using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Matches;

namespace PlayCourt.Application.Interfaces
{
    public interface IMatchService
    {
        Task<PagedResponse<List<MatchResponseDto>>> SearchAsync(int userId, MatchSearchRequestDto request);
        Task<ApiResponse<List<MatchResponseDto>>> GetRecommendedAsync(int userId, int limit = 20);
        Task<ApiResponse<MatchDetailResponseDto>> GetByIdAsync(int userId, int matchId);
        Task<ApiResponse<MatchResponseDto>> CreateAsync(int userId, CreateMatchRequestDto request);
        Task<ApiResponse<MatchResponseDto>> UpdateAsync(int userId, int matchId, UpdateMatchRequestDto request);
        Task<ApiResponse<MatchResponseDto>> CancelAsync(int userId, int matchId);
        Task<ApiResponse<MatchJoinRequestDto>> RequestToJoinAsync(int userId, int matchId);
        Task<ApiResponse<MatchJoinRequestDto>> CancelJoinRequestAsync(int userId, int matchId);
        Task<ApiResponse<List<MatchJoinRequestDto>>> GetJoinRequestsAsync(int userId, int matchId);
        Task<ApiResponse<MatchJoinRequestDto>> RespondToJoinRequestAsync(
            int userId,
            int matchId,
            int requestId,
            RespondJoinRequestDto request);
        Task<ApiResponse<MatchResponseDto>> LeaveAsync(int userId, int matchId);
        Task<ApiResponse<List<PlayerMatchCandidateDto>>> GetCandidatesAsync(int userId, int matchId, int limit = 20);
        Task<ApiResponse<MatchInvitationDto>> InviteAsync(
            int userId,
            int matchId,
            CreateMatchInvitationDto request);
        Task<ApiResponse<List<MatchInvitationDto>>> GetMyInvitationsAsync(int userId);
        Task<ApiResponse<MatchInvitationDto>> RespondToInvitationAsync(
            int userId,
            int invitationId,
            RespondMatchInvitationDto request);
    }
}
