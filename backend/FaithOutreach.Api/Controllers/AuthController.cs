using FaithOutreach.Api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FaithOutreach.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [EnableRateLimiting("registration")]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var inviteCode = _configuration["Registration:InviteCode"];
        if (string.IsNullOrWhiteSpace(inviteCode))
            return StatusCode(503, new { message = "Registration is closed for this review." });
        if (string.IsNullOrWhiteSpace(request.InviteCode) ||
            !System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(
                System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(inviteCode)),
                System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(request.InviteCode))))
            return Forbid();
        if (string.IsNullOrWhiteSpace(request.FullName) || request.FullName.Length > 100 ||
            string.IsNullOrWhiteSpace(request.Email) || request.Email.Length > 254 ||
            string.IsNullOrWhiteSpace(request.Password) || request.Password.Length > 128)
            return BadRequest(new { message = "Check the name, email, and password lengths." });
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
        {
            return Conflict(new { message = "User already exists." });
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        return Ok(new { message = "Registration successful." });
    }

    [EnableRateLimiting("login")]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        await _signInManager.SignInAsync(user, isPersistent: false);

        return Ok(new
        {
            message = "Login successful.",
            user = new
            {
                user.Id,
                user.Email,
                user.FullName,
                IsAdmin = await _userManager.IsInRoleAsync(user, "Admin")
            }
        });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var user = await _userManager.GetUserAsync(User);
        return user is null ? Unauthorized() : Ok(new { user = new { user.Id, user.Email, user.FullName, IsAdmin = await _userManager.IsInRoleAsync(user, "Admin") } });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { message = "Signed out." });
    }
}

public class RegisterRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string InviteCode { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string InviteCode { get; set; } = string.Empty;
}
