using Fitscan.API.Data;
using Fitscan_Api.DTOs;
using Fitscan_Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fitscan_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : Controller
    {
        private readonly FitscanDbContext _context;

        public PaymentController(FitscanDbContext context)
        {
            _context = context;
        }

        // Create Payment
        [HttpPost("add")]
        public async Task<IActionResult> AddPayment([FromBody] CreatePaymentDto dto)
        {
            var newPaymentId = await GenerateNextPaymentIdAsync();
            var payment = new PaymentDetails
            {
                PaymentId = newPaymentId,
                UserId = dto.UserId,
                PlanId = dto.PlanId,
                Amount = dto.Amount,
                PaymentStatus = dto.PaymentStatus,
                Gymcode = dto.Gymcode,
                PaymentMethod = dto.PaymentMethod,
                PaidOn = dto.PaidOn.ToUniversalTime(),
                TransactionId = dto.TransactionId,
                Notes = dto.Notes,
                UpdatedBy = dto.UpdatedBy,
                UpdatedOn = DateTime.UtcNow
            };

            _context.PaymentDetails.Add(payment);
            await _context.SaveChangesAsync();

            return Ok(payment);
        }

        // Update Payment
        [HttpPut("update")]
        public async Task<IActionResult> UpdatePayment([FromBody] UpdatePaymentDto dto)
        {
            var payment = await _context.PaymentDetails.FindAsync(dto.Id);
            if (payment == null)
                return NotFound("Payment not found");

            payment.PaymentId = dto.PaymentId;
            payment.UserId = dto.UserId;
            payment.PlanId = dto.PlanId;
            payment.Amount = dto.Amount;
            payment.PaymentStatus = dto.PaymentStatus;
            payment.PaymentMethod = dto.PaymentMethod;
            payment.PaidOn = dto.PaidOn;
            payment.TransactionId = dto.TransactionId;
            payment.Notes = dto.Notes;
            payment.UpdatedBy = dto.UpdatedBy;
            payment.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(payment);
        }

        // Delete Payment
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.PaymentDetails.FindAsync(id);
            if (payment == null)
                return NotFound("Payment not found");

            _context.PaymentDetails.Remove(payment);
            await _context.SaveChangesAsync();

            return Ok("Payment deleted successfully");
        }

        // Get by ID
        [HttpPost("fetchdetails")]
        public async Task<IActionResult> GetgymPaymentdetails([FromBody] string gymCode)
        {
            var payments = await (from payment in _context.PaymentDetails
                                  join user in _context.ApplicationUsers
                                  on payment.UserId equals user.UserName
                                  where payment.Gymcode == gymCode
                                  select new
                                  {
                                      payment.PaymentId,
                                      FirstName = user.FirstName,
                                      LastName = user.LastName,
                                      payment.PlanId,
                                      payment.Amount,
                                      payment.PaymentStatus,
                                      //payment.Gymcode,
                                      payment.PaymentMethod,
                                      payment.PaidOn,
                                      payment.TransactionId,
                                      payment.Notes
                                     // payment.UpdatedBy,
                                      //payment.UpdatedOn
                                  }).ToListAsync();

            if (!payments.Any())
                return NotFound("No payments found");

            return Ok(payments);
        }

        // Get all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _context.PaymentDetails.ToListAsync();
            return Ok(payments);
        }


        private async Task<string> GenerateNextPaymentIdAsync()
        {
            var lastPayment = await _context.PaymentDetails
                .OrderByDescending(p => p.PaymentId)
                .FirstOrDefaultAsync();

            if (lastPayment == null || string.IsNullOrEmpty(lastPayment.PaymentId))
                return "PAY0001";

            var lastIdNumber = int.Parse(lastPayment.PaymentId.Substring(3)); // "0001" → 1
            var newIdNumber = lastIdNumber + 1;

            return $"PAY{newIdNumber.ToString("D4")}"; // → "PID0002"
        }

    }

}
