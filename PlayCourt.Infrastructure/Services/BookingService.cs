using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Bookings;
using PlayCourt.Application.DTOs.Notifications;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;

namespace PlayCourt.Infrastructure.Services
{
    public sealed class BookingService : IBookingService
    {
        private const decimal PlatformFeeRate = 0.05m;
        private readonly PlayCourtDbContext _dbContext;
        private readonly INotificationWriter _notificationWriter;

        public BookingService(PlayCourtDbContext dbContext, INotificationWriter notificationWriter)
        {
            _dbContext = dbContext;
            _notificationWriter = notificationWriter;
        }

        public async Task<ApiResponse<BookingResponseDto>> CreateAsync(int userId, CreateBookingRequestDto request)
        {
            var playerProfile = await GetActivePlayerProfileAsync(userId);
            if (playerProfile is null)
            {
                return ApiResponse<BookingResponseDto>.Fail("Player profile not found.");
            }

            var priceResult = await CalculatePriceAsync(request.CourtId, request.StartAt, request.EndAt);
            if (!priceResult.Success)
            {
                return ApiResponse<BookingResponseDto>.Fail(priceResult.Message);
            }

            var platformFee = Math.Round(priceResult.TotalPrice * PlatformFeeRate, 2);
            var booking = new Booking
            {
                UserProfileId = playerProfile.Id,
                CourtId = request.CourtId,
                StartAt = request.StartAt,
                EndAt = request.EndAt,
                TotalPrice = priceResult.TotalPrice,
                PlatformFee = platformFee,
                OwnerEarnings = priceResult.TotalPrice - platformFee,
                Status = BookingStatus.Pending,
                Note = Normalize(request.Note),
                CreatedAt = DateTimeOffset.Now
            };

            var ownerUserId = await _dbContext.Courts
                .Where(c => c.Id == request.CourtId)
                .Select(c => c.Venue.CourtOwnerProfile.UserProfile.UserId)
                .FirstOrDefaultAsync();

            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                await AcquireBookingSlotLockAsync(request.CourtId);

                var validation = await ValidateSlotAsync(request.CourtId, request.StartAt, request.EndAt);
                if (!validation.IsAvailable)
                {
                    await transaction.RollbackAsync();
                    return ApiResponse<BookingResponseDto>.Fail(validation.Reason ?? "Court slot is not available.");
                }

                _dbContext.Bookings.Add(booking);
                _dbContext.BookingStatusHistories.Add(new BookingStatusHistory
                {
                    Booking = booking,
                    OldStatus = null,
                    NewStatus = BookingStatus.Pending,
                    ChangedByUserId = userId,
                    Reason = "Booking created.",
                    CreatedAt = DateTimeOffset.Now
                });

                await _dbContext.SaveChangesAsync();

                // Notify court owner about new booking
                if (ownerUserId > 0)
                {
                    _notificationWriter.Add(new CreateNotificationRequest
                    {
                        UserId = ownerUserId,
                        Title = "Có đơn đặt sân mới",
                        Content = "Bạn vừa nhận được một yêu cầu đặt sân mới.",
                        Type = NotificationType.Booking,
                        ReferenceType = NotificationReferenceType.Booking,
                        ReferenceId = booking.Id
                    });
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            var savedBooking = await BookingQuery()
                .AsNoTracking()
                .SingleAsync(b => b.Id == booking.Id);

            return ApiResponse<BookingResponseDto>.Ok(MapToDto(savedBooking), "Booking created successfully.");
        }

        public async Task<ApiResponse<BookingResponseDto>> GetByIdAsync(int userId, int bookingId)
        {
            var booking = await BookingQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking is null)
            {
                return ApiResponse<BookingResponseDto>.Fail("Booking not found.");
            }

            var access = await CanAccessBookingAsync(userId, booking);
            if (!access)
            {
                return ApiResponse<BookingResponseDto>.Fail("You are not authorized to view this booking.");
            }

            return ApiResponse<BookingResponseDto>.Ok(MapToDto(booking), "Booking retrieved successfully.");
        }

        public async Task<PagedResponse<IReadOnlyCollection<BookingResponseDto>>> GetMyBookingsAsync(int userId, BookingQueryDto query)
        {
            var playerProfile = await GetActivePlayerProfileAsync(userId);
            if (playerProfile is null)
            {
                return PagedResponse<IReadOnlyCollection<BookingResponseDto>>.Fail("Player profile not found.");
            }

            var bookings = ApplyBookingFilters(BookingQuery().AsNoTracking(), query)
                .Where(b => b.UserProfileId == playerProfile.Id);

            return await ToPagedResponseAsync(bookings, query, "Bookings retrieved successfully.");
        }

