using System.ComponentModel.DataAnnotations; // <--- KIỂM TRA DÒNG NÀY

namespace VANTHINGOCHANG_2123110352.Models
{
    public class Branch
    {
        [Key]
        public int BranchId { get; set; }
        public string? Name { get; set; }
    }
}