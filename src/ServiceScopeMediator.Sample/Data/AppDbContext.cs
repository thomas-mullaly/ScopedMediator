using Microsoft.EntityFrameworkCore;
using ServiceScopeMediator.Sample.Model;

namespace ServiceScopeMediator.Sample.Data;

public class AppDbContext : DbContext
{
    public DbSet<Stuff> Stuffs { get; set; } = default!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}