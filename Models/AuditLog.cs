using System.ComponentModel.DataAnnotations;

namespace VANTHINGOCHANG_2123110352.Models
{
    public class AuditLog
    {
        [Key] // Thêm dòng này
        public int Id { get; set; }

        public string? Action { get; set; }
        public DateTime Timestamp { get; set; }
        public string? UserId { get; set; }
        // ... các thuộc tính khác của bạn
    }
}