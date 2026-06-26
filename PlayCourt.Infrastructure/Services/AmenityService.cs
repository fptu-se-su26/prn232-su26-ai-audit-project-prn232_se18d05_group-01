using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Amenities;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Entities;
using PlayCourt.Infrastructure.Data;

namespace PlayCourt.Infrastructure.Services;

public sealed class AmenityService : IAmenityService
{
    private readonly PlayCourtDbContext _dbContext;

    public AmenityService(PlayCourtDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<IReadOnlyCollection<AmenityDto>>> GetAllAmenitiesAsync()
    {
        var amenities = await _dbContext.Amenities
            .AsNoTracking()
            .Select(a => new AmenityDto { Id = a.Id, Name = a.Name })
            .ToListAsync();

        return ApiResponse<IReadOnlyCollection<AmenityDto>>.Ok(amenities, "Amenities retrieved successfully.");
    }

    public async Task<ApiResponse<AmenityDto>> GetAmenityByIdAsync(int id)
    {
        var amenity = await _dbContext.Amenities
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (amenity is null)
            return ApiResponse<AmenityDto>.Fail("Amenity not found.");

        return ApiResponse<AmenityDto>.Ok(new AmenityDto { Id = amenity.Id, Name = amenity.Name }, "Amenity retrieved successfully.");
    }

    public async Task<ApiResponse<AmenityDto>> CreateAmenityAsync(CreateAmenityRequestDto request)
    {
        if (await _dbContext.Amenities.AnyAsync(a => a.Name.ToLower() == request.Name.ToLower()))
            return ApiResponse<AmenityDto>.Fail("Amenity with this name already exists.");

        var amenity = new Amenity { Name = request.Name.Trim() };
        _dbContext.Amenities.Add(amenity);
        await _dbContext.SaveChangesAsync();

        return ApiResponse<AmenityDto>.Ok(new AmenityDto { Id = amenity.Id, Name = amenity.Name }, "Amenity created successfully.");
    }

    public async Task<ApiResponse<AmenityDto>> UpdateAmenityAsync(int id, CreateAmenityRequestDto request)
    {
        var amenity = await _dbContext.Amenities.FirstOrDefaultAsync(a => a.Id == id);
        if (amenity is null)
            return ApiResponse<AmenityDto>.Fail("Amenity not found.");

        if (await _dbContext.Amenities.AnyAsync(a => a.Id != id && a.Name.ToLower() == request.Name.ToLower()))
            return ApiResponse<AmenityDto>.Fail("Amenity with this name already exists.");

        amenity.Name = request.Name.Trim();
        await _dbContext.SaveChangesAsync();

        return ApiResponse<AmenityDto>.Ok(new AmenityDto { Id = amenity.Id, Name = amenity.Name }, "Amenity updated successfully.");
    }

    public async Task<ApiResponse<object>> DeleteAmenityAsync(int id)
    {
        var amenity = await _dbContext.Amenities.FirstOrDefaultAsync(a => a.Id == id);
        if (amenity is null)
            return ApiResponse<object>.Fail("Amenity not found.");

        _dbContext.Amenities.Remove(amenity);
        await _dbContext.SaveChangesAsync();

        return ApiResponse<object>.Ok(null, "Amenity deleted successfully.");
    }
}
