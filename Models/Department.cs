using System.ComponentModel.DataAnnotations;

namespace VANTHINGOCHANG_2123110352.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; } // Phải đặt đúng tên này
        public string? Name { get; set; }
    }
}