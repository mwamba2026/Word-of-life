using System.Security.Cryptography;
using System.Text;
using FaithOutreach.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FaithOutreach.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly RoleManager<IdentityRole> _roles;
    private readonly IConfiguration _config;
    public AdminController(UserManager<ApplicationUser> users, RoleManager<IdentityRole> roles, IConfiguration config)
    {
        _users = users; _roles = roles; _config = config;
    }

    [HttpPost("bootstrap")]
    public async Task<IActionResult> Bootstrap([FromHeader(Name = "X-Admin-Bootstrap-Key")] string? key)
    {
        var email = _config["Admin:Email"];
        var secret = _config["Admin:BootstrapKey"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(secret))
            return StatusCode(503, new { message = "Admin bootstrap is not configured." });
        var user = await _users.GetUserAsync(User);
        if (user is null || !string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            return Forbid();
        var expected = SHA256.HashData(Encoding.UTF8.GetBytes(secret));
        var supplied = SHA256.HashData(Encoding.UTF8.GetBytes(key ?? ""));
        if (!CryptographicOperations.FixedTimeEquals(expected, supplied))
            return Forbid();
        if (!await _roles.RoleExistsAsync("Admin"))
        {
            var created = await _roles.CreateAsync(new IdentityRole("Admin"));
            if (!created.Succeeded) return StatusCode(500);
        }
        if ((await _users.GetUsersInRoleAsync("Admin")).Count > 0)
            return Conflict(new { message = "An administrator already exists." });
        var result = await _users.AddToRoleAsync(user, "Admin");
        if (!result.Succeeded) return StatusCode(500, new { errors = result.Errors.Select(e => e.Description) });
        await _users.UpdateSecurityStampAsync(user);
        return Ok(new { message = "Administrator role assigned. Sign in again." });
    }
}
