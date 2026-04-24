using System.ComponentModel.DataAnnotations;

namespace ASP.DTOs;

public class CreateRoomDto
{
    [Required]
    public string RoomNumber { get; set; }

    public string RoomType { get; set; } = "Standard";
    public decimal Price { get; set; } = 1500000;

    [Required]
    public string Status { get; set; }
}