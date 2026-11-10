using System.Text.Json;
using FaithOutreach.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FaithOutreach.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentController : ControllerBase
{
    private static readonly HashSet<string> Categories = new(StringComparer.OrdinalIgnoreCase)
    { "readings", "teachings", "audiobooks", "podcasts", "guides", "hymns", "reflections" };
    private readonly ApplicationDbContext _db;
    public ContentController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.ContentItems.Where(x => x.IsPublished)
            .OrderByDescending(x => x.CreatedAt).ToListAsync());

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public async Task<IActionResult> GetAdmin() =>
        Ok(await _db.ContentItems.OrderByDescending(x => x.CreatedAt).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _db.ContentItems.FindAsync(id);
        if (item is null) return NotFound();
        if (!item.IsPublished && !User.IsInRole("Admin")) return NotFound();
        return Ok(item);
    }

    private static string? ValidationError(ContentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Length > 200)
            return "Title is required and must be at most 200 characters.";
        if (!Categories.Contains(request.Category ?? "")) return "Unsupported category.";
        try
        {
            using var doc = JsonDocument.Parse(request.Body ?? "");
            if (doc.RootElement.ValueKind != JsonValueKind.Object) return "Body must be a JSON object.";
            var category = request.Category?.ToLowerInvariant();
            if (category is "teachings" or "audiobooks" or "podcasts" or "guides")
            {
                var hasExternal = false;
                if (doc.RootElement.TryGetProperty("url", out var url) &&
                    url.ValueKind == JsonValueKind.String &&
                    !string.IsNullOrWhiteSpace(url.GetString()))
                {
                    if (!Uri.TryCreate(url.GetString(), UriKind.Absolute, out var uri) ||
                        uri.Scheme != Uri.UriSchemeHttps)
                        return "Publisher links must use HTTPS.";
                    hasExternal = true;
                }
                var hasUpload = doc.RootElement.TryGetProperty("mediaUrl", out var mediaUrl) &&
                    mediaUrl.ValueKind == JsonValueKind.String &&
                    System.Text.RegularExpressions.Regex.IsMatch(mediaUrl.GetString() ?? "", @"^/media/[a-f0-9]{32}\.(mp3|m4a|mp4|webm)$");
                if (category == "guides" ? !hasExternal : !hasExternal && !hasUpload)
                    return "Provide an HTTPS publisher link or an uploaded media file.";
            }
        }
        catch (JsonException) { return "Body must contain valid JSON."; }
        return null;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ContentRequest request)
    {
        var error = ValidationError(request);
        if (error is not null) return BadRequest(new { message = error });
        var item = new ContentItem
        {
            Title = request.Title.Trim(),
            Category = request.Category.ToLowerInvariant(),
            Body = request.Body,
            IsPublished = request.IsPublished,
            CreatedAt = DateTime.UtcNow
        };
        _db.ContentItems.Add(item);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ContentRequest request)
    {
        var error = ValidationError(request);
        if (error is not null) return BadRequest(new { message = error });
        var item = await _db.ContentItems.FindAsync(id);
        if (item is null) return NotFound();
        item.Title = request.Title.Trim();
        item.Category = request.Category.ToLowerInvariant();
        item.Body = request.Body;
        item.IsPublished = request.IsPublished;
        await _db.SaveChangesAsync();
        return Ok(item);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.ContentItems.FindAsync(id);
        if (item is null) return NotFound();
        _db.ContentItems.Remove(item);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public sealed class ContentRequest
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
}
