using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fitscan_Api.Models
{
    public class GymUsers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Username { get; set; }

        public string? Gymcode { get; set; }

        public string? Userplanname { get; set; }

        public DateTime? UserPlanJoiningDate { get; set; }

        public DateTime? UserPlanExpiryDate { get; set; }

        public string? Traineremail { get; set; }

        public bool Active { get; set; } 


    }
}
