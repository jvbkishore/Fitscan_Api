using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Fitscan_Api.Models
{
    public class PaymentDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? PaymentId { get; set; }
        public required string UserId { get; set; }
        public required string PlanId { get; set; }
        public required float Amount { get; set; }

        public required string Gymcode { get; set; }

        public required string PaymentStatus { get; set; }
        public required string PaymentMethod { get; set; }

        // Fix: Replace DateTimeUtc with DateTime
        public required DateTime PaidOn { get; set; }
        public string? TransactionId { get; set; }

        public string? Notes { get; set; }
        public required DateTime UpdatedOn { get; set; }

        public required string UpdatedBy { get; set; }

    }
}
