using VANTHINGOCHANG_2123110352.Models;

public class Shift
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public List<ShiftAssignment> Assignments { get; set; } = new();
}