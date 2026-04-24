using ASP.Data;
using ASP.DTOs;
using ASP.Models;
using ASP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ASP.Controllers.API;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username && u.Password == dto.Password);
        
        if (user == null)
            return Unauthorized(new { message = "Sai tài khoản hoặc mật khẩu" });

        var token = _jwtService.GenerateToken(user);
        
        return Ok(new { token = token, username = user.Username, role = user.Role });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            return BadRequest(new { message = "Tài khoản đã tồn tại" });

        var user = new User
        {
            Username = dto.Username,
            Password = dto.Password, // In production, this must be hashed
            Role = dto.Role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Đăng ký thành công" });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // JWT is stateless, logout is handled client-side by deleting the token
        return Ok(new { message = "Đăng xuất thành công. Vui lòng xóa token ở phía Client." });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user == null)
            return NotFound(new { message = "Không tìm thấy tài khoản" });

        // Mock password reset
        return Ok(new { message = "Hướng dẫn khôi phục mật khẩu đã được gửi qua email (giả lập)." });
    }
}
