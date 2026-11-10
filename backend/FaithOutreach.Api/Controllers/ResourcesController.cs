using System.Text.Json.Nodes;
using FaithOutreach.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FaithOutreach.Api.Controllers;

[ApiController]
[Route("api/resources")]
public class ResourcesController : ControllerBase
{
    private static readonly HashSet<string> Categories = new(StringComparer.OrdinalIgnoreCase)
    { "readings", "teachings", "audiobooks", "podcasts", "guides", "hymns", "reflections" };
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;
    public ResourcesController(ApplicationDbContext db, IWebHostEnvironment env, IConfiguration config)
    {
        _db = db; _env = env; _config = config;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var path = Path.Combine(_env.ContentRootPath, "Resources", "catalog.json");
        var root = JsonNode.Parse(System.IO.File.ReadAllText(path))!.AsObject();
        var published = await _db.ContentItems.Where(x => x.IsPublished).OrderBy(x => x.CreatedAt).ToListAsync();
        foreach (var item in published)
        {
            var category = item.Category.ToLowerInvariant();
            if (!Categories.Contains(category)) continue;
            try
            {
                if (JsonNode.Parse(item.Body) is not JsonObject resource) continue;
                if (category is "readings" or "hymns" or "reflections")
                    resource["id"] = $"content-{item.Id}";
                root[category]!.AsArray().Add(resource);
            }
            catch (System.Text.Json.JsonException) { /* A malformed older item is skipped. */ }
        }

        if (_config["Site:Mission"] is { Length: > 0 } mission)
            root["site"]!["mission"] = mission;
        foreach (var key in new[] { "mpesaPaybill", "mpesaAccount", "bankName",
                     "bankAccountName", "bankAccountNumber", "contactEmail" })
        {
            if (_config[$"Site:Donation:{key}"] is { Length: > 0 } value)
                root["site"]!["donation"]![key] = value;
        }
        return Ok(root);
    }
}
