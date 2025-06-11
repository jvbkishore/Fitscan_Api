using Fitscan.API.Data;
using Fitscan_Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fitscan_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReferralController : Controller
    {
        private readonly FitscanDbContext _context;

        public ReferralController(FitscanDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateReferral([FromBody] CreateReferralDto dto)
        {
            var referralCode = Guid.NewGuid().ToString().Substring(0, 8);
            var referralLink = $"https://fitscan.app/ref/{referralCode}";

            var referral = new ReferralInfo
            {
                ReferralCode = referralCode,
                ReferralLink = referralLink,
                CreatedByUserId = dto.CreatedByUserId,
                GymId = dto.GymId,
            };

            _context.ReferralInfo.Add(referral);
            await _context.SaveChangesAsync();

            return Ok(referral);
        }

        [HttpPost("use")]
        public async Task<IActionResult> UseReferral([FromBody] UseReferralDto dto)
        {
            var referral = await _context.ReferralInfo
                .FirstOrDefaultAsync(r => r.ReferralCode == dto.ReferralCode);

            if (referral == null)
                return NotFound("Invalid referral code");

            var status = new ReferralStatus()
            {
                FriendName = dto.FriendName,
                JoinedDate = DateTime.UtcNow,
                Reward = dto.Reward,
                ReferralInfoId = referral.Id,
                ReferralInfo = referral, // Fix: Set the required 'ReferralInfo' property
                UsedByUserId = dto.UsedByUserId
            };

            _context.ReferralStatus.Add(status);
            await _context.SaveChangesAsync();

            return Ok(status);
        }

        [HttpGet("{referralCode}/status")]
        public async Task<IActionResult> GetReferralStatus(string referralCode)
        {
            var referral = await _context.ReferralInfo
                .Include(r => r.ReferralStatuses)
                .FirstOrDefaultAsync(r => r.ReferralCode == referralCode);

            if (referral == null)
                return NotFound("Referral not found");

            return Ok(referral);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetReferralsByUser(int userId)
        {
            var referrals = await _context.ReferralInfo
                .Where(r => r.CreatedByUserId == userId)
                .Include(r => r.ReferralStatuses)
                .ToListAsync();

            return Ok(referrals);
        }

        [HttpGet("gym/{gymId}")]
        public async Task<IActionResult> GetReferralsByGym(int gymId)
        {
            var referrals = await _context.ReferralInfo
                .Where(r => r.GymId == gymId)
                .Include(r => r.ReferralStatuses)
                .ToListAsync();

            return Ok(referrals);
        }
    }

}
