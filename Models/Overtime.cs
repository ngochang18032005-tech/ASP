namespace VANTHINGOCHANG_2123110352.Models
{
    public class Overtime
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public float Hours { get; set; }
        public DateTime Date { get; set; }
    }
}
