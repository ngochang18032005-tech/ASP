namespace ASP.Models;
using System;
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int RoleId { get; set; }
    public string Password { get; set; } = "admin123";
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
