using Fitscan.API.Data;
using Fitscan_Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fitscan_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsistencyScoreController : ControllerBase
    {
        private readonly FitscanDbContext _context;

        public ConsistencyScoreController(FitscanDbContext context)
        {
            _context = context;
        }

        // Add new score
        [HttpPost("add")]
        public async Task<IActionResult> AddConsistencyScore([FromBody] ConsistencyScore score)
        {
            var existing = await _context.ConsistencyScores
                .FirstOrDefaultAsync(x => x.Username == score.Username);

            if (existing != null)
                return BadRequest("Consistency score already exists for this user.");

            _context.ConsistencyScores.Add(score);
            await _context.SaveChangesAsync();
            return Ok(score);
        }

        // Update score
        [HttpPut("update/{username}")]
        public async Task<IActionResult> UpdateConsistencyScore(string username, [FromBody] ConsistencyScore updated)
        {
            var existing = await _context.ConsistencyScores.FirstOrDefaultAsync(x => x.Username == username);
            if (existing == null)
                return NotFound("User not found");

            // Update fields
            existing.CurrentStreak = updated.CurrentStreak;
            existing.LongestStreak = updated.LongestStreak;
            existing.WeeklyAverage = updated.WeeklyAverage;
            existing.MonthlyVisits = updated.MonthlyVisits;
            existing.LastVisitDate = updated.LastVisitDate;
            existing.ImprovementPercent = updated.ImprovementPercent;
            existing.ReferralsThisMonth = updated.ReferralsThisMonth;
            existing.TriedNewClass = updated.TriedNewClass;
            existing.JoinedChallenge = updated.JoinedChallenge;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // Get score by username
        [HttpGet("{username}")]
        public async Task<IActionResult> GetConsistencyScore(string username)
        {
            var score = await _context.ConsistencyScores.FirstOrDefaultAsync(x => x.Username == username);
            if (score == null)
                return NotFound("User not found");

            return Ok(new
            {
                score.Username,
                score.CurrentStreak,
                score.LongestStreak,
                score.WeeklyAverage,
                score.MonthlyVisits,
                score.LastVisitDate,
                score.ImprovementPercent,
                score.ReferralsThisMonth,
                score.TriedNewClass,
                score.JoinedChallenge,
                score.Score,
                score.MotivationalMessage
            });
        }

        // Delete score
        [HttpDelete("{username}")]
        public async Task<IActionResult> DeleteConsistencyScore(string username)
        {
            var score = await _context.ConsistencyScores.FirstOrDefaultAsync(x => x.Username == username);
            if (score == null)
                return NotFound("User not found");

            _context.ConsistencyScores.Remove(score);
            await _context.SaveChangesAsync();

            return Ok("Deleted successfully");
        }

        // Get all scores (admin/gym owner)
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var scores = await _context.ConsistencyScores.ToListAsync();
            return Ok(scores.Select(score => new
            {
                score.Username,
                score.Score,
                score.MotivationalMessage
            }));
        }
    }

}
