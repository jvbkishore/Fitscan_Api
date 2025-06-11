using System.ComponentModel.DataAnnotations;

namespace Fitscan_Api.Models
{
    public class GymDetails
    {

        [Key]
        public Guid Id { get; set; }

        [Required]
        public required string Code { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        [Length(10, 10)]
        public required string Phonenumber { get; set; }

        [Required]
        public required string OwnerName { get; set; }

        [Required]
        [EmailAddress]
        public string OwnerEmail { get; set; }

        // Coordinates
        [Range(-90, 90)]
        public double? Latitude { get; set; }

        [Range(-180, 180)]
        public double? Longitude { get; set; }

        [Required]
        public DateTime Joiningdate { get; set; }


        public string? LogoUrl { get; set; }

        public bool Active { get; set; } = true;

    }
}
