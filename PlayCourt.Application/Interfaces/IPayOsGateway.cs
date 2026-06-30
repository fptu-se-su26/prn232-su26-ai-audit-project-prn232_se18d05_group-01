namespace PlayCourt.Application.Interfaces
{
    public interface IPayOsGateway
    {
        Task<PayOsCreatePaymentLinkResult> CreatePaymentLinkAsync(
            PayOsCreatePaymentLinkRequest request,
            CancellationToken cancellationToken = default);

        Task<PayOsPaymentLinkStatusResult> GetPaymentLinkInformationAsync(
            long orderCode,
            CancellationToken cancellationToken = default);

        Task<PayOsWebhookResult> VerifyWebhookAsync(
            string webhookBody,
            CancellationToken cancellationToken = default);
    }

    public sealed record PayOsCreatePaymentLinkRequest(
        long OrderCode,
        long Amount,
        string Description,
        string ReturnUrl,
        string CancelUrl);

    public sealed record PayOsCreatePaymentLinkResult(
        string CheckoutUrl,
        string? PaymentLinkId);

    public sealed record PayOsPaymentLinkStatusResult(
        long OrderCode,
        string Status,
        string? Reference,
        string? PaymentLinkId,
        long AmountPaid,
        string RawPayload);

    public sealed record PayOsWebhookResult(
        long OrderCode,
        long Amount,
        string? Reference,
        string? PaymentLinkId,
        bool Success,
        string RawPayload);
}
