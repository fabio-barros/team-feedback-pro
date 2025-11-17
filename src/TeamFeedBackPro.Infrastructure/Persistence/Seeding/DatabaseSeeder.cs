using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;
using TeamFeedbackPro.Application.Common.Interfaces;
using TeamFeedBackPro.Infrastructure.Persistence;

namespace TeamFeedBackPro.Infrastructure.Persistence.Seeding;

public static class DatabaseSeeder
{
    public static async Task MigrateAndSeedAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher>();

        await context.Database.MigrateAsync();

        if (await context.Teams.AnyAsync().ConfigureAwait(false))
            return;

        var teamEngineering = new Team("Engineering");
        var teamMarketing = new Team("Marketing");

        await context.Teams.AddRangeAsync(teamEngineering, teamMarketing);

        var admin = new User(
            "admin@example.com",
            passwordHasher.HashPassword("Admin1234"),
            "System Admin",
            UserRole.Admin,
            teamEngineering.Id);

        var manager = new User(
            "manager@example.com",
            passwordHasher.HashPassword("Manager123"),
            "Team Manager",
            UserRole.Manager,
            teamEngineering.Id);

        var member = new User(
            "member@example.com",
            passwordHasher.HashPassword("Member123"),
            "Team Member",
            UserRole.Member,
            teamMarketing.Id);

        await context.Users.AddRangeAsync(admin, manager, member);

        await context.SaveChangesAsync();
    }
}