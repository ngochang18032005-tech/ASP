using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ASP.Data;
using ASP.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ASP.Controllers;

public class CustomerController : Controller
{
    private readonly AppDbContext _context;
    private readonly ASP.Services.EmailService _emailService;

    public CustomerController(AppDbContext context, ASP.Services.EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string email)
    {
        var customer = _context.Customers.FirstOrDefault(c => c.Email == email);
        if (customer == null)
        {
            ViewBag.Error = "Email không tồn tại trong hệ thống!";
            return View();
        }

        // Create 6-digit OTP
        var otp = new System.Random().Next(100000, 999999).ToString();
        customer.OTP = otp;
        customer.OTPExpiry = System.DateTime.Now.AddMinutes(10);
        _context.SaveChanges();

        // Send Email with OTP
        var subject = "Mã OTP khôi phục mật khẩu - LuxManage Resort";
        var body = $@"
            <div style='font-family: Arial, sans-serif; padding: 30px; border: 1px solid #eee; border-radius: 15px; text-align: center; max-width: 500px; margin: auto;'>
                <h2 style='color: #d4af37; margin-bottom: 20px;'>Mã xác thực OTP</h2>
                <p>Chào <b>{customer.FullName}</b>,</p>
                <p>Bạn đang thực hiện khôi phục mật khẩu. Vui lòng sử dụng mã OTP dưới đây để xác thực:</p>
                <div style='background: #f9f9f9; padding: 20px; font-size: 32px; font-weight: bold; letter-spacing: 10px; color: #1a1a1a; border-radius: 10px; margin: 25px 0;'>
                    {otp}
                </div>
                <p style='color: #666; font-size: 0.9em;'>Mã này có hiệu lực trong vòng 10 phút. Tuyệt đối không cung cấp mã này cho bất kỳ ai.</p>
                <hr style='margin: 30px 0; border: 0; border-top: 1px solid #eee;'>
                <p style='font-size: 0.8em; color: #999;'>© 2026 LuxManage Resort & Spa</p>
            </div>";

        try {
            await _emailService.SendEmailAsync(customer.Email, subject, body);
            return RedirectToAction("VerifyOTP", new { email = customer.Email });
        } catch (System.Exception ex) {
            ViewBag.Error = "Lỗi gửi mail: " + ex.Message;
            return View();
        }
    }

    [HttpGet]
    public IActionResult VerifyOTP(string email)
    {
        ViewBag.Email = email;
        return View();
    }

    [HttpPost]
    public IActionResult VerifyOTP(string email, string otp)
    {
        var customer = _context.Customers.FirstOrDefault(c => c.Email == email && c.OTP == otp && c.OTPExpiry > System.DateTime.Now);
        if (customer == null)
        {
            ViewBag.Error = "Mã OTP không chính xác hoặc đã hết hạn!";
            ViewBag.Email = email;
            return View();
        }

        return RedirectToAction("ResetPassword", new { email = email, otp = otp });
    }

    [HttpGet]
    public IActionResult ResetPassword(string email, string otp)
    {
        var customer = _context.Customers.FirstOrDefault(c => c.Email == email && c.OTP == otp && c.OTPExpiry > System.DateTime.Now);
        if (customer == null) return RedirectToAction("ForgotPassword");

        ViewBag.Email = email;
        ViewBag.OTP = otp;
        return View();
    }

    [HttpPost]
    public IActionResult ResetPassword(string email, string otp, string newPassword)
    {
        var customer = _context.Customers.FirstOrDefault(c => c.Email == email && c.OTP == otp && c.OTPExpiry > System.DateTime.Now);
        if (customer == null) return RedirectToAction("ForgotPassword");

        customer.Password = newPassword;
        customer.OTP = null;
        customer.OTPExpiry = null;
        _context.SaveChanges();

        return RedirectToAction("Login", new { msg = "Đổi mật khẩu thành công!" });
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var customer = _context.Customers.FirstOrDefault(c => c.Email == email && c.Password == password);
        if (customer != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, customer.FullName),
                new Claim(ClaimTypes.Email, customer.Email),
                new Claim(ClaimTypes.Role, "CUSTOMER")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }
        
        ViewBag.Error = "Email hoặc mật khẩu không chính xác!";
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string fullName, string email, string phone, string password)
    {
        if (_context.Customers.Any(c => c.Email == email))
        {
            ViewBag.Error = "Email này đã được sử dụng!";
            return View();
        }

        var customer = new Customer
        {
            FullName = fullName,
            Email = email,
            Phone = phone,
            Password = password
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        // Tự động đăng nhập luôn
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, customer.FullName),
            new Claim(ClaimTypes.Email, customer.Email),
            new Claim(ClaimTypes.Role, "CUSTOMER")
        };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "CUSTOMER")]
    [HttpGet]
    public IActionResult Profile()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var customer = _context.Customers.FirstOrDefault(c => c.Email == email);
        if (customer == null) return RedirectToAction("Login");

        var bookings = _context.Bookings
            .Include(b => b.Room)
            .Where(b => b.CustomerEmail == email)
            .OrderByDescending(b => b.Id)
            .ToList();

        ViewBag.Bookings = bookings;
        return View(customer);
    }

    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "CUSTOMER")]
    [HttpPost]
    public async Task<IActionResult> Profile(string fullName, string phone, string currentPassword, string newPassword)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var customer = _context.Customers.FirstOrDefault(c => c.Email == email);
        if (customer == null) return RedirectToAction("Login");

        // Simple validation
        if (!string.IsNullOrEmpty(currentPassword))
        {
            if (customer.Password != currentPassword)
            {
                ViewBag.Error = "Mật khẩu hiện tại không đúng!";
                return View(customer);
            }
            if (!string.IsNullOrEmpty(newPassword))
            {
                customer.Password = newPassword;
            }
        }

        customer.FullName = fullName;
        customer.Phone = phone;

        _context.SaveChanges();

        // Update name claim if changed
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, customer.FullName),
            new Claim(ClaimTypes.Email, customer.Email),
            new Claim(ClaimTypes.Role, "CUSTOMER")
        };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

        ViewBag.Success = "Cập nhật thông tin thành công!";
        return View(customer);
    }
}
