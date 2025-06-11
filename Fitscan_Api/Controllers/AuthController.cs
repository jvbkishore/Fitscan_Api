using Fitscan.API.DTOs;
using Fitscan.API.Models;
using Fitscan.API.Services;
using Fitscan_Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Fitscan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly TokenService _tokenService;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, TokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser
            {

                Email = dto.Email,
                UserName = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Gender = dto.Gender,
                Role = dto.Role,
                PhotoUrl = dto.PhotoUrl,
                JoiningDate = dto.JoiningDate?.ToUniversalTime()

            };
            
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, dto.Role);
            return Ok(new
            {
                message = "Registration successful"
                //userId = user.Id
            });
        }

        
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.email);
            if (user == null) return Unauthorized("Invalid username");

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded) return Unauthorized("Invalid password");

            var token = _tokenService.CreateToken(user);
            
            return Ok(new
            {
                token = token,
                user = new
                {
                    username = user.Email,
                    role = user.Role
                },
                message = "Login successful"
            });
        }

        [Authorize]
        [HttpPost("upload-photo")]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var user = await _userManager.GetUserAsync(User);
            var uploads = Path.Combine("wwwroot", "uploads");
            Directory.CreateDirectory(uploads);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploads, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            user.PhotoUrl = $"/uploads/{fileName}";
            await _userManager.UpdateAsync(user);

            return Ok(new { user.PhotoUrl });
        }




        [Authorize]
        [HttpDelete("delete-photo")]
        public async Task<IActionResult> DeletePhoto([FromBody] DeletePhotoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PhotoUrl))
                return BadRequest("Photo URL is required.");

            var filePath = Path.Combine("wwwroot", request.PhotoUrl.TrimStart('/'));

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return Ok();
        }

        public class DeletePhotoRequest
        {
            public string PhotoUrl { get; set; }
        }

    }
}
