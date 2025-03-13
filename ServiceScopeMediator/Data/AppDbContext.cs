using Microsoft.EntityFrameworkCore;
using ServiceScopeMediator.Model;

namespace ServiceScopeMediator.Data;

public class AppDbContext : DbContext
{
    public DbSet<Stuff> Stuffs { get; set; } = default!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}