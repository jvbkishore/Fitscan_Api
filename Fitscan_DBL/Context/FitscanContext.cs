using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Fitscan.Data.Entities;
using Fitscan_DBL.Entities;

namespace Fitscan_DBL.Context
{
    public class FitscanContext : DbContext
    {

        public FitscanContext(DbContextOptions<FitscanContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<WeightRecord> WeightRecords => Set<WeightRecord>();
        public DbSet<GymAccess> GymAccesses => Set<GymAccess>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // PostgreSQL-specific configurations
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<WeightRecord>()
                .Property(w => w.Weight)
                .HasColumnType("decimal(5,2)");
        }

    }
}
