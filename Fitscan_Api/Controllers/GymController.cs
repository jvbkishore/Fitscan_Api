using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fitscan_Api.Models;
using Fitscan.API.Data;
using Fitscan_Api.DTOs;
using Fitscan_Api.Helpers;
using System.Net.WebSockets;

namespace Fitscan_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GymController : ControllerBase
    {
        private readonly FitscanDbContext _context;

        public GymController(FitscanDbContext context)
        {
            _context = context;
        }

        // POST: api/GymDetails
        [HttpPost]
        public async Task<ActionResult<GymDetails>> AddGym(GymDetailsDto gymData)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            GymDetails gymDetails = new GymDetails
            {
                Id = Guid.NewGuid(),
                Code = gymData.Code,
                Name = gymData.Name,
                Phonenumber = gymData.Phonenumber,
                OwnerName = gymData.OwnerName,
                OwnerEmail = gymData.Owneremail,
                Joiningdate = gymData.Joiningdate == default ? DateTime.UtcNow : gymData.Joiningdate,
                Longitude = gymData.Longitude,
                Latitude = gymData.Latitude,
                LogoUrl = gymData.LogoUrl,
                Active = gymData.Active
            };



            _context.GymDetails.Add(gymDetails);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGymById), new { id = gymDetails.Id }, gymDetails);
        }

        // GET: api/GymDetails/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GymDetails>> GetGymById(Guid id)
        {
            var gym = await _context.GymDetails.FindAsync(id);

            if (gym == null)
                return NotFound();

            return gym;
        }

        // Optional: GET all gyms
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<GymDetails>>> GetAllGyms()
        //{
        //    return await _context.GymDetails.ToListAsync();
        //}

        // Optional: PUT (Update)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGym(Guid id, GymDetails gymDetails)
        {
            if (id != gymDetails.Id)
                return BadRequest();

            _context.Entry(gymDetails).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.GymDetails.Any(e => e.Id == id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        // Optional: DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGym(Guid id)
        {
            var gym = await _context.GymDetails.FindAsync(id);
            if (gym == null)
                return NotFound();

            _context.GymDetails.Remove(gym);
            await _context.SaveChangesAsync();

            return NoContent();
        }





        [HttpPost("adduser")]
        public async Task<IActionResult> AddGymUser([FromBody] GymUserDto userDto)
        {
            try
            {
                // Check if the gym exists by code
                var gym = await _context.GymDetails
                                        .FirstOrDefaultAsync(g => g.Code == userDto.Gymcode);

                if (gym == null)
                {
                    return NotFound($"Gym with code '{userDto.Gymcode}' not found.");
                }
                var newUser = new GymUsers
                {
                    Username = userDto.Username,
                    Gymcode = userDto.Gymcode,
                    Userplanname = userDto.Userplanname,
                    UserPlanJoiningDate = DateFormatHelper.ConvertToUtc(userDto.UserPlanJoiningDate) ?? DateTime.UtcNow, // Use current date if null
                    UserPlanExpiryDate = DateFormatHelper.ConvertToUtc(userDto.UserPlanExpiryDate),
                    Traineremail = userDto.Traineremail,
                    Active = userDto.Active
                };

                _context.GymUsers.Add(newUser);
                await _context.SaveChangesAsync();
                return Ok(new { message = "User added to gym successfully", userId = newUser.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");

            }
        }


        [HttpGet("gettrainers")]
        public async Task<ActionResult<IEnumerable<object>>> GetTrainers()
        {
            var trainers = await _context.ApplicationUsers
                                         .Where(u => u.Role == "Trainer")
                                         .Select(u => new
                                         {
                                             u.Id,
                                             u.FirstName,
                                             u.LastName,
                                             u.Email
                                         })
                                         .ToListAsync();

            return Ok(trainers);
        }





        [HttpPost("dashboard")]
        public async Task<IActionResult> GetGymDashboardData([FromBody] Owneremail email)
        {

            var today = DateTime.UtcNow.Date;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var expiryDate = today.AddDays(7);

            var previousMonthDate = DateTime.UtcNow.AddMonths(-1);
            var previousMonth = previousMonthDate.Month;
            var previousYear = previousMonthDate.Year;

            var currentMonth = today.Month;
            var currentYear = today.Year;
            var normalizedEmail = email.Email.ToLower();

            // 1. Get GymCode
            var gymcode = await _context.GymDetails
                .Where(g => g.OwnerEmail.ToLower() == normalizedEmail)
                .Select(c => c.Code)
                .FirstOrDefaultAsync();

            if (gymcode == null)
                return NotFound("Gym not found for the provided owner name.");



            var allGymUsers = await (
                from gymUser in _context.GymUsers
                join appUser in _context.ApplicationUsers
                on gymUser.Username equals appUser.UserName
                where gymUser.Gymcode == gymcode && gymUser.Active
                select new
                {
                    gymUser.Username,
                    gymUser.UserPlanExpiryDate,
                    gymUser.Userplanname,
                    gymUser.UserPlanJoiningDate,
                    appUser.FirstName,
                    appUser.LastName
                }).ToListAsync();

            var expiringSoon = allGymUsers
                .Where(u => u.UserPlanExpiryDate.HasValue &&
                            u.UserPlanExpiryDate.Value.Date >= today &&
                            u.UserPlanExpiryDate.Value.Date <= expiryDate)
                .ToList();

            // 3. Get today's distinct check-ins
            var activeToday = await _context.CheckInDetails
                .Where(c => c.Gymcode == gymcode &&
                            c.Checkintime.HasValue &&
                            c.Checkintime.Value.Date == today)
                .Select(c => c.Username)
                .Distinct()
                .CountAsync();

            // 4. Get payments
            var allPayments = await _context.PaymentDetails
                .Where(p => p.Gymcode == gymcode && p.PaymentStatus == "Success")
                .ToListAsync();



            var totalActiveUsers = allGymUsers.Count;

            double checkInPercentage = totalActiveUsers > 0
                ? (double)activeToday / totalActiveUsers * 100
                : 0;

            // 6. New member growth
            var currentMonthCount = allGymUsers
                .Count(u => u.UserPlanJoiningDate.HasValue &&
                            u.UserPlanJoiningDate.Value.Month == currentMonth &&
                            u.UserPlanJoiningDate.Value.Year == currentYear);

            var previousMonthCount = allGymUsers
                .Count(u => u.UserPlanJoiningDate.HasValue &&
                            u.UserPlanJoiningDate.Value.Month == previousMonth &&
                            u.UserPlanJoiningDate.Value.Year == previousYear);

            double newThisMonth = 0;
            if (previousMonthCount == 0 && currentMonthCount > 0)
                newThisMonth = 100;
            else if (previousMonthCount > 0)
                newThisMonth = ((double)(currentMonthCount - previousMonthCount) / previousMonthCount) * 100;

            // 7. Revenue comparison
            var currentMonthRevenue = allPayments
                .Where(p => p.PaidOn.Month == currentMonth && p.PaidOn.Year == currentYear)
                .Sum(p => (decimal?)p.Amount) ?? 0;

            var previousMonthRevenue = allPayments
                .Where(p => p.PaidOn.Month == previousMonth && p.PaidOn.Year == previousYear)
                .Sum(p => (decimal?)p.Amount) ?? 0;

            double revenueGrowth = 0;
            if (previousMonthRevenue == 0 && currentMonthRevenue > 0)
                revenueGrowth = 100;
            else if (previousMonthRevenue > 0)
                revenueGrowth = (double)(currentMonthRevenue - previousMonthRevenue) / (double)previousMonthRevenue * 100;

            // 8. Total revenue for current month (date based)
            var monthlyRevenue = allPayments
                .Where(p => p.PaidOn >= startOfMonth && p.PaidOn <= today)
                .Sum(p => (float?)p.Amount) ?? 0;

            return Ok(new
            {
                gymcode,
                totalActiveUsers,
                //allGymUsers,
                checkInPercentage,
                activeToday,
                monthlyRevenue,
                newThisMonth,
                revenueGrowth,
                //expiringIn7Days = expiringSoon,
                expiringIn7DaysCount = expiringSoon.Count

            });
        }



        [HttpPost("Gymuserdata")]
        public async Task<IActionResult> GetGymuserdata(string gymcode)
        {
            var today = DateTime.UtcNow.Date;
            var expiryDate = today.AddDays(7);

            var allGymUsers = await (
                from gymUser in _context.GymUsers
                join appUser in _context.ApplicationUsers
                on gymUser.Username equals appUser.UserName
                where gymUser.Gymcode == gymcode && gymUser.Active
                select new
                {
                    gymUser.Username,
                    gymUser.UserPlanExpiryDate,
                    gymUser.Userplanname,
                    gymUser.UserPlanJoiningDate,
                    appUser.FirstName,
                    appUser.LastName
                }).ToListAsync();

            var expiringSoon = allGymUsers
                .Where(u => u.UserPlanExpiryDate.HasValue &&
                            u.UserPlanExpiryDate.Value.Date >= today &&
                            u.UserPlanExpiryDate.Value.Date <= expiryDate)
                .ToList();

            return Ok(new { allGymUsers, expiringSoon });
        }



        [HttpPost("gymdetails")]
        public async Task<IActionResult> GetGymDetails([FromBody] string gymCode)
        {
            var gym = await _context.GymDetails.FirstOrDefaultAsync(g => g.Code == gymCode);

            if (gym == null)
                return NotFound("Gym not found");

            return Ok(gym);
        }

        [HttpPost("gymdetailsupdate")]
        public async Task<IActionResult> UpdateGymDetails([FromBody] GymDetails updatedGym)
        {
            var existingGym = await _context.GymDetails.FirstOrDefaultAsync(g => g.Code == updatedGym.Code);
            if (existingGym == null)
                return NotFound("Gym not found");

            existingGym.Name = updatedGym.Name;
            existingGym.OwnerName = updatedGym.OwnerName;
            existingGym.OwnerEmail = updatedGym.OwnerEmail;
            existingGym.Phonenumber = updatedGym.Phonenumber;
            existingGym.LogoUrl = updatedGym.LogoUrl;
            existingGym.Latitude = updatedGym.Latitude;
            existingGym.Longitude = updatedGym.Longitude;
            existingGym.Joiningdate = updatedGym.Joiningdate;
            existingGym.Active = updatedGym.Active;

            await _context.SaveChangesAsync();
            return Ok("Updated successfully");
        }

      





    }
}

public class Owneremail
{
    public required string Email { get; set; }
}

public class GymRequest
{
    public string GymCode { get; set; }
}
