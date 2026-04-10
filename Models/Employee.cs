using System.ComponentModel.DataAnnotations;

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
