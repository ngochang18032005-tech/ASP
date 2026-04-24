namespace ASP.Models;
using System;

public class Booking
{
    public int Id { get; set; }

    public int RoomId { get; set; }
    public Room Room { get; set; }

    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }

    public string CustomerName { get; set; }
    public string CustomerPhone { get; set; }
    public string CustomerEmail { get; set; }

    public string Status { get; set; } = "PENDING";
    public decimal TotalAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public int DepositPercentage { get; set; } = 100; // 100 for full, 20-40 for deposit
    public bool IsDepositPaid { get; set; } = false;
    public bool IsEmailSent { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}