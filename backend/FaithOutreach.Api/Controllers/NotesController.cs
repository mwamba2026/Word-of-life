using System.ComponentModel.DataAnnotations;
using FaithOutreach.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FaithOutreach.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/notes")]
public class NotesController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    public NotesController(ApplicationDbContext db, UserManager<ApplicationUser> users)
    {
        _db = db;
        _users = users;
    }

    private string? CurrentUserId => _users.GetUserId(User);
    private static object View(NoteItem note) => new
    {
        note.Id, note.Title, note.Reference, note.Body, note.CreatedAt, note.UpdatedAt
    };

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var userId = CurrentUserId;
        if (userId is null) return Unauthorized();
        var notes = await _db.Notes.Where(n => n.UserId == userId)
            .OrderByDescending(n => n.UpdatedAt).ToListAsync();
        return Ok(notes.Select(View));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] NoteRequest request)
    {
        var userId = CurrentUserId;
        if (userId is null) return Unauthorized();
        var note = new NoteItem
        {
            UserId = userId,
            Title = request.Title.Trim(),
            Reference = request.Reference?.Trim() ?? "",
            Body = request.Body.Trim()
        };
        _db.Notes.Add(note);
        await _db.SaveChangesAsync();
        return Ok(View(note));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] NoteRequest request)
    {
        var userId = CurrentUserId;
        if (userId is null) return Unauthorized();
        var note = await _db.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
        if (note is null) return NotFound();
        note.Title = request.Title.Trim();
        note.Reference = request.Reference?.Trim() ?? "";
        note.Body = request.Body.Trim();
        note.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(View(note));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var userId = CurrentUserId;
        if (userId is null) return Unauthorized();
        var note = await _db.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);
        if (note is null) return NotFound();
        _db.Notes.Remove(note);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public sealed class NoteRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    [MaxLength(100)]
    public string? Reference { get; set; }
    [Required, MaxLength(4000)]
    public string Body { get; set; } = string.Empty;
}
