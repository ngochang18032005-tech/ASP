using Microsoft.AspNetCore.Mvc;
using VANTHINGOCHANG_2123110352.Data;
using VANTHINGOCHANG_2123110352.Models;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly AppDbContext _context;

    public AttendanceController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("checkin")]
    public IActionResult CheckIn(int employeeId)
    {
        var today = DateTime.Today;

        var exist = _context.Attendances
            .FirstOrDefault(a => a.EmployeeId == employeeId &&
                                 a.CheckIn.Value.Date == today);

        if (exist != null)
            return BadRequest("Already check-in");

        var att = new Attendance
        {
            EmployeeId = employeeId,
            CheckIn = DateTime.Now,
            Status = "present"
        };

        _context.Attendances.Add(att);
        _context.SaveChanges();

        return Ok(att);
    }

    [HttpPost("checkout")]
    public IActionResult CheckOut(int employeeId)
    {
        var today = DateTime.Today;

        var att = _context.Attendances
            .FirstOrDefault(a => a.EmployeeId == employeeId &&
                                 a.CheckIn.Value.Date == today);

        if (att == null)
            return BadRequest("No check-in");

        att.CheckOut = DateTime.Now;
        _context.SaveChanges();

        return Ok(att);
    }
}