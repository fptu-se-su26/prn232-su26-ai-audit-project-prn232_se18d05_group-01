using System.Text.Json;
using PlayCourt.Application.Common.Responses;
using PlayCourt.Application.DTOs.Payments;

namespace PlayCourt.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<ApiResponse<CreatePayOsPaymentResponseDto>> CreatePayOsPaymentLinkAsync(int userId, int bookingId);
        Task<ApiResponse<PaymentResponseDto>> SyncPayOsPaymentAsync(int userId, int bookingId);
        Task<ApiResponse<IReadOnlyCollection<PaymentResponseDto>>> GetBookingPaymentsAsync(int userId, int bookingId);
        Task<ApiResponse<PaymentResponseDto>> HandlePayOsWebhookAsync(JsonElement webhookBody);
    }
}
