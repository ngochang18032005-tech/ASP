namespace VANTHINGOCHANG_2123110352.Models
{
    public class ShiftAssignment
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public int ShiftId { get; set; }
        public Shift? Shift { get; set; }

        public DateTime Date { get; set; }
    }
}