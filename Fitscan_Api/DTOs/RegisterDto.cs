using System.ComponentModel.DataAnnotations;

namespace Fitscan.API.DTOs
{
    public class RegisterDto
    {
        //public required string Username { get; set; }

        [EmailAddress]
        public required string Email { get; set; } = string.Empty;

        [Phone]
        [Length(10,10)]
        public required string PhoneNumber { get; set; } = string.Empty;
        public required string Password { get; set; }
        public string Role { get; set; } = "User";

        public string Gender { get; set; } = string.Empty;
        public required string FirstName { get; set; } = string.Empty;
        public required string LastName { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }

        public DateTime? JoiningDate { get; set; } = null; // Changed DateTime to DateTime? to allow null values
    }
}