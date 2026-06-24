using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Amenities;

namespace PlayCourt.Application.Interfaces;

public interface IAmenityService
{
    Task<ApiResponse<IReadOnlyCollection<AmenityDto>>> GetAllAmenitiesAsync();
    Task<ApiResponse<AmenityDto>> GetAmenityByIdAsync(int id);
    Task<ApiResponse<AmenityDto>> CreateAmenityAsync(CreateAmenityRequestDto request);
    Task<ApiResponse<AmenityDto>> UpdateAmenityAsync(int id, CreateAmenityRequestDto request);
    Task<ApiResponse<object>> DeleteAmenityAsync(int id);
}
