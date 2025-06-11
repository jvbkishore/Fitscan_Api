using Fitscan.API.Data;
using Fitscan_Api.DTOs;
using Fitscan_Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Fitscan_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FitnessTrackingController : Controller
    {
        private readonly FitscanDbContext _context;

        public FitnessTrackingController(FitscanDbContext context)
        {
            _context = context;
        }

        [HttpPost("AddFitnessTracking")]
        public async Task<IActionResult> AddFitnessTracking([FromBody] CreateFitnessTrackingDto dto)
        {
            var entry = new FitnessTrackingDetails
            {
                Username = dto.Username,
                Enteredby = dto.EnteredBy,
                Date = DateTime.UtcNow,
                Weight = dto.Weight,
                Neck = dto.Neck,
                Shoulders = dto.Shoulders,
                Chest = dto.Chest,
                Biceps = dto.Biceps,
                Belly = dto.Belly,
                Waist = dto.Waist,
                Hip = dto.Hip,
                Calf = dto.Calf,
                Thighs = dto.Thighs,
                RecordedOn = DateTime.UtcNow,
                Active = dto.Active
            };

            _context.FitnessTrackingDetails.Add(entry);
            await _context.SaveChangesAsync();

            return Ok(entry);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateFitnessTracking([FromBody] UpdateFitnessTrackingDto dto)
        {
            var entry = await _context.FitnessTrackingDetails.FindAsync(dto.Id);
            if (entry == null)
                return NotFound("Tracking entry not found");

            entry.Username = dto.Username;
            entry.Enteredby = dto.EnteredBy;
            entry.Date = dto.Date;
            entry.Weight = dto.Weight;
            entry.Neck = dto.Neck;
            entry.Shoulders = dto.Shoulders;
            entry.Chest = dto.Chest;
            entry.Biceps = dto.Biceps;
            entry.Belly = dto.Belly;
            entry.Waist = dto.Waist;
            entry.Hip = dto.Hip;
            entry.Calf = dto.Calf;
            entry.Thighs = dto.Thighs;
            entry.Active = dto.Active;
            entry.RecordedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(entry);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFitnessTracking(int id)
        {
            var entry = await _context.FitnessTrackingDetails.FindAsync(id);
            if (entry == null)
                return NotFound("Entry not found");

            _context.FitnessTrackingDetails.Remove(entry);
            await _context.SaveChangesAsync();

            return Ok("Deleted successfully");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFitnessTrackingById(int id)
        {
            var entry = await _context.FitnessTrackingDetails.FindAsync(id);
            if (entry == null)
                return NotFound("Entry not found");

            return Ok(entry);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllFitnessTracking()
        {
            var list = await _context.FitnessTrackingDetails.ToListAsync();
            return Ok(list);
        }

        [HttpPost("records")]
        public async Task<IActionResult> GetTrackingByUsername([FromBody] string username)
        {
            
            var list = await (from fitnessdata in _context.FitnessTrackingDetails
                              where fitnessdata.Username == username
                              select new
                              {
                                  date = fitnessdata.Date.ToString("yyyy-MM-dd"),
                                  fitnessdata.Weight,
                                  fitnessdata.Neck,
                                  fitnessdata.Shoulders,
                                  fitnessdata.Chest,
                                  fitnessdata.Biceps,
                                  fitnessdata.Belly,
                                  fitnessdata.Waist,
                                  fitnessdata.Hip,
                                  fitnessdata.Calf,
                                  fitnessdata.Thighs
                              }).ToListAsync(); 
            return Ok(list);
        }



        [HttpPost("Userslist")]
        public async Task<IActionResult> GetGymuserdata([FromBody]  string gymcode)
        {
            

            var allGymUsers = await (
                from gymUser in _context.GymUsers
                join appUser in _context.ApplicationUsers
                on gymUser.Username equals appUser.UserName
                where gymUser.Gymcode == gymcode && gymUser.Active
                select new
                {
                    gymUser.Username,
                    appUser.FirstName,
                    appUser.LastName
                }).ToListAsync();

            
            return Ok(new { allGymUsers });
        }
    }

}
