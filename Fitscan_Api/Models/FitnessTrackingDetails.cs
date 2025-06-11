using System.ComponentModel.DataAnnotations;

namespace Fitscan_Api.Models
{
    public class FitnessTrackingDetails
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        
        public DateTime? RecordedOn { get; set; }

        public string? Enteredby { get; set; }
        public bool Active { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Range(0, 500)]
        public double Weight { get; set; }

        public double Neck { get; set; }
        public double Shoulders { get; set; }
        public double Chest { get; set; }
        public double Biceps { get; set; }
        public double Belly { get; set; }
        public double Waist { get; set; }
        public double Hip { get; set; }
        public double Calf { get; set; }
        public double Thighs { get; set; }


    }
}
