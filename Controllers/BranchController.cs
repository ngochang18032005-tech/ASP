using Microsoft.AspNetCore.Mvc;
using VANTHINGOCHANG_2123110352.Data;
using VANTHINGOCHANG_2123110352.Models; // <--- THÊM DÒNG NÀY ĐỂ HẾT LỖI CS0246

[ApiController]
[Route("api/[controller]")]
public class BranchesController : ControllerBase
{
    private readonly AppDbContext _context;

    public BranchesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_context.Branches.ToList());

    [HttpPost]
    public IActionResult Create(Branch b)
    {
        _context.Branches.Add(b);
        _context.SaveChanges();
        return Ok(b);
    }
}