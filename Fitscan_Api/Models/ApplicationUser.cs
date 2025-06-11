using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Fitscan.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        
        public required string FirstName { get; set; } = string.Empty;
        public required string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public DateTime? JoiningDate { get; set; } = null; // Changed DateTime to DateTime? to allow null values
        public string Gender { get; set; } = string.Empty;

        //  public DateTime? JoiningDate { get; set; } = null;

        public string? PhotoUrl { get; set; }

        public bool IsActive { get; set; } = true;

    }
}
