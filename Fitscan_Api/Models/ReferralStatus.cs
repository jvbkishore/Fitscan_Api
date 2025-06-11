public class ReferralStatus
{
    public int Id { get; set; }
    public required string FriendName { get; set; }
    public DateTime JoinedDate { get; set; }
    public required string Reward { get; set; }

    public int ReferralInfoId { get; set; }
    public virtual required ReferralInfo ReferralInfo { get; set; }

    public int UsedByUserId { get; set; } // To track the user who used the referral
}
