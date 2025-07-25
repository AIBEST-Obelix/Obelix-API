using Microsoft.EntityFrameworkCore;
using Obelix.Api.Services.Requests.Data.Models;    

namespace Obelix.Api.Services.Requests.Data.Data;

/// <summary>
/// Application Database Context.
/// </summary>
/// <param name="options">Database Context Options.</param>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets Requests.
    /// </summary>
    public virtual DbSet<Request> Requests { get; set; }
    
    /// <summary>
    /// Gets or sets Items.
    /// </summary>
    public virtual DbSet<Item> Items { get; set; }
    
    /// <summary>
    /// Gets or sets Users.
    /// </summary>
    public virtual DbSet<User> Users { get; set; }
    
    /// <summary>
    /// Overrides the default on model creating method.
    /// </summary>
    /// <param name="builder">Model Builder.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}