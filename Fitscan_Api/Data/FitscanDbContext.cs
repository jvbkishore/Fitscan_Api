using Fitscan.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Fitscan_Api.Models;

namespace Fitscan.API.Data
{
    public class FitscanDbContext : IdentityDbContext<ApplicationUser>
    {
        public FitscanDbContext(DbContextOptions<FitscanDbContext> options) : base(options) { }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<GymDetails> GymDetails { get; set; }

        public DbSet<CheckInDetails> CheckInDetails { get; set; }

        public DbSet<GymUsers> GymUsers { get; set; }

        public DbSet<FitnessTrackingDetails> FitnessTrackingDetails { get; set; }
        public DbSet<QrCode> QrCodes { get; set; }

        public DbSet<PlanDetails> PlanDetails { get; set; }

        public DbSet<PaymentDetails> PaymentDetails { get; set; }

        public DbSet<ReferralInfo> ReferralInfo { get; set; }
        public DbSet<ReferralStatus> ReferralStatus { get; set; }

        public DbSet<ConsistencyScore> ConsistencyScores { get; set; }




    }
}