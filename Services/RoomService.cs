using ASP.Data;
using ASP.DTOs;
using ASP.Models;
using System.Linq;

namespace ASP.Services;

public class RoomService
{
    private readonly AppDbContext _context;

    public RoomService(AppDbContext context)
    {
        _context = context;
    }

    // GET ALL (paging + filter)
    public object GetAll(int page, int pageSize, string? status, string? search)
    {
        var query = _context.Rooms.AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(r => r.Status == status);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(r => r.RoomNumber.Contains(search));

        var total = query.Count();

        var data = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new RoomDto
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                RoomType = r.RoomType,
                Price = r.Price,
                Status = r.Status
            }).ToList();

        return new
        {
            total,
            page,
            pageSize,
            data
        };
    }

    // GET BY ID
    public Room? GetById(int id)
    {
        return _context.Rooms.Find(id);
    }

    // CREATE
    public Room Create(CreateRoomDto dto)
    {
        var room = new Room
        {
            RoomNumber = dto.RoomNumber,
            RoomType = dto.RoomType,
            Price = dto.Price,
            Status = dto.Status
        };

        _context.Rooms.Add(room);
        _context.SaveChanges();

        return room;
    }

    // UPDATE
    public Room? Update(int id, CreateRoomDto dto)
    {
        var room = _context.Rooms.Find(id);
        if (room == null) return null;

        room.RoomNumber = dto.RoomNumber;
        room.RoomType = dto.RoomType;
        room.Price = dto.Price;
        room.Status = dto.Status;

        _context.SaveChanges();
        return room;
    }

    // DELETE
    public bool Delete(int id)
    {
        var room = _context.Rooms.Find(id);
        if (room == null) return false;

        _context.Rooms.Remove(room);
        _context.SaveChanges();

        return true;
    }
}