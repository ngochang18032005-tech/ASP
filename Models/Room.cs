namespace ASP.Models;

public class Room
{
    public int Id { get; set; }
    public string RoomNumber { get; set; }
    public string RoomType { get; set; } = "Standard";
    public decimal Price { get; set; } = 1500000;
    public string Status { get; set; } = "AVAILABLE";
}