        public async Task<PagedResponse<IReadOnlyCollection<BookingResponseDto>>> GetVenueBookingsAsync(int userId, int venueId, BookingQueryDto query)
        {
            var ownsVenue = await _dbContext.Venues
                .AnyAsync(v => v.Id == venueId && v.CourtOwnerProfile.UserProfile.UserId == userId);
            if (!ownsVenue)
            {
                return PagedResponse<IReadOnlyCollection<BookingResponseDto>>.Fail("Venue not found or you are not authorized to view its bookings.");
            }

            var bookings = ApplyBookingFilters(BookingQuery().AsNoTracking(), query)
                .Where(b => b.Court.VenueId == venueId);

            return await ToPagedResponseAsync(bookings, query, "Venue bookings retrieved successfully.");
        }

        public async Task<PagedResponse<IReadOnlyCollection<BookingResponseDto>>> GetCourtBookingsAsync(int userId, int courtId, BookingQueryDto query)
        {
            var ownsCourt = await _dbContext.Courts
                .AnyAsync(c => c.Id == courtId && c.Venue.CourtOwnerProfile.UserProfile.UserId == userId);
            if (!ownsCourt)
            {
                return PagedResponse<IReadOnlyCollection<BookingResponseDto>>.Fail("Court not found or you are not authorized to view its bookings.");
            }

            var bookings = ApplyBookingFilters(BookingQuery().AsNoTracking(), query)
                .Where(b => b.CourtId == courtId);

            return await ToPagedResponseAsync(bookings, query, "Court bookings retrieved successfully.");
        }

        public async Task<ApiResponse<BookingAvailabilityResponseDto>> CheckAvailabilityAsync(int courtId, BookingAvailabilityRequestDto request)
        {
            var validation = await ValidateSlotAsync(courtId, request.StartAt, request.EndAt);
            var response = new BookingAvailabilityResponseDto
            {
                CourtId = courtId,
                StartAt = request.StartAt,
                EndAt = request.EndAt,
                IsAvailable = validation.IsAvailable,
                Reason = validation.Reason
            };

            if (validation.IsAvailable)
            {
                var priceResult = await CalculatePriceAsync(courtId, request.StartAt, request.EndAt);
                if (priceResult.Success)
                {
                    response.EstimatedPrice = priceResult.TotalPrice;
                }
                else
                {
                    response.IsAvailable = false;
                    response.Reason = priceResult.Message;
                }
            }

            return ApiResponse<BookingAvailabilityResponseDto>.Ok(response, "Availability checked successfully.");
        }

        private async Task AcquireBookingSlotLockAsync(int courtId)
        {
            if (!_dbContext.Database.IsRelational())
            {
                return;
            }

            var currentTransaction = _dbContext.Database.CurrentTransaction
                ?? throw new InvalidOperationException("Booking slot lock requires an active transaction.");

            var connection = _dbContext.Database.GetDbConnection();
            await using var command = connection.CreateCommand();
            command.Transaction = currentTransaction.GetDbTransaction();
            command.CommandText = """
                DECLARE @result int;
                EXEC @result = sp_getapplock
                    @Resource = @resource,
                    @LockMode = 'Exclusive',
                    @LockOwner = 'Transaction',
                    @LockTimeout = 10000;
                SELECT @result;
                """;

            var resourceParameter = command.CreateParameter();
            resourceParameter.ParameterName = "@resource";
            resourceParameter.Value = $"booking-slot:court:{courtId}";
            command.Parameters.Add(resourceParameter);

            var result = (int)(await command.ExecuteScalarAsync()
                ?? throw new InvalidOperationException("Could not acquire booking slot lock."));

            if (result < 0)
            {
                throw new InvalidOperationException("Could not acquire booking slot lock.");
            }
        }

        public async Task<ApiResponse<BookingResponseDto>> CancelByPlayerAsync(int userId, int bookingId, UpdateBookingStatusRequestDto request)
        {
            var booking = await BookingQuery().FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking is null)
            {
                return ApiResponse<BookingResponseDto>.Fail("Booking not found.");
            }

            if (booking.UserProfile.UserId != userId)
            {
                return ApiResponse<BookingResponseDto>.Fail("You are not authorized to cancel this booking.");
            }

            if (booking.Status is not (BookingStatus.Pending or BookingStatus.Confirmed))
            {
                return ApiResponse<BookingResponseDto>.Fail("Only pending or confirmed bookings can be cancelled by player.");
            }

            // Notify court owner player cancelled booking
            var ownerUserId = booking.Court?.Venue?.CourtOwnerProfile?.UserProfile?.UserId ?? 0;
            if (ownerUserId > 0 && ownerUserId != userId)
            {
                _notificationWriter.Add(new CreateNotificationRequest
                {
                    UserId = ownerUserId,
                    Title = "Khách hàng đã hủy đặt sân",
                    Content = "Một đơn đặt sân tại cơ sở của bạn đã được khách hàng hủy.",
                    Type = NotificationType.Booking,
                    ReferenceType = NotificationReferenceType.Booking,
                    ReferenceId = booking.Id
                });
            }

            return await ChangeStatusAsync(booking, BookingStatus.CancelledByUser, userId, request.Reason, "Booking cancelled successfully.");
        }

        public async Task<ApiResponse<BookingResponseDto>> ConfirmByOwnerAsync(int userId, int bookingId, UpdateBookingStatusRequestDto request)
        {
            var booking = await BookingQuery().FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking is null)
            {
                return ApiResponse<BookingResponseDto>.Fail("Booking not found.");
            }

            if (!IsVenueOwner(userId, booking))
            {
                return ApiResponse<BookingResponseDto>.Fail("You are not authorized to confirm this booking.");
            }

            if (booking.Status != BookingStatus.Pending)
            {
                return ApiResponse<BookingResponseDto>.Fail("Only pending bookings can be confirmed.");
            }

            // Notify player booking confirmed
            if (booking.UserProfile.UserId > 0 && booking.UserProfile.UserId != userId)
            {
                _notificationWriter.Add(new CreateNotificationRequest
                {
                    UserId = booking.UserProfile.UserId,
                    Title = "Đặt sân đã được xác nhận",
                    Content = "Yêu cầu đặt sân của bạn đã được chủ sân xác nhận.",
                    Type = NotificationType.Booking,
                    ReferenceType = NotificationReferenceType.Booking,
                    ReferenceId = booking.Id
                });
            }

