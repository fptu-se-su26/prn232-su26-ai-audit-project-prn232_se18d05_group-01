using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Notifications;

namespace PlayCourt.Application.Interfaces
{
    public interface INotificationService
    {
        Task<PagedResponse<IReadOnlyCollection<NotificationDto>>> GetMyNotificationsAsync(
            int userId,
            NotificationQueryDto query);

        Task<ApiResponse<UnreadNotificationCountDto>> GetUnreadCountAsync(
            int userId);

        Task<ApiResponse<NotificationDto>> MarkAsReadAsync(
            int userId,
            int notificationId);

        Task<ApiResponse<MarkAllNotificationsReadResponseDto>> MarkAllAsReadAsync(
            int userId);

        Task<ApiResponse<object>> DeleteAsync(
            int userId,
            int notificationId);
    }
}
