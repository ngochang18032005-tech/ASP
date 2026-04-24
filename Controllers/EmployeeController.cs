using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ASP.Controllers;

public class EmployeeController : Controller
{
    private readonly ASP.Data.AppDbContext _context;

    public EmployeeController(ASP.Data.AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Customers()
    {
        var customers = _context.Customers.ToList();
        return View(customers);
    }
}
