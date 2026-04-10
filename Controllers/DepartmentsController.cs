using Microsoft.AspNetCore.Mvc;
using VANTHINGOCHANG_2123110352.Data;
using VANTHINGOCHANG_2123110352.Models;

namespace VANTHINGOCHANG_2123110352.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DepartmentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Departments.ToList());

        [HttpPost]
        public IActionResult Create(Department d)
        {
            _context.Departments.Add(d);
            _context.SaveChanges();
            return Ok(d);
        }
    }
}
