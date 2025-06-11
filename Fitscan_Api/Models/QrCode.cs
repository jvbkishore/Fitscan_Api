using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitscan.API.Models
{
    public class QrCode
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid TokenId { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required]
        public string OrgID { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; }

        public bool IsUsed { get; set; } = false;
    }
}
