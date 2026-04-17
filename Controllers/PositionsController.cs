using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VANTHINGOCHANG_2123110352.Data;
using VANTHINGOCHANG_2123110352.Models;

namespace VANTHINGOCHANG_2123110352.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PositionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PositionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/positions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Positions.ToListAsync();
            return Ok(data);
        }

        // GET: api/positions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pos = await _context.Positions.FindAsync(id);
            if (pos == null)
                return NotFound();

            return Ok(pos);
        }

        // POST: api/positions
        [HttpPost]
        public async Task<IActionResult> Create(Position p)
        {
            if (string.IsNullOrEmpty(p.Name))
                return BadRequest("Name is required");

            await _context.Positions.AddAsync(p);
            await _context.SaveChangesAsync();

            return Ok(p);
        }

        // PUT: api/positions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Position p)
        {
            var exist = await _context.Positions.FindAsync(id);
            if (exist == null)
                return NotFound();

            exist.Name = p.Name;

            await _context.SaveChangesAsync();

            return Ok(exist);
        }

        // DELETE: api/positions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var pos = await _context.Positions.FindAsync(id);
            if (pos == null)
                return NotFound();

            _context.Positions.Remove(pos);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}