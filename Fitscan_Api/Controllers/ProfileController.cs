using Fitscan.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Fitscan_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Optional: Remove this if you don’t want to enforce auth yet
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: api/profile/{username}
        [HttpPost("userprofile")]
        public async Task<IActionResult> GetProfile([FromBody]string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound(new { success = false, message = "User not found" });

            return Ok(new
            {
                FullName = user.FirstName + " " + user.LastName, // Fixed CS0746 by assigning a named property
                user.Email,
                user.PhoneNumber,
                user.PhotoUrl,
            });
        }

        // PUT: api/profile/{username}
        public class UpdateUserProfileRequest
        {
            public string username { get; set; } = string.Empty;
            public ApplicationUser updatedUser { get; set; } = default!;
        }

        [HttpPut("updateuserprofile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.username);
            if (user == null)
                return NotFound(new { success = false, message = "User not found" });

            var updatedUser = request.updatedUser;

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Gender = updatedUser.Gender;
            user.JoiningDate = updatedUser.JoiningDate;
            user.PhotoUrl = updatedUser.PhotoUrl;
            user.PhoneNumber = updatedUser.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new { success = false, errors = result.Errors });

            return Ok(new { success = true, message = "Profile updated successfully" });
        }

    }
}
