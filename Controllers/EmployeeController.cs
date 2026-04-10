using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VANTHINGOCHANG_2123110352.Data;
using VANTHINGOCHANG_2123110352.Models;

namespace VANTHINGOCHANG_2123110352.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Employees.ToListAsync();
            return Ok(data);
        }

        // ================= GET BY ID =================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return NotFound();

            return Ok(emp);
        }

        // ================= CREATE =================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Employee emp)
        {
            // validate null
            if (string.IsNullOrEmpty(emp.Phone))
                return BadRequest("Phone is required");

            if (string.IsNullOrEmpty(emp.Name))
                return BadRequest("Name is required");

            // check trùng phone
            var exists = await _context.Employees
                .AnyAsync(e => e.Phone != null && e.Phone == emp.Phone);

            if (exists)
                return BadRequest("Phone already exists");

            _context.Employees.Add(emp);
            await _context.SaveChangesAsync();

            return Ok(emp);
        }

        // ================= UPDATE =================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Employee emp)
        {
            var e = await _context.Employees.FindAsync(id);
            if (e == null) return NotFound();

            // validate
            if (string.IsNullOrEmpty(emp.Phone))
                return BadRequest("Phone is required");

            if (string.IsNullOrEmpty(emp.Name))
                return BadRequest("Name is required");

            // check trùng phone (trừ chính nó)
            var exists = await _context.Employees
                .AnyAsync(x => x.Id != id && x.Phone == emp.Phone);

            if (exists)
                return BadRequest("Phone already exists");

            // update
            e.Name = emp.Name;
            e.Phone = emp.Phone;
            e.BaseSalary = emp.BaseSalary;

            await _context.SaveChangesAsync();

            return Ok(e);
        }

        // ================= DELETE =================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var e = await _context.Employees.FindAsync(id);
            if (e == null) return NotFound();

            _context.Employees.Remove(e);
            await _context.SaveChangesAsync();

            return Ok("Deleted successfully");
        }
    }
}