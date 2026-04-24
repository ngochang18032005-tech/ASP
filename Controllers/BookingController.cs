using Microsoft.AspNetCore.Mvc;
using ASP.Data;
using ASP.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ASP.Controllers
{
    public class BookingController : Controller
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Payment(int id)
        {
            var booking = _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefault(b => b.Id == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        [HttpPost]
        public IActionResult ProcessPayment(int id, int percentage)
        {
            var booking = _context.Bookings.Find(id);
            if (booking == null) return NotFound();

            booking.DepositPercentage = percentage;
            booking.DepositAmount = booking.TotalAmount * percentage / 100;
            booking.Status = "AWAITING_PAYMENT";
            _context.SaveChanges();

            return Json(new { success = true, depositAmount = booking.DepositAmount });
        }
    }
}
