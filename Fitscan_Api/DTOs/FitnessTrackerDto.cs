namespace Fitscan_Api.DTOs
{
    public class CreateFitnessTrackingDto
    {
        public string Username { get; set; } = string.Empty; // User whose measurements are entered
        public string EnteredBy { get; set; } = string.Empty; // Trainer/Owner
        public DateTime Date { get; set; }

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

        public bool Active { get; set; } = true;
    }



    public class UpdateFitnessTrackingDto : CreateFitnessTrackingDto
    {
        public int Id { get; set; }
    }

}
