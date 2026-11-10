using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FaithOutreach.Api.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<ContentItem> ContentItems { get; set; }
    public DbSet<FeedbackMessage> FeedbackMessages { get; set; }
    public DbSet<SupportRequest> SupportRequests { get; set; }
    public DbSet<NoteItem> Notes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NoteItem>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UserId).IsRequired().HasMaxLength(450);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Reference).HasMaxLength(100);
            entity.Property(x => x.Body).IsRequired().HasMaxLength(4000);
            entity.HasIndex(x => x.UserId);
        });

        modelBuilder.Entity<ContentItem>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Category).IsRequired().HasMaxLength(50);
            entity.Property(x => x.Body).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<FeedbackMessage>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100);
            entity.Property(x => x.Email).HasMaxLength(254);
            entity.Property(x => x.Message).IsRequired().HasMaxLength(4000);
            entity.Property(x => x.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<SupportRequest>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Email).IsRequired().HasMaxLength(254);
            entity.Property(x => x.Phone).HasMaxLength(30);
            entity.Property(x => x.Topic).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Message).IsRequired().HasMaxLength(4000);
            entity.Property(x => x.Status).IsRequired().HasMaxLength(30);
            entity.Property(x => x.CreatedAt).IsRequired();
        });
    }
}

public class ApplicationUser : Microsoft.AspNetCore.Identity.IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}

public class ContentItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsPublished { get; set; }
}

public class NoteItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class FeedbackMessage
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SupportRequest
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Topic { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = "New";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
