using Microsoft.AspNetCore.Mvc;
using VANTHINGOCHANG_2123110352.Data;
using VANTHINGOCHANG_2123110352.Models;

namespace VANTHINGOCHANG_2123110352.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShiftsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ShiftsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Shifts.ToList());

        [HttpPost]
        public IActionResult Create(Shift s)
        {
            _context.Shifts.Add(s);
            _context.SaveChanges();
            return Ok(s);
        }
    }
}
