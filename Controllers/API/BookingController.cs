using System;
using ASP.Data;
using ASP.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ASP.Controllers.API;

[ApiController]
[Route("api/bookings")]
public class BookingController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ASP.Services.EmailService _emailService;

    public BookingController(AppDbContext context, ASP.Services.EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    // GET: api/bookings
    [HttpGet]
    public IActionResult GetAll()
    {
        var data = _context.Bookings
            .Select(b => new {
                b.Id,
                b.RoomId,
                b.CheckIn,
                b.CheckOut,
                b.Status,
                b.CustomerName,
                b.CustomerPhone,
                b.CustomerEmail
            }).ToList();

        return Ok(data);
    }

    // POST: api/bookings
    [HttpPost]
    public async Task<IActionResult> Create(Booking model)
    {
        bool isAvailable = !_context.Bookings.Any(b =>
            b.RoomId == model.RoomId &&
            (model.CheckIn < b.CheckOut && model.CheckOut > b.CheckIn)
        );

        if (!isAvailable)
            return Conflict("Room already booked");

        // Đảm bảo các giá trị mặc định luôn đúng
        model.CreatedAt = DateTime.Now;
        model.Status = "PENDING_PAYMENT";
        model.IsDepositPaid = false;
        model.IsEmailSent = false;
        
        if (model.DepositPercentage == 0) model.DepositPercentage = 30;

        _context.Bookings.Add(model);
        await _context.SaveChangesAsync();

        // Send Confirmation Email
        var room = _context.Rooms.Find(model.RoomId);
        var subject = "Xác nhận đăng ký phòng thành công - LuxManage Resort";
        var body = $@"
            <div style='font-family: Arial, sans-serif; padding: 25px; border: 1px solid #e0e0e0; border-radius: 12px; color: #333;'>
                <div style='text-align: center; margin-bottom: 20px;'>
                    <h1 style='color: #d4af37; margin: 0;'> LuxManage Resort </h1>
                    <p style='color: #888; font-size: 0.9em;'>Cảm ơn bạn đã lựa chọn chúng tôi!</p>
                </div>
                <h3 style='border-bottom: 2px solid #d4af37; padding-bottom: 10px;'>Thông tin đăng ký phòng:</h3>
                <p><b>Mã đơn đăng ký:</b> #{model.Id}</p>
                <p><b>Khách hàng:</b> {model.CustomerName}</p>
                <p><b>Phòng:</b> {room?.RoomNumber} ({room?.RoomType})</p>
                <p><b>Ngày nhận phòng:</b> {model.CheckIn:dd/MM/yyyy}</p>
                <p><b>Ngày trả phòng:</b> {model.CheckOut:dd/MM/yyyy}</p>
                <p style='font-size: 1.1em; color: #d4af37; margin-top: 15px;'><b>Số tiền cần đặt cọc (30%): {model.DepositAmount:N0} VND</b></p>
                <p><b>Trạng thái:</b> Đã xác nhận đăng ký</p>
                <div style='margin-top: 25px; padding: 15px; background: #fdf8e6; border-radius: 8px;'>
                    <p style='margin: 0;'><b>Hướng dẫn đặt cọc:</b> Vui lòng chuyển khoản tiền cọc vào tài khoản: <b>MB Bank - 123456789 (LuxManage Resort)</b> với nội dung: <b>DC {model.Id}</b>.</p>
                </div>
                <div style='margin-top: 15px; padding: 15px; background: #f9f9f9; border-radius: 8px;'>
                    <p style='margin: 0;'>Chúng tôi sẽ sớm liên hệ với bạn qua số điện thoại <b>{model.CustomerPhone}</b> để xác nhận sau khi nhận được tiền cọc.</p>
                </div>
                <hr style='margin: 30px 0; border: 0; border-top: 1px solid #eee;'>
                <p style='font-size: 0.8em; color: #999; text-align: center;'>123 Đường Tôn Đức Thắng, Quận 1, TP. HCM | Hotline: 1800-xxxx</p>
            </div>";

        try {
            await _emailService.SendEmailAsync(model.CustomerEmail, subject, body);
        } catch (System.Exception ex) {
            // Log error but don't fail the booking
            Console.WriteLine("Email Error: " + ex.Message);
        }

        return Ok(model);
    }

    // GET: api/bookings/{id}/status
    [HttpGet("{id}/status")]
    public IActionResult GetStatus(int id)
    {
        var booking = _context.Bookings.Find(id);
        if (booking == null) return NotFound();
        return Ok(new { status = booking.Status, isPaid = booking.IsDepositPaid });
    }

    // GET: api/bookings/recent
    [HttpGet("recent")]
    public IActionResult GetRecent()
    {
        var recent = _context.Bookings
            .OrderByDescending(b => b.Id)
            .Take(5)
            .Select(b => new { b.Id, b.CustomerName, b.CreatedAt })
            .ToList();
        return Ok(recent);
    }

    // GET: api/bookings/latest
    [HttpGet("latest")]
    public IActionResult GetLatest()
    {
        var latest = _context.Bookings
            .OrderByDescending(b => b.Id)
            .Select(b => new { b.Id, b.CustomerName, b.CreatedAt })
            .FirstOrDefault();
        return Ok(latest);
    }

    // PUT: api/bookings/{id}/checkin
    [HttpPut("{id}/checkin")]
    public IActionResult CheckIn(int id)
    {
        var booking = _context.Bookings.Find(id);
        if (booking == null) return NotFound();

        booking.Status = "CHECKED_IN";
        
        var room = _context.Rooms.Find(booking.RoomId);
        if(room != null) {
            room.Status = "Có khách";
        }

        _context.SaveChanges();
        return Ok();
    }

    // PUT: api/bookings/{id}/confirm-deposit
    [HttpPut("{id}/confirm-deposit")]
    public IActionResult ConfirmDeposit(int id)
    {
        var booking = _context.Bookings.Find(id);
        if (booking == null) return NotFound();

        booking.IsDepositPaid = true;
        booking.Status = "DEPOSIT_PAID";
        _context.SaveChanges();

        // Send Payment Success Email
        var room = _context.Rooms.Find(booking.RoomId);
        var subject = "Đã nhận tiền cọc - LuxManage Resort";
        var body = $@"
            <div style='font-family: Arial, sans-serif; padding: 25px; border: 1px solid #e0e0e0; border-radius: 12px;'>
                <h2 style='color: #28a745;'>Thanh toán thành công!</h2>
                <p>Chào <b>{booking.CustomerName}</b>,</p>
                <p>LuxManage Resort xác nhận đã nhận được số tiền cọc: <b>{booking.DepositAmount:N0} VNĐ</b> cho đơn đăng ký phòng <b>#{booking.Id}</b>.</p>
                <div style='background: #f8f9fa; padding: 15px; border-radius: 8px;'>
                    <p><b>Phòng:</b> {room?.RoomNumber} ({room?.RoomType})</p>
                    <p><b>Trạng thái:</b> Đã giữ phòng thành công</p>
                </div>
                <p style='margin-top: 20px;'>Hẹn gặp lại bạn vào ngày <b>{booking.CheckIn:dd/MM/yyyy}</b>. Chúc bạn có một kỳ nghỉ tuyệt vời!</p>
                <hr>
                <p style='font-size: 0.8em; color: #999;'>Đây là email tự động, vui lòng không trả lời.</p>
            </div>";

        try {
            _emailService.SendEmailAsync(booking.CustomerEmail, subject, body);
            booking.IsEmailSent = true;
            _context.SaveChanges();
        } catch { }

        return Ok(new { success = true });
    }

    // PUT: api/bookings/{id}/checkout
    [HttpPut("{id}/checkout")]
    public IActionResult CheckOut(int id)
    {
        var booking = _context.Bookings.Find(id);
        if (booking == null) return NotFound();

        booking.Status = "CHECKED_OUT";

        var room = _context.Rooms.Find(booking.RoomId);
        if(room != null) {
            room.Status = "Đang dọn";
        }

        _context.SaveChanges();
        return Ok();
    }
}