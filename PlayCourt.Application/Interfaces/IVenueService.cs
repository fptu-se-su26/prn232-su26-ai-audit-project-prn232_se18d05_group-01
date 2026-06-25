using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Venues;

namespace PlayCourt.Application.Interfaces;

public interface IVenueService
{
    Task<ApiResponse<VenueResponseDto>> CreateVenueAsync(
        int userId,
        CreateVenueRequestDto request);

    Task<ApiResponse<IReadOnlyCollection<VenueResponseDto>>> GetMyVenuesAsync(int userId);

    Task<ApiResponse<VenueResponseDto>> GetVenueByIdAsync(
        int userId,
        int venueId);

    Task<ApiResponse<VenueResponseDto>> UpdateVenueAsync(
        int userId,
        int venueId,
        UpdateVenueRequestDto request);

    Task<ApiResponse<object>> DeleteVenueAsync(int userId, int venueId);

    /// <summary>Tìm kiếm venues public với filter và phân trang đầy đủ (có TotalCount).</summary>
    Task<PagedResponse<IReadOnlyCollection<VenueResponseDto>>> GetAllVenuesAsync(VenueSearchRequestDto request);

    Task<ApiResponse<VenueResponseDto>> GetPublicVenueByIdAsync(int venueId);

    // Images
    Task<ApiResponse<VenueImageDto>> AddImageAsync(int userId, int venueId, string imageUrl, bool isCover);
    Task<ApiResponse<object>> DeleteImageAsync(int userId, int venueId, int imageId);
    Task<ApiResponse<VenueImageDto>> SetCoverImageAsync(int userId, int venueId, int imageId);

    // Amenities
    Task<ApiResponse<PlayCourt.Application.DTOs.Amenities.AmenityDto>> AddVenueAmenityAsync(int userId, int venueId, int amenityId);
    Task<ApiResponse<object>> RemoveVenueAmenityAsync(int userId, int venueId, int amenityId);

    // Opening Hours
    Task<ApiResponse<IReadOnlyCollection<VenueOpeningHourDto>>> GetOpeningHoursAsync(int venueId);
    Task<ApiResponse<IReadOnlyCollection<VenueOpeningHourDto>>> UpdateOpeningHoursAsync(int userId, int venueId, UpdateOpeningHoursRequestDto request);

    // Owner detail
    Task<ApiResponse<VenueResponseDto>> GetMyVenueByIdAsync(int userId, int venueId);

    // Favorites
    Task<ApiResponse<FavoriteVenueResponseDto>> AddFavoriteAsync(int userId, int venueId);
    Task<ApiResponse<object>> RemoveFavoriteAsync(int userId, int venueId);
    Task<ApiResponse<IReadOnlyCollection<FavoriteVenueResponseDto>>> GetMyFavoritesAsync(int userId);

    // Dashboard
    Task<ApiResponse<VenueStatsResponseDto>> GetOwnerStatsAsync(int userId);
}
