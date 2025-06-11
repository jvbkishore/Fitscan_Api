namespace Fitscan_Api.DTOs
{
    public class CreatePaymentDto
    {
        public string? PaymentId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string PlanId { get; set; } = string.Empty;
        public float Amount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime PaidOn { get; set; }

        public required string Gymcode { get; set; }
        public string? TransactionId { get; set; }
        public string? Notes { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }

    public class UpdatePaymentDto : CreatePaymentDto
    {
        public int Id { get; set; }
    }

}
