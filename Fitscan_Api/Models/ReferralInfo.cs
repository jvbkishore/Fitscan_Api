using Fitscan_Api.Models;

public class ReferralInfo
{
    public int Id { get; set; }
    public required string ReferralCode { get; set; }
    public required string ReferralLink { get; set; }

    public int CreatedByUserId { get; set; }
    public int GymId { get; set; }

    public virtual List<ReferralStatus> ReferralStatuses { get; set; } = new();
}
