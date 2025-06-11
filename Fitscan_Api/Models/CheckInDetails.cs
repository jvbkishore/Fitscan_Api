using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Fitscan.API.Models;

namespace Fitscan_Api.Models
{
    public class CheckInDetails
    {


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }

        public string? Username { get; set; }
        public string? Gymcode { get; set; }
        public DateTime? Checkintime { get; set; }
        public DateTime? Checkouttime { get; set; }
        public bool Active { get; set; } = true;
        public required string  QrCodeId { get; set; }  // <-- Foreign Key


    }
}
