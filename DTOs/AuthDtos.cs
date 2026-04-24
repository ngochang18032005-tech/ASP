using System.ComponentModel.DataAnnotations;

namespace ASP.DTOs;

public class LoginDto
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}

public class RegisterDto
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    public string Role { get; set; } = "STAFF";
}

public class ForgotPasswordDto
{
    [Required]
    public string Username { get; set; }
}
