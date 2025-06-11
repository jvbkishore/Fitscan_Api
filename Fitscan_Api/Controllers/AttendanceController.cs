using Fitscan.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fitscan_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : Controller
    {
        private readonly FitscanDbContext _context;
        public AttendanceController(FitscanDbContext context)
        {
            _context = context;
        }


        [HttpPost("today")]
        public async Task<IActionResult> GetTodaysAttendance([FromBody] string gymCode)
        {
            var today = DateTime.UtcNow.Date;

            var checkins = await _context.CheckInDetails
                .Where(c => c.Gymcode == gymCode && c.Checkintime != null && c.Checkintime.Value.Date == today)
                .Join(_context.Users,
                      checkin => checkin.Username,
                      user => user.UserName,
                      (checkin, user) => new
                      {
                          Id = checkin.Id,
                          Name = user.FirstName,
                          CheckIn = checkin.Checkintime,
                          CheckOut = checkin.Checkouttime
                      })
                .ToListAsync();

            return Ok(checkins);
        }


        [HttpPost("weekly-summary")]
        public async Task<IActionResult> GetWeeklyAttendanceSummary([FromBody] string gymCode)
        {
            var startOfWeek = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.Date.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(6);

            var data = await _context.CheckInDetails
                .Where(c => c.Gymcode == gymCode && c.Checkintime >= startOfWeek && c.Checkintime <= endOfWeek)
                .ToListAsync();

            var grouped = data
                .GroupBy(c => c.Checkintime!.Value.Date)
                .Select(g => new
                {
                    Date = g.Key.DayOfWeek.ToString().Substring(0, 3),
                    Present = g.Count(),
                    Absent = 0 // Optional: depends if you're tracking registered users vs attendees
                })
                .ToList();

            return Ok(grouped);
        }


    }
}
