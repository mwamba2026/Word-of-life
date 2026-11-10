using System.ComponentModel.DataAnnotations;
using FaithOutreach.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FaithOutreach.Api.Controllers;

[ApiController]
[Route("api")]
public class MessagesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MessagesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [EnableRateLimiting("public-form")]
    [HttpPost("feedback")]
    public async Task<IActionResult> SendFeedback([FromBody] FeedbackRequest request)
    {
        var item = new FeedbackMessage
        {
            Name = CleanOptional(request.Name),
            Email = CleanOptional(request.Email),
            Message = request.Message.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _context.FeedbackMessages.Add(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Feedback received. Thank you." });
    }

    [EnableRateLimiting("public-form")]
    [HttpPost("support")]
    public async Task<IActionResult> SendSupportRequest([FromBody] SupportMessageRequest request)
    {
        var item = new SupportRequest
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            Phone = CleanOptional(request.Phone),
            Topic = request.Topic.Trim(),
            Message = request.Message.Trim(),
            Status = "New",
            CreatedAt = DateTime.UtcNow
        };

        _context.SupportRequests.Add(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Support request received and saved. A personal reply is not guaranteed." });
    }

    private static string? CleanOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

public sealed class FeedbackRequest
{
    [MaxLength(100)]
    public string? Name { get; set; }

    [EmailAddress, MaxLength(254)]
    public string? Email { get; set; }

    [Required, MaxLength(4000)]
    public string Message { get; set; } = string.Empty;
}

public sealed class SupportMessageRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(254)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(30)]
    public string? Phone { get; set; }

    [Required, MaxLength(100)]
    public string Topic { get; set; } = string.Empty;

    [Required, MaxLength(4000)]
    public string Message { get; set; } = string.Empty;
}
