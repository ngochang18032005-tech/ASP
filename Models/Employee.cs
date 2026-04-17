<<<<<<< HEAD
namespace ASP.Models;
using System;
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int RoleId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
=======
﻿using System.ComponentModel.DataAnnotations;

using VANTHINGOCHANG_2123110352.Models;

    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Phone { get; set; }

        public decimal BaseSalary { get; set; }

        // Navigation
        public List<Attendance> Attendances { get; set; } = new();
        public List<Salary> Salaries { get; set; } = new();
    }
>>>>>>> 3d6026659e82f5ef66e782889a0d3e362d3a1c50
