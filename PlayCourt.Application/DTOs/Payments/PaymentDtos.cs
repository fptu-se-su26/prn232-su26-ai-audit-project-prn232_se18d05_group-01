namespace PlayCourt.Application.DTOs.Payments
{
    public sealed class CreatePayOsPaymentResponseDto
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public long OrderCode { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "VND";
        public string CheckoutUrl { get; set; } = string.Empty;
        public string? PaymentLinkId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
    }

    public sealed class PaymentResponseDto
    {
        public int Id { get; set; }
        public int? BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string? TransactionCode { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTimeOffset? PaidAt { get; set; }
        public string Currency { get; set; } = "VND";
        public string? Note { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
