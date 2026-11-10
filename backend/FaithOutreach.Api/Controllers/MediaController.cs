using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using FaithOutreach.Api.Data;

namespace FaithOutreach.Api.Controllers;

[ApiController]
[Route("api/media")]
public sealed class MediaController : ControllerBase
{
    private static readonly Dictionary<string, (string Mime, string Kind, long MaxBytes)> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        [".mp3"] = ("audio/mpeg", "audio", 50L * 1024 * 1024),
        [".m4a"] = ("audio/mp4", "audio", 50L * 1024 * 1024),
        [".mp4"] = ("video/mp4", "video", 250L * 1024 * 1024),
        [".webm"] = ("video/webm", "video", 250L * 1024 * 1024)
    };
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _environment;
    public MediaController(ApplicationDbContext db, IConfiguration config, IWebHostEnvironment environment)
    {
        _db = db;
        _config = config;
        _environment = environment;
    }

    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("upload")]
    [HttpPost]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(260L * 1024 * 1024)]
    public async Task<IActionResult> Upload([FromForm] IFormFile? file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0) return BadRequest(new { message = "Select an audio or video file." });
        var extension = Path.GetExtension(file.FileName);
        if (!Allowed.TryGetValue(extension, out var format))
            return BadRequest(new { message = "Use MP3, M4A, MP4, or WebM." });
        if (file.Length > format.MaxBytes)
            return BadRequest(new { message = format.Kind == "audio" ? "Audio must be 50 MB or less." : "Video must be 250 MB or less." });
        await using var input = file.OpenReadStream();
        var prefix = new byte[12];
        var count = await input.ReadAsync(prefix.AsMemory(0, prefix.Length), cancellationToken);
        if (!HasSignature(extension, prefix, count))
            return BadRequest(new { message = "The file does not match its audio or video format." });
        var directory = _config["Media:StoragePath"];
        if (string.IsNullOrWhiteSpace(directory))
            directory = Path.Combine(_environment.ContentRootPath, "media");
        directory = Path.GetFullPath(directory);
        Directory.CreateDirectory(directory);
        var name = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var path = Path.Combine(directory, name);
        await using (var output = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true))
        {
            await output.WriteAsync(prefix.AsMemory(0, count), cancellationToken);
            await input.CopyToAsync(output, cancellationToken);
        }
        return Ok(new { url = $"/media/{name}", type = format.Kind, size = file.Length });
    }

    [AllowAnonymous]
    [HttpGet("/media/{name}")]
    public async Task<IActionResult> Get(string name)
    {
        if (!System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-f0-9]{32}\.(mp3|m4a|mp4|webm)$"))
            return NotFound();
        var reference = $"/media/{name}";
        if (!await _db.ContentItems.AnyAsync(item => item.IsPublished && item.Body.Contains(reference)))
            return NotFound();
        var directory = _config["Media:StoragePath"];
        if (string.IsNullOrWhiteSpace(directory)) directory = Path.Combine(_environment.ContentRootPath, "media");
        var path = Path.Combine(Path.GetFullPath(directory), name);
        if (!System.IO.File.Exists(path)) return NotFound();
        Response.Headers.CacheControl = "no-store";
        Response.Headers.XContentTypeOptions = "nosniff";
        return PhysicalFile(path, Allowed[Path.GetExtension(name)].Mime, enableRangeProcessing: true);
    }

    private static bool HasSignature(string extension, byte[] bytes, int count) => extension.ToLowerInvariant() switch
    {
        ".mp3" => count >= 3 && ((bytes[0] == 'I' && bytes[1] == 'D' && bytes[2] == '3') || (bytes[0] == 0xff && (bytes[1] & 0xe0) == 0xe0)),
        ".m4a" or ".mp4" => count >= 8 && bytes[4] == 'f' && bytes[5] == 't' && bytes[6] == 'y' && bytes[7] == 'p',
        ".webm" => count >= 4 && bytes[0] == 0x1a && bytes[1] == 0x45 && bytes[2] == 0xdf && bytes[3] == 0xa3,
        _ => false
    };
}
