using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;

namespace PlayCourt.Infrastructure.Services
{
    public sealed class ReviewService : IReviewService
    {
        private readonly PlayCourtDbContext _dbContext;

        public ReviewService(PlayCourtDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ApiResponse<ReviewResponseDto>> CreateReviewAsync(int userId, CreateReviewRequestDto request)
        {
            var playerProfile = await _dbContext.UserProfiles
                .FirstOrDefaultAsync(up => up.UserId == userId);
            if (playerProfile == null)
            {
                return ApiResponse<ReviewResponseDto>.Fail("Player profile not found.");
            }

            var booking = await _dbContext.Bookings
                .Include(b => b.Court)
                    .ThenInclude(c => c.Venue)
                .FirstOrDefaultAsync(b => b.Id == request.BookingId && b.UserProfileId == playerProfile.Id);
            if (booking == null)
            {
                return ApiResponse<ReviewResponseDto>.Fail("Booking not found or does not belong to you.");
            }

            if (booking.Status != BookingStatus.Completed)
            {
                return ApiResponse<ReviewResponseDto>.Fail("You can only review completed bookings.");
            }

            // Enforce rating ranges and 0.5 star step increments
            if (request.Rating < 1.0m || request.Rating > 5.0m || (request.Rating * 10) % 5 != 0)
            {
                return ApiResponse<ReviewResponseDto>.Fail("Rating must be between 1.0 and 5.0 in increments of 0.5.");
            }

            // Check unique review constraint
            var existingReview = await _dbContext.Reviews
                .AnyAsync(r => r.BookingId == request.BookingId && !r.IsDeleted);
            if (existingReview)
            {
                return ApiResponse<ReviewResponseDto>.Fail("You have already reviewed this booking.");
            }

            // Check maximum review images limit
            if (request.ImageUrls != null && request.ImageUrls.Count > 5)
            {
                return ApiResponse<ReviewResponseDto>.Fail("You can upload at most 5 images.");
            }

            var review = new Review
            {
                PlayerId = playerProfile.Id,
                BookingId = request.BookingId,
                Rating = request.Rating,
                ReviewText = request.ReviewText?.Trim(),
                Status = ReviewStatus.Visible,
                CreatedAt = DateTimeOffset.Now,
                IsDeleted = false
            };

            if (request.ImageUrls != null)
            {
                short index = 0;
                foreach (var url in request.ImageUrls)
                {
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        review.Images.Add(new ReviewImage
                        {
                            ImageUrl = url.Trim(),
                            DisplayOrder = index++,
                            CreatedAt = DateTimeOffset.Now
                        });
                    }
                }
            }

            _dbContext.Reviews.Add(review);
            await _dbContext.SaveChangesAsync();

            // Load navigations for mapper
            var savedReview = await _dbContext.Reviews
                .Include(r => r.Player)
                .Include(r => r.Images)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Court)
                        .ThenInclude(c => c.Venue)
                .FirstOrDefaultAsync(r => r.Id == review.Id);

            return ApiResponse<ReviewResponseDto>.Ok(MapToResponse(savedReview!), "Review created successfully.");
        }

