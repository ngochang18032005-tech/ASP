using System.ComponentModel.DataAnnotations;

namespace VANTHINGOCHANG_2123110352.Models
{
    public class Position
    {
        [Key]
        public int PositionId { get; set; } // Phải đặt đúng tên này
        public string? Name { get; set; }
    }
}