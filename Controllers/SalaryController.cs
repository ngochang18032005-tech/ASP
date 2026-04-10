using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VANTHINGOCHANG_2123110352.Data;
using VANTHINGOCHANG_2123110352.Models;

namespace VANTHINGOCHANG_2123110352.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalaryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SalaryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> Calculate(int employeeId, int month)
        {
            // ================= VALIDATE =================
            if (employeeId <= 0)
                return BadRequest("employeeId must be > 0");

            if (month < 1 || month > 12)
                return BadRequest("Invalid month");

            // ================= GET EMPLOYEE =================
            var emp = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (emp == null)
                return NotFound("Employee not found");

            // ================= TOTAL HOURS =================
            var attendances = await _context.Attendances
                .Where(a => a.EmployeeId == employeeId
                            && a.CheckIn != null
                            && a.CheckOut != null
                            && a.CheckIn.Value.Month == month)
                .ToListAsync();

            double totalHours = attendances
                .Sum(a => (a.CheckOut!.Value - a.CheckIn!.Value).TotalHours);

            // ================= BONUS =================
            decimal bonus = await _context.Bonuses
                .Where(b => b.EmployeeId == employeeId && b.Date.Month == month)
                .SumAsync(b => (decimal?)b.Amount) ?? 0;

            // ================= PENALTY =================
            decimal penalty = await _context.Penalties
                .Where(p => p.EmployeeId == employeeId && p.Date.Month == month)
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            // ================= SALARY CALC =================
            decimal salary =
                ((decimal)emp.BaseSalary / 160m) * (decimal)totalHours
                + bonus
                - penalty;

            // ================= DELETE OLD =================
            var oldSalary = await _context.Salaries
                .FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.Month == month);

            if (oldSalary != null)
            {
                _context.Salaries.Remove(oldSalary);
            }

            // ================= INSERT NEW =================
            var result = new Salary
            {
                EmployeeId = employeeId,
                Month = month,
                TotalHours = (decimal)totalHours,
                TotalSalary = salary
            };

            _context.Salaries.Add(result);

            await _context.SaveChangesAsync();

            return Ok(result);
        }
    }
}