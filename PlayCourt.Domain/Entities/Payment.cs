using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PlayCourt.Domain.Enums;

namespace PlayCourt.Domain.Entities
{
    // [Table]: map class này vào bảng dbo.Payments.
    // Bảng Payments là ledger/audit giao dịch thanh toán, refund, payout.
    [Table("Payments", Schema = "dbo")]
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        // FK -> dbo.Users(Id). Người liên quan giao dịch.
        [Required]
        public int UserId { get; set; }

        // FK -> dbo.Bookings(Id), nullable vì refund/payout có thể không gắn booking.
        public int? BookingId { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal Amount { get; set; }

        // Cổng thanh toán: VNPay, Momo, ZaloPay...
        [Required]
        [MaxLength(50)]
        public string Provider { get; set; } = default!;

        // Mã giao dịch từ provider.
        [MaxLength(100)]
        public string? TransactionCode { get; set; }

        // Type: 0=BookingPayment, 1=Refund, 2=Payout.
        [Required]
        public PaymentType Type { get; set; }

        // Status: 0=Pending, 1=Success, 2=Failed.
        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public DateTimeOffset? PaidAt { get; set; }

        [Required]
        [MaxLength(3)]
        [Column(TypeName = "char(3)")]
        public string Currency { get; set; } = "VND";

        // Raw callback data từ payment gateway.
        public string? ProviderPayload { get; set; }

        public string? Note { get; set; }

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset? UpdatedAt { get; set; }

        public User User { get; set; } = default!;

        public Booking? Booking { get; set; }
    }
}
