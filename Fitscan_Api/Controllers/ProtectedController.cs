using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fitscan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProtectedController : ControllerBase
    {
        [Authorize(Roles = "SystemAdmin,Owner")]
        [HttpGet("members")]
        public IActionResult GetMembers() => Ok("This is a protected members list.");

        [Authorize(Roles = "Owner,Trainer")]
        [HttpGet("attendance")]
        public IActionResult GetAttendance() => Ok("This is a protected attendance log.");

        [Authorize(Roles = "SalesExecutive")]
        [HttpGet("gyms")]
        public IActionResult GetGyms() => Ok("Sales dashboard view.");
    }
}