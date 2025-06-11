using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitscan_DBL.Entities
{
    public  class User
    {

        public int Id { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string Role { get; set; }

        // Navigation properties
        public List<WeightRecord> WeightRecords { get; set; }
        public List<GymAccess> GymAccesses { get; set; }
    }
}
