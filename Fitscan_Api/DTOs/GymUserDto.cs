namespace Fitscan_Api.DTOs
{
    public class GymUserDto
    {
        public string? Username { get; set; }

        public string? Gymcode { get; set; }

        

        public string? Userplanname { get; set; }

        public DateTime? UserPlanJoiningDate { get; set; }

        public DateTime? UserPlanExpiryDate { get; set; }

        public string? Traineremail { get; set; }

        public bool Active { get; set; }
    }
}