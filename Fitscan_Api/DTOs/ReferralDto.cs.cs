namespace Fitscan_Api.DTOs
{
    public class CreateReferralDto
    {
        public int CreatedByUserId { get; set; }
        public int GymId { get; set; }
    }

    public class UseReferralDto
    {
        public string ReferralCode { get; set; } = string.Empty;
        public string FriendName { get; set; } = string.Empty;
        public string Reward { get; set; } = string.Empty;
        public int UsedByUserId { get; set; }
    }

}
