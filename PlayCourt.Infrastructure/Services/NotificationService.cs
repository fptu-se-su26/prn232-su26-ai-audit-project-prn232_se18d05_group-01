using Microsoft.EntityFrameworkCore;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Notifications;
using PlayCourt.Application.Interfaces;
using PlayCourt.Domain.Entities;
using PlayCourt.Domain.Enums;
using PlayCourt.Infrastructure.Data;

namespace PlayCourt.Infrastructure.Services
{
    public sealed class NotificationService : INotificationService
    {
        private readonly PlayCourtDbContext _dbContext;

        public NotificationService(PlayCourtDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResponse<IReadOnlyCollection<NotificationDto>>> GetMyNotificationsAsync(
            int userId,
            NotificationQueryDto query)
        {
            var pageIndex = Math.Max(query.PageIndex, 1);
            var pageSize = Math.Clamp(query.PageSize, 1, 100);

            var baseQuery = _dbContext.Notifications
                .AsNoTracking()
                .Where(n => n.UserId == userId);

            if (query.IsRead.HasValue)
            {
                baseQuery = baseQuery.Where(n => n.IsRead == query.IsRead.Value);
            }

            if (query.Type.HasValue)
            {
                baseQuery = baseQuery.Where(n => n.Type == query.Type.Value);
            }

            var totalCount = await baseQuery.CountAsync();

            var notifications = await baseQuery
                .OrderByDescending(n => n.CreatedAt)
                .ThenByDescending(n => n.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = notifications.Select(MapToDto).ToList();

            return PagedResponse<IReadOnlyCollection<NotificationDto>>.Ok(
                data,
                totalCount,
                pageIndex,
                pageSize,
                "Notifications retrieved successfully.");
        }

        public async Task<ApiResponse<UnreadNotificationCountDto>> GetUnreadCountAsync(int userId)
        {
            var count = await _dbContext.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);

            return ApiResponse<UnreadNotificationCountDto>.Ok(
                new UnreadNotificationCountDto { Count = count },
                "Unread count retrieved successfully.");
        }

        public async Task<ApiResponse<NotificationDto>> MarkAsReadAsync(int userId, int notificationId)
        {
            var notification = await _dbContext.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification is null)
            {
                return ApiResponse<NotificationDto>.Fail("Notification not found.");
            }

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                await _dbContext.SaveChangesAsync();
            }

            return ApiResponse<NotificationDto>.Ok(MapToDto(notification), "Notification marked as read.");
        }

        public async Task<ApiResponse<MarkAllNotificationsReadResponseDto>> MarkAllAsReadAsync(int userId)
        {
            var unreadNotifications = await _dbContext.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            var count = unreadNotifications.Count;
            if (count > 0)
            {
                foreach (var notification in unreadNotifications)
                {
                    notification.IsRead = true;
                }

                await _dbContext.SaveChangesAsync();
            }

            return ApiResponse<MarkAllNotificationsReadResponseDto>.Ok(
                new MarkAllNotificationsReadResponseDto { UpdatedCount = count },
                "All notifications marked as read.");
        }

        public async Task<ApiResponse<object>> DeleteAsync(int userId, int notificationId)
        {
            var notification = await _dbContext.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification is null)
            {
                return ApiResponse<object>.Fail("Notification not found.");
            }

            _dbContext.Notifications.Remove(notification);
            await _dbContext.SaveChangesAsync();

            return ApiResponse<object>.Ok(null, "Notification deleted successfully.");
        }

        private static NotificationDto MapToDto(Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                Title = notification.Title,
                Content = notification.Content,
                Type = notification.Type.ToString(),
                ReferenceType = notification.ReferenceType?.ToString(),
                ReferenceId = notification.ReferenceId,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };
        }
    }
}