        public async Task<PagedResponse<IReadOnlyCollection<ReviewResponseDto>>> GetVenueReviewsAsync(int venueId, int page, int pageSize)
        {
            var query = _dbContext.Reviews
                .Where(r => r.Booking.Court.VenueId == venueId && r.Status == ReviewStatus.Visible && !r.IsDeleted);

            var totalCount = await query.CountAsync();

            var reviews = await query
                .Include(r => r.Player)
                .Include(r => r.Images)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Court)
                        .ThenInclude(c => c.Venue)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = reviews.Select(MapToResponse).ToList();
            return PagedResponse<IReadOnlyCollection<ReviewResponseDto>>.Ok(dtos, totalCount, page, pageSize, "Reviews retrieved successfully.");
        }

        public async Task<PagedResponse<IReadOnlyCollection<ReviewResponseDto>>> GetCourtReviewsAsync(int courtId, int page, int pageSize)
        {
            var query = _dbContext.Reviews
                .Where(r => r.Booking.CourtId == courtId && r.Status == ReviewStatus.Visible && !r.IsDeleted);

            var totalCount = await query.CountAsync();

            var reviews = await query
                .Include(r => r.Player)
                .Include(r => r.Images)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Court)
                        .ThenInclude(c => c.Venue)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = reviews.Select(MapToResponse).ToList();
            return PagedResponse<IReadOnlyCollection<ReviewResponseDto>>.Ok(dtos, totalCount, page, pageSize, "Reviews retrieved successfully.");
        }

        public async Task<ApiResponse<ReviewResponseDto>> UpdateReviewAsync(int userId, int reviewId, UpdateReviewRequestDto request)
        {
            var playerProfile = await _dbContext.UserProfiles
                .FirstOrDefaultAsync(up => up.UserId == userId);
            if (playerProfile == null)
            {
                return ApiResponse<ReviewResponseDto>.Fail("Player profile not found.");
            }

            var review = await _dbContext.Reviews
                .Include(r => r.Player)
                .Include(r => r.Images)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Court)
                        .ThenInclude(c => c.Venue)
                .FirstOrDefaultAsync(r => r.Id == reviewId && !r.IsDeleted);

            if (review == null)
            {
                return ApiResponse<ReviewResponseDto>.Fail("Review not found.");
            }

            if (review.PlayerId != playerProfile.Id)
            {
                return ApiResponse<ReviewResponseDto>.Fail("You are not authorized to update this review.");
            }

            // Guard: Block editing if the review is not Visible (e.g. Reported or Hidden)
            if (review.Status != ReviewStatus.Visible)
            {
                return ApiResponse<ReviewResponseDto>.Fail("Cannot edit a review that is reported or hidden by administrator.");
            }

            if (request.Rating < 1.0m || request.Rating > 5.0m || (request.Rating * 10) % 5 != 0)
            {
                return ApiResponse<ReviewResponseDto>.Fail("Rating must be between 1.0 and 5.0 in increments of 0.5.");
            }

            review.Rating = request.Rating;
            review.ReviewText = request.ReviewText?.Trim();
            review.UpdatedAt = DateTimeOffset.Now;

            await _dbContext.SaveChangesAsync();
            return ApiResponse<ReviewResponseDto>.Ok(MapToResponse(review), "Review updated successfully.");
        }

        public async Task<ApiResponse<object>> DeleteReviewAsync(int userId, int reviewId)
        {
            var playerProfile = await _dbContext.UserProfiles
                .FirstOrDefaultAsync(up => up.UserId == userId);
            if (playerProfile == null)
            {
                return ApiResponse<object>.Fail("Player profile not found.");
            }

            var review = await _dbContext.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId && !r.IsDeleted);
            if (review == null)
            {
                return ApiResponse<object>.Fail("Review not found.");
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (review.PlayerId != playerProfile.Id && user?.Role != UserRole.Admin)
            {
                return ApiResponse<object>.Fail("You are not authorized to delete this review.");
            }

            // Soft delete
            review.IsDeleted = true;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<object>.Ok(null, "Review deleted successfully.");
        }

        public async Task<ApiResponse<object>> ReportReviewAsync(int userId, int reviewId)
        {
            var playerProfile = await _dbContext.UserProfiles
                .FirstOrDefaultAsync(up => up.UserId == userId);
            if (playerProfile == null)
            {
                return ApiResponse<object>.Fail("Player profile not found.");
            }

            var review = await _dbContext.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId && !r.IsDeleted);
            if (review == null)
            {
                return ApiResponse<object>.Fail("Review not found.");
            }

            // Guard: Cannot report self review
            if (review.PlayerId == playerProfile.Id)
            {
                return ApiResponse<object>.Fail("You cannot report your own review.");
            }

            // Guard: Cannot report multiple times
            if (review.Status == ReviewStatus.Reported)
            {
                return ApiResponse<object>.Fail("This review has already been reported.");
            }

            if (review.Status == ReviewStatus.Hidden)
            {
                return ApiResponse<object>.Fail("This review is already hidden.");
            }

            review.Status = ReviewStatus.Reported;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<object>.Ok(null, "Review reported successfully.");
        }

        public async Task<ApiResponse<ReviewResponseDto>> ModerateReviewAsync(int reviewId, ReviewStatus status)
        {
            var review = await _dbContext.Reviews
                .Include(r => r.Player)
                .Include(r => r.Images)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Court)
                        .ThenInclude(c => c.Venue)
                .FirstOrDefaultAsync(r => r.Id == reviewId && !r.IsDeleted);

            if (review == null)
            {
                return ApiResponse<ReviewResponseDto>.Fail("Review not found.");
            }

            review.Status = status;
            review.UpdatedAt = DateTimeOffset.Now;
            await _dbContext.SaveChangesAsync();

            return ApiResponse<ReviewResponseDto>.Ok(MapToResponse(review), "Review moderated successfully.");
        }

        public async Task<ApiResponse<ReviewImageDto>> AddReviewImageAsync(int userId, int reviewId, string imageUrl, short displayOrder)
        {
            var playerProfile = await _dbContext.UserProfiles
                .FirstOrDefaultAsync(up => up.UserId == userId);
            if (playerProfile == null)
            {
                return ApiResponse<ReviewImageDto>.Fail("Player profile not found.");
            }

            var review = await _dbContext.Reviews
                .Include(r => r.Images)
                .FirstOrDefaultAsync(r => r.Id == reviewId && !r.IsDeleted);

            if (review == null)
            {
                return ApiResponse<ReviewImageDto>.Fail("Review not found.");
            }

            // Verify ownership check in service
            if (review.PlayerId != playerProfile.Id)
            {
                return ApiResponse<ReviewImageDto>.Fail("You are not authorized to add images to this review.");
            }

            if (review.Images.Count >= 5)
            {
                return ApiResponse<ReviewImageDto>.Fail("You can upload at most 5 images.");
            }

            var image = new ReviewImage
            {
                ReviewId = reviewId,
                ImageUrl = imageUrl.Trim(),
                DisplayOrder = displayOrder,
                CreatedAt = DateTimeOffset.Now
            };

            _dbContext.ReviewImages.Add(image);
            await _dbContext.SaveChangesAsync();

            var dto = new ReviewImageDto
            {
                Id = image.Id,
                ReviewId = image.ReviewId,
                ImageUrl = image.ImageUrl,
                DisplayOrder = image.DisplayOrder,
                CreatedAt = image.CreatedAt
            };

            return ApiResponse<ReviewImageDto>.Ok(dto, "Image added successfully.");
        }

        public async Task<ApiResponse<object>> DeleteReviewImageAsync(int userId, int reviewId, int imageId)
        {
            var playerProfile = await _dbContext.UserProfiles
                .FirstOrDefaultAsync(up => up.UserId == userId);
            if (playerProfile == null)
            {
                return ApiResponse<object>.Fail("Player profile not found.");
            }

            var image = await _dbContext.ReviewImages
                .Include(i => i.Review)
                .FirstOrDefaultAsync(i => i.Id == imageId && i.ReviewId == reviewId);

            if (image == null)
            {
                return ApiResponse<object>.Fail("Image not found.");
            }

            // Verify ownership check
            if (image.Review.PlayerId != playerProfile.Id)
            {
                return ApiResponse<object>.Fail("You are not authorized to delete this image.");
            }

            _dbContext.ReviewImages.Remove(image);
            await _dbContext.SaveChangesAsync();

            return ApiResponse<object>.Ok(null, "Image deleted successfully.");
        }

        public async Task<ApiResponse<ReviewStatsDto>> GetVenueStatsAsync(int venueId)
        {
            var reviews = await _dbContext.Reviews
                .Where(r => r.Booking.Court.VenueId == venueId && r.Status == ReviewStatus.Visible && !r.IsDeleted)
                .Select(r => r.Rating)
                .ToListAsync();

            var totalCount = reviews.Count;
            var averageRating = totalCount > 0 ? reviews.Average() : 0.0m;

            var distribution = new Dictionary<decimal, int>
            {
                [1.0m] = 0,
                [1.5m] = 0,
                [2.0m] = 0,
                [2.5m] = 0,
                [3.0m] = 0,
                [3.5m] = 0,
                [4.0m] = 0,
                [4.5m] = 0,
                [5.0m] = 0
            };

            foreach (var rating in reviews)
            {
                if (distribution.ContainsKey(rating))
                {
                    distribution[rating]++;
                }
            }

            return ApiResponse<ReviewStatsDto>.Ok(new ReviewStatsDto
            {
                AverageRating = averageRating,
                TotalReviews = totalCount,
                RatingDistribution = distribution
            }, "Stats retrieved successfully.");
        }

        public async Task<ApiResponse<ReviewStatsDto>> GetCourtStatsAsync(int courtId)
        {
            var reviews = await _dbContext.Reviews
                .Where(r => r.Booking.CourtId == courtId && r.Status == ReviewStatus.Visible && !r.IsDeleted)
                .Select(r => r.Rating)
                .ToListAsync();

            var totalCount = reviews.Count;
            var averageRating = totalCount > 0 ? reviews.Average() : 0.0m;

            var distribution = new Dictionary<decimal, int>
            {
                [1.0m] = 0,
                [1.5m] = 0,
                [2.0m] = 0,
                [2.5m] = 0,
                [3.0m] = 0,
                [3.5m] = 0,
                [4.0m] = 0,
                [4.5m] = 0,
                [5.0m] = 0
            };

            foreach (var rating in reviews)
            {
                if (distribution.ContainsKey(rating))
                {
                    distribution[rating]++;
                }
            }

            return ApiResponse<ReviewStatsDto>.Ok(new ReviewStatsDto
            {
                AverageRating = averageRating,
                TotalReviews = totalCount,
                RatingDistribution = distribution
            }, "Stats retrieved successfully.");
        }

        public async Task<PagedResponse<IReadOnlyCollection<ReviewResponseDto>>> GetMyReviewsAsync(int userId, int page, int pageSize)
        {
            var playerProfile = await _dbContext.UserProfiles
                .FirstOrDefaultAsync(up => up.UserId == userId);
            if (playerProfile == null)
            {
                return PagedResponse<IReadOnlyCollection<ReviewResponseDto>>.Fail("Player profile not found.");
            }

            var query = _dbContext.Reviews
                .Where(r => r.PlayerId == playerProfile.Id && !r.IsDeleted);

            var totalCount = await query.CountAsync();

            var reviews = await query
                .Include(r => r.Player)
                .Include(r => r.Images)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Court)
                        .ThenInclude(c => c.Venue)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = reviews.Select(MapToResponse).ToList();
            return PagedResponse<IReadOnlyCollection<ReviewResponseDto>>.Ok(dtos, totalCount, page, pageSize, "Reviews retrieved successfully.");
        }

        private static ReviewResponseDto MapToResponse(Review review)
        {
            return new ReviewResponseDto
            {
                Id = review.Id,
                PlayerId = review.PlayerId,
                PlayerName = review.Player?.FullName ?? string.Empty,
                PlayerAvatar = review.Player?.AvatarUrl,
                BookingId = review.BookingId,
                VenueId = review.Booking?.Court?.VenueId ?? 0,
                VenueName = review.Booking?.Court?.Venue?.Name ?? string.Empty,
                CourtId = review.Booking?.CourtId ?? 0,
                CourtName = review.Booking?.Court?.Name ?? string.Empty,
                Rating = review.Rating,
                ReviewText = review.ReviewText,
                Status = review.Status.ToString(),
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt,
                Images = review.Images?.Select(i => new ReviewImageDto
                {
                    Id = i.Id,
                    ReviewId = i.ReviewId,
                    ImageUrl = i.ImageUrl,
                    DisplayOrder = i.DisplayOrder,
                    CreatedAt = i.CreatedAt
                }).ToList() ?? []
            };
        }
    }
}
