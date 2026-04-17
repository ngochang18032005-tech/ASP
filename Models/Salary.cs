namespace VANTHINGOCHANG_2123110352.Models
{
    public class Salary
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public int Month { get; set; }

        public decimal TotalHours { get; set; }

        public decimal TotalSalary { get; set; }

        // ✔ FIX NULL WARNING
        public Employee? Employee { get; set; }
    }
}