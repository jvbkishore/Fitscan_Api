using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using Fitscan.API.Data;
using Fitscan.API.Models;
using Fitscan_Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Fitscan_Api.Controllers
{
    [ApiController]
    [Route("api/qrcode")]
    public class QrController : ControllerBase
    {
        private readonly FitscanDbContext _context;
        private readonly IConfiguration _configuration;

        public QrController(FitscanDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        
        [Authorize]
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateQrCode([FromBody] QrRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.username))
                    return BadRequest("Username is required.");

                var tokenId = Guid.NewGuid();
                var expiresAt = DateTime.UtcNow.AddMinutes(1);

                var orgId = _context.GymUsers
                   .Where(c => c.Username.ToLower() == request.username.ToLower() &&
                               c.UserPlanExpiryDate >= DateTime.UtcNow &&
                               c.UserPlanJoiningDate <= DateTime.UtcNow &&
                               c.Active)
                   .Select(g => g.Gymcode)
                   .FirstOrDefault();

                if (string.IsNullOrEmpty(orgId))
                {
                    return NotFound(new { success = false, message = "Active membership not found or gymcode missing for this user." });
                }

                var qrEntry = new QrCode
                {
                    Id = Guid.NewGuid(),
                    TokenId = tokenId,
                    Username = request.username,
                    OrgID = orgId,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = expiresAt,
                    IsUsed = false
                };

                _context.QrCodes.Add(qrEntry);
                await _context.SaveChangesAsync();

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["QRJwt:Key"]);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim("qrusername", request.username),
                        new Claim("qrorgid", orgId.ToString()),
                        new Claim(JwtRegisteredClaimNames.Jti, tokenId.ToString())
                    }),
                    Expires = expiresAt,
                    Issuer = _configuration["QRJwt:Issuer"],
                    Audience = _configuration["QRJwt:Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);

                return Ok(new { success = true, jwtToken });
            }
            catch (Exception ex)
            {
                // Log the error if needed: _logger.LogError(ex, "QR Code generation failed.");
                return StatusCode(500, new { success = false, message = "An error occurred while generating the QR code.", error = ex.Message });
            }
        }



        [Authorize]
        [HttpPost("scan")]

        public async Task<IActionResult> ValidateQrToken([FromBody] string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["QRJwt:Key"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["QRJwt:Issuer"],
                    ValidAudience = _configuration["QRJwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var tokenIdStr = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

                if (!Guid.TryParse(tokenIdStr, out Guid tokenId))
                    return BadRequest("Invalid token ID format.");

                var qr = await _context.QrCodes.FirstOrDefaultAsync(q => q.TokenId == tokenId);
                // Replace the problematic code with the following:
                var gymcode = await _context.GymUsers
                    .Where(g => g.Username != null && qr != null && g.Username.ToLower() == qr.Username.ToLower())
                    .Select(g => g.Gymcode)
                    .FirstOrDefaultAsync();

                if (qr == null || qr.IsUsed || qr.ExpiresAt < DateTime.UtcNow || gymcode != qr.OrgID)
                    return BadRequest("QR code is invalid or expired.");

                qr.IsUsed = true;
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Check-in successful", username = qr.Username, orgId = qr.OrgID });
            }
            catch (SecurityTokenExpiredException)
            {
                return BadRequest("Token has expired.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Token validation failed: {ex.Message}");
            }
        }





        
        [HttpPost("getcheckindetails")]
        public async Task<IActionResult> GetCheckInDetails([FromBody] string gymcode)
        {
           
            DateTime today = DateTime.UtcNow.Date; // Use DateTime.Today if you're not working in UTC

            var checkInsToday = _context.CheckInDetails
                .Where(c => c.Gymcode == gymcode && c.Checkintime.HasValue && c.Checkintime.Value.Date == today)
                .ToList();

            var checkinGroups = new[]
            {
                new { Label = "6-8 AM", Start = 6, End = 8 },
                new { Label = "8-10 AM", Start = 8, End = 10 },
                new { Label = "10-12 PM", Start = 10, End = 12 },
                new { Label = "12-2 PM", Start = 12, End = 14 },
                new { Label = "2-4 PM", Start = 14, End = 16 },
                new { Label = "4-6 PM", Start = 16, End = 18 },
                new { Label = "6-8 PM", Start = 18, End = 20 },
            };


            var checkinsBySlot = checkinGroups.Select(g =>
                    new CheckinTimeSlotDto
                    {
                        Hour = g.Label,
                        Count = checkInsToday.Count(c =>c.Checkintime.Value.Hour >= g.Start && c.Checkintime.Value.Hour < g.End)
                    }).ToList();

            return Ok(checkinsBySlot);
        }

     
    }
}

