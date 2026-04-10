namespace VANTHINGOCHANG_2123110352.Models
{
    public class Bonus
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public float Amount { get; set; }
        public string Reason { get; set; } = string.Empty;

        public DateTime Date { get; set; }
    }
}
