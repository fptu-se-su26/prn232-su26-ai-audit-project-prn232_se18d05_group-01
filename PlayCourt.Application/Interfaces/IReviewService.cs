using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Application.Interfaces
{
    public interface IReviewService
    {
        Task<ApiResponse<ReviewResponseDto>> CreateReviewAsync(int userId, CreateReviewRequestDto request);
        Task<PagedResponse<IReadOnlyCollection<ReviewResponseDto>>> GetVenueReviewsAsync(int venueId, int page, int pageSize);
        Task<PagedResponse<IReadOnlyCollection<ReviewResponseDto>>> GetCourtReviewsAsync(int courtId, int page, int pageSize);
        Task<ApiResponse<ReviewResponseDto>> UpdateReviewAsync(int userId, int reviewId, UpdateReviewRequestDto request);
        Task<ApiResponse<object>> DeleteReviewAsync(int userId, int reviewId);
        Task<ApiResponse<object>> ReportReviewAsync(int userId, int reviewId);
        Task<ApiResponse<ReviewResponseDto>> ModerateReviewAsync(int reviewId, ReviewStatus status);
        Task<ApiResponse<ReviewImageDto>> AddReviewImageAsync(int userId, int reviewId, AddReviewImageRequestDto request);
        Task<ApiResponse<object>> DeleteReviewImageAsync(int userId, int reviewId, int imageId);
        Task<ApiResponse<ReviewStatsDto>> GetVenueStatsAsync(int venueId);
        Task<ApiResponse<ReviewStatsDto>> GetCourtStatsAsync(int courtId);
        Task<PagedResponse<IReadOnlyCollection<ReviewResponseDto>>> GetMyReviewsAsync(int userId, int page, int pageSize);
    }
}
