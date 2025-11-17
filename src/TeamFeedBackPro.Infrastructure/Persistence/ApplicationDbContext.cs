using Microsoft.EntityFrameworkCore;
using TeamFeedbackPro.Domain.Entities;

namespace TeamFeedBackPro.Infrastructure.Persistence;
public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Team> Teams => Set<Team>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}