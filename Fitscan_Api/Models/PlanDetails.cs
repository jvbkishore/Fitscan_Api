using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitscan_Api.Models
{
    public class PlanDetails
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public required string PlanName { get; set; }

        [Required]
        public required string PlanDescription { get; set; }
        
        [Required]
        public required string GymCode { get; set; }
        public required float Amount { get; set; }
        public required string Duration { get; set; } 
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