            return await ChangeStatusAsync(booking, BookingStatus.Confirmed, userId, request.Reason, "Booking confirmed successfully.");
        }

        public async Task<ApiResponse<BookingResponseDto>> RejectByOwnerAsync(int userId, int bookingId, UpdateBookingStatusRequestDto request)
        {
            var booking = await BookingQuery().FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking is null)
            {
                return ApiResponse<BookingResponseDto>.Fail("Booking not found.");
            }

            if (!IsVenueOwner(userId, booking))
            {
                return ApiResponse<BookingResponseDto>.Fail("You are not authorized to reject this booking.");
            }

            if (booking.Status != BookingStatus.Pending)
            {
                return ApiResponse<BookingResponseDto>.Fail("Only pending bookings can be rejected.");
            }

            // Notify player booking rejected
            if (booking.UserProfile.UserId > 0 && booking.UserProfile.UserId != userId)
            {
                var content = "Yêu cầu đặt sân của bạn đã bị chủ sân từ chối.";
                if (!string.IsNullOrWhiteSpace(request.Reason))
                {
                    content += $" Lý do: {request.Reason.Trim()}";
                }

                _notificationWriter.Add(new CreateNotificationRequest
                {
                    UserId = booking.UserProfile.UserId,
                    Title = "Đặt sân đã bị từ chối",
                    Content = content,
                    Type = NotificationType.Booking,
                    ReferenceType = NotificationReferenceType.Booking,
                    ReferenceId = booking.Id
                });
            }

            return await ChangeStatusAsync(booking, BookingStatus.CancelledByOwner, userId, request.Reason, "Booking rejected successfully.");
        }

        public async Task<ApiResponse<BookingResponseDto>> CompleteByOwnerAsync(int userId, int bookingId, UpdateBookingStatusRequestDto request)
        {
            var booking = await BookingQuery().FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking is null)
            {
                return ApiResponse<BookingResponseDto>.Fail("Booking not found.");
            }

            if (!IsVenueOwner(userId, booking))
            {
                return ApiResponse<BookingResponseDto>.Fail("You are not authorized to complete this booking.");
            }

            if (booking.Status != BookingStatus.Confirmed)
            {
                return ApiResponse<BookingResponseDto>.Fail("Only confirmed bookings can be completed.");
            }

            // Notify player booking completed
            if (booking.UserProfile.UserId > 0 && booking.UserProfile.UserId != userId)
            {
                _notificationWriter.Add(new CreateNotificationRequest
                {
                    UserId = booking.UserProfile.UserId,
                    Title = "Buổi đặt sân đã hoàn tất",
                    Content = "Buổi đặt sân của bạn đã được đánh dấu hoàn tất.",
                    Type = NotificationType.Booking,
                    ReferenceType = NotificationReferenceType.Booking,
                    ReferenceId = booking.Id
                });
            }

            return await ChangeStatusAsync(booking, BookingStatus.Completed, userId, request.Reason, "Booking completed successfully.");
        }

        private IQueryable<Booking> BookingQuery()
        {
            return _dbContext.Bookings
                .Include(b => b.UserProfile)
                .Include(b => b.Court)
                    .ThenInclude(c => c.Venue)
                        .ThenInclude(v => v.CourtOwnerProfile)
                            .ThenInclude(cop => cop.UserProfile);
        }

        private static IQueryable<Booking> ApplyBookingFilters(IQueryable<Booking> query, BookingQueryDto filter)
        {
            if (filter.Status.HasValue)
            {
                query = query.Where(b => b.Status == filter.Status.Value);
            }

            if (filter.From.HasValue)
            {
                query = query.Where(b => b.EndAt >= filter.From.Value);
            }

            if (filter.To.HasValue)
            {
                query = query.Where(b => b.StartAt <= filter.To.Value);
            }

            return query;
        }

        private async Task<PagedResponse<IReadOnlyCollection<BookingResponseDto>>> ToPagedResponseAsync(
            IQueryable<Booking> query,
            BookingQueryDto filter,
            string message)
        {
            var page = Math.Max(filter.Page, 1);
            var pageSize = Math.Clamp(filter.PageSize, 1, 100);
            var totalCount = await query.CountAsync();
            var bookings = await query
                .OrderByDescending(b => b.StartAt)
                .ThenByDescending(b => b.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return PagedResponse<IReadOnlyCollection<BookingResponseDto>>.Ok(
                bookings.Select(MapToDto).ToList(),
                totalCount,
                page,
                pageSize,
                message);
        }

        private async Task<UserProfile?> GetActivePlayerProfileAsync(int userId)
        {
            return await _dbContext.UserProfiles
                .Include(up => up.User)
                .FirstOrDefaultAsync(up =>
                    up.UserId == userId &&
                    up.User.Role == UserRole.Player &&
                    up.User.Status == UserStatus.Active);
        }

        private async Task<bool> CanAccessBookingAsync(int userId, Booking booking)
        {
            if (booking.UserProfile.UserId == userId || IsVenueOwner(userId, booking))
            {
                return true;
            }

            return await _dbContext.Users.AnyAsync(u => u.Id == userId && u.Role == UserRole.Admin);
        }

        private static bool IsVenueOwner(int userId, Booking booking)
        {
            return booking.Court.Venue.CourtOwnerProfile.UserProfile.UserId == userId;
        }

        private async Task<(bool IsAvailable, string? Reason)> ValidateSlotAsync(
            int courtId,
            DateTimeOffset startAt,
            DateTimeOffset endAt)
        {
            if (courtId <= 0)
            {
                return (false, "Court not found.");
            }

            if (startAt >= endAt)
            {
                return (false, "Start time must be before end time.");
            }

            if (startAt <= DateTimeOffset.Now)
            {
                return (false, "Booking start time must be in the future.");
            }

            if (startAt.Date != endAt.Date)
            {
                return (false, "Booking must start and end on the same day.");
            }

            var court = await _dbContext.Courts
                .Include(c => c.Venue)
                .FirstOrDefaultAsync(c => c.Id == courtId);
            if (court is null)
            {
                return (false, "Court not found.");
            }

            if (court.Status != CourtStatus.Available)
            {
                return (false, "Court is not available.");
            }

            if (court.Venue.Status != VenueStatus.Approved)
            {
                return (false, "Venue is not approved for booking.");
            }

            var hasBookingOverlap = await _dbContext.Bookings.AnyAsync(b =>
                b.CourtId == courtId &&
                (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed) &&
                b.StartAt < endAt &&
                b.EndAt > startAt);
            if (hasBookingOverlap)
            {
                return (false, "Court already has an active booking in this time range.");
            }

            var hasMatchOverlap = await _dbContext.Matches.AnyAsync(m =>
                m.CourtId == courtId &&
                (m.Status == MatchStatus.Open || m.Status == MatchStatus.Full) &&
                m.StartAt < endAt &&
                m.EndAt > startAt);
            if (hasMatchOverlap)
            {
                return (false, "Court already has an active match in this time range.");
            }

            var hasScheduleOverlap = await _dbContext.CourtSchedules.AnyAsync(s =>
                s.CourtId == courtId &&
                s.StartAt < endAt &&
                s.EndAt > startAt);
            if (hasScheduleOverlap)
            {
                return (false, "Court is blocked by a schedule in this time range.");
            }

            return (true, null);
        }

        private async Task<(bool Success, decimal TotalPrice, string Message)> CalculatePriceAsync(
            int courtId,
            DateTimeOffset startAt,
            DateTimeOffset endAt)
        {
            var dayOfWeek = ToPricingDayOfWeek(startAt);
            var startTime = startAt.TimeOfDay;
            var endTime = endAt.TimeOfDay;
            var bookingDate = startAt.Date;

            var rules = await _dbContext.PricingRules
                .Where(r =>
                    r.CourtId == courtId &&
                    r.DayOfWeek == dayOfWeek &&
                    r.EffectiveFrom <= bookingDate &&
                    (r.EffectiveTo == null || r.EffectiveTo >= bookingDate) &&
                    r.StartTime < endTime &&
                    r.EndTime > startTime)
                .ToListAsync();

            var cursor = startTime;
            var totalPrice = 0m;
            while (cursor < endTime)
            {
                var rule = rules
                    .Where(r => r.StartTime <= cursor && r.EndTime > cursor)
                    .OrderByDescending(r => r.EffectiveFrom)
                    .ThenByDescending(r => r.StartTime)
                    .FirstOrDefault();

                if (rule is null)
                {
                    return (false, 0m, "No pricing rule covers this booking time.");
                }

                var segmentEnd = rule.EndTime < endTime ? rule.EndTime : endTime;
                var hours = (decimal)(segmentEnd - cursor).TotalHours;
                totalPrice += hours * rule.PricePerHour;
                cursor = segmentEnd;
            }

            totalPrice = Math.Round(totalPrice, 2);
            if (totalPrice <= 0)
            {
                return (false, 0m, "Calculated booking price is invalid.");
            }

            return (true, totalPrice, "Price calculated successfully.");
        }

        private async Task<ApiResponse<BookingResponseDto>> ChangeStatusAsync(
            Booking booking,
            BookingStatus newStatus,
            int changedByUserId,
            string? reason,
            string successMessage)
        {
            var oldStatus = booking.Status;
            booking.Status = newStatus;
            booking.UpdatedAt = DateTimeOffset.Now;

            _dbContext.BookingStatusHistories.Add(new BookingStatusHistory
            {
                BookingId = booking.Id,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                ChangedByUserId = changedByUserId,
                Reason = Normalize(reason),
                CreatedAt = DateTimeOffset.Now
            });

            await _dbContext.SaveChangesAsync();
            return ApiResponse<BookingResponseDto>.Ok(MapToDto(booking), successMessage);
        }

        private static BookingResponseDto MapToDto(Booking booking)
        {
            return new BookingResponseDto
            {
                Id = booking.Id,
                UserProfileId = booking.UserProfileId,
                PlayerName = booking.UserProfile.FullName,
                CourtId = booking.CourtId,
                CourtName = booking.Court.Name,
                VenueId = booking.Court.VenueId,
                VenueName = booking.Court.Venue.Name,
                StartAt = booking.StartAt,
                EndAt = booking.EndAt,
                TotalPrice = booking.TotalPrice,
                PlatformFee = booking.PlatformFee,
                OwnerEarnings = booking.OwnerEarnings,
                Status = booking.Status.ToString(),
                Note = booking.Note,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt
            };
        }

        private static int ToPricingDayOfWeek(DateTimeOffset dateTime)
        {
            return dateTime.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)dateTime.DayOfWeek;
        }

        private static string? Normalize(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
