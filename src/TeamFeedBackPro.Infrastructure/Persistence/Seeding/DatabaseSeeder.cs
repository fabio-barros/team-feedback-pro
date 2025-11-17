using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedBackPro.Infrastructure.Persistence.Seeding;

public static class DatabaseSeeder
{
    public static async Task MigrateAndSeedAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher>();
        var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();

        logger.LogInformation("Starting database migration and seeding...");

        await context.Database.MigrateAsync();

        if (await context.Teams.AnyAsync().ConfigureAwait(false))
        {
            logger.LogInformation("Database already seeded. Skipping seed data.");
            return;
        }

        logger.LogInformation("Seeding teams...");

        // Create Teams
        var teamEngineering = new Team("Engineering");
        var teamMarketing = new Team("Marketing");
        var teamProductDesign = new Team("Product & Design");
        var teamDataScience = new Team("Data Science");

        await context.Teams.AddRangeAsync(
            teamEngineering,
            teamMarketing,
            teamProductDesign,
            teamDataScience
        );

        await context.SaveChangesAsync();

        logger.LogInformation("Seeding users...");

        // Create Users - Engineering Team
        var admin = new User(
            "admin@teamfeedback.com",
            passwordHasher.HashPassword("Admin1234"),
            "Alice Anderson",
            UserRole.Admin,
            teamEngineering.Id);

        var managerEng = new User(
            "sarah.johnson@teamfeedback.com",
            passwordHasher.HashPassword("Manager123"),
            "Sarah Johnson",
            UserRole.Manager,
            teamEngineering.Id);

        var memberEng1 = new User(
            "bob.smith@teamfeedback.com",
            passwordHasher.HashPassword("Member123"),
            "Bob Smith",
            UserRole.Member,
            teamEngineering.Id);

        var memberEng2 = new User(
            "carol.white@teamfeedback.com",
            passwordHasher.HashPassword("Member123"),
            "Carol White",
            UserRole.Member,
            teamEngineering.Id);

        var memberEng3 = new User(
            "david.brown@teamfeedback.com",
            passwordHasher.HashPassword("Member123"),
            "David Brown",
            UserRole.Member,
            teamEngineering.Id);

        // Marketing Team
        var managerMkt = new User(
            "emily.davis@teamfeedback.com",
            passwordHasher.HashPassword("Manager123"),
            "Emily Davis",
            UserRole.Manager,
            teamMarketing.Id);

        var memberMkt1 = new User(
            "frank.miller@teamfeedback.com",
            passwordHasher.HashPassword("Member123"),
            "Frank Miller",
            UserRole.Member,
            teamMarketing.Id);

        var memberMkt2 = new User(
            "grace.wilson@teamfeedback.com",
            passwordHasher.HashPassword("Member123"),
            "Grace Wilson",
            UserRole.Member,
            teamMarketing.Id);

        // Product & Design Team
        var managerPD = new User(
            "henry.moore@teamfeedback.com",
            passwordHasher.HashPassword("Manager123"),
            "Henry Moore",
            UserRole.Manager,
            teamProductDesign.Id);

        var memberPD1 = new User(
            "isabel.taylor@teamfeedback.com",
            passwordHasher.HashPassword("Member123"),
            "Isabel Taylor",
            UserRole.Member,
            teamProductDesign.Id);

        var memberPD2 = new User(
            "jack.thomas@teamfeedback.com",
            passwordHasher.HashPassword("Member123"),
            "Jack Thomas",
            UserRole.Member,
            teamProductDesign.Id);

        // Data Science Team
        var managerDS = new User(
            "karen.jackson@teamfeedback.com",
            passwordHasher.HashPassword("Manager123"),
            "Karen Jackson",
            UserRole.Manager,
            teamDataScience.Id);

        var memberDS1 = new User(
            "leo.martin@teamfeedback.com",
            passwordHasher.HashPassword("Member123"),
            "Leo Martin",
            UserRole.Member,
            teamDataScience.Id);

        var memberDS2 = new User(
            "maria.garcia@teamfeedback.com",
            passwordHasher.HashPassword("Member123"),
            "Maria Garcia",
            UserRole.Member,
            teamDataScience.Id);

        await context.Users.AddRangeAsync(
            admin,
            managerEng, memberEng1, memberEng2, memberEng3,
            managerMkt, memberMkt1, memberMkt2,
            managerPD, memberPD1, memberPD2,
            managerDS, memberDS1, memberDS2
        );

        await context.SaveChangesAsync();

        logger.LogInformation("Seeding feedbacks...");

        // Create Sample Feedbacks
        var feedbacks = new List<Feedback>
        {
            // Engineering Team Feedbacks
            new(
                memberEng1.Id,
                memberEng2.Id,
                FeedbackType.Positive,
                FeedbackCategory.CodeQuality,
                "Great work on the recent code review! Your attention to detail helped us catch several potential bugs before they reached production. Keep up the excellent work.",
                false
            ),

            new(
                memberEng2.Id,
                memberEng1.Id,
                FeedbackType.Constructive,
                FeedbackCategory.Communication,
                "I appreciate your contributions in our sprint meetings. However, I think we could benefit from more detailed updates on your tasks. It would help the team understand dependencies better.",
                false
            ),

            new(
                managerEng.Id,
                memberEng3.Id,
                FeedbackType.Positive,
                FeedbackCategory.ProblemSolving,
                "Outstanding job solving that performance issue in the database queries. Your analytical approach and persistence really made a difference. The application is now 40% faster!",
                false
            ),

            new(
                memberEng3.Id,
                managerEng.Id,
                FeedbackType.Positive,
                FeedbackCategory.Leadership,
                "Thank you for your excellent leadership during the last sprint. Your clear communication and support helped the team deliver on time despite the challenges we faced.",
                false
            ),

            new(
                memberEng1.Id,
                memberEng3.Id,
                FeedbackType.Constructive,
                FeedbackCategory.Teamwork,
                "You're a great developer, but I've noticed you sometimes work in isolation. Collaborating more with the team during implementation could lead to better solutions and knowledge sharing.",
                false
            ),

            // Marketing Team Feedbacks
            new(
                memberMkt1.Id,
                memberMkt2.Id,
                FeedbackType.Positive,
                FeedbackCategory.Communication,
                "Your presentation to stakeholders was excellent! You communicated complex metrics in a way that everyone could understand. This really helped get buy-in for our new campaign.",
                false
            ),

            new(
                memberMkt2.Id,
                managerMkt.Id,
                FeedbackType.Positive,
                FeedbackCategory.Leadership,
                "I really appreciate how you've been mentoring me on campaign strategy. Your guidance has significantly improved my approach to content planning and execution.",
                false
            ),

            new(
                managerMkt.Id,
                memberMkt1.Id,
                FeedbackType.Constructive,
                FeedbackCategory.Other,
                "Your creative ideas are fantastic, but I've noticed some deadlines have been missed recently. Let's work together to improve time management and prioritization of tasks.",
                false
            ),

            // Product & Design Team Feedbacks
            new(
                memberPD1.Id,
                memberPD2.Id,
                FeedbackType.Positive,
                FeedbackCategory.CodeQuality,
                "The UI components you designed are not only beautiful but also highly reusable. The design system you're building will save the team countless hours. Excellent work!",
                false
            ),

            new(
                managerPD.Id,
                memberPD1.Id,
                FeedbackType.Positive,
                FeedbackCategory.ProblemSolving,
                "I'm impressed by how you approached the accessibility issues in our app. Your research and implementation of ARIA labels and keyboard navigation shows real dedication to inclusive design.",
                false
            ),

            // Data Science Team Feedbacks
            new(
                memberDS1.Id,
                memberDS2.Id,
                FeedbackType.Positive,
                FeedbackCategory.Teamwork,
                "Your collaboration on the ML model was exceptional. You were always willing to explain complex concepts and help debug issues. This really accelerated our project timeline.",
                false
            ),

            new(
                memberDS2.Id,
                managerDS.Id,
                FeedbackType.Constructive,
                FeedbackCategory.Communication,
                "While your technical expertise is outstanding, I think our stakeholder presentations could be improved by simplifying the technical jargon. Not everyone has a data science background.",
                false
            ),

            new(
                managerDS.Id,
                memberDS1.Id,
                FeedbackType.Positive,
                FeedbackCategory.ProblemSolving,
                "Your innovative approach to feature engineering significantly improved our model's accuracy. Your ability to think outside the box and experiment with new techniques is a real asset to the team.",
                false
            ),

            // Anonymous Feedback Examples
            new(
                memberEng2.Id,
                managerEng.Id,
                FeedbackType.Critical,
                FeedbackCategory.Leadership,
                "I feel that some decisions are made without sufficient team input. More transparency in the decision-making process would help build trust and ensure we're all aligned on priorities.",
                true 
            ),

            new(
                memberMkt2.Id,
                memberMkt1.Id,
                FeedbackType.Constructive,
                FeedbackCategory.Teamwork,
                "Sometimes during team discussions, you tend to dominate the conversation. Giving others more space to contribute their ideas would improve our collaborative environment.",
                true 
            )
        };

        await context.Feedbacks.AddRangeAsync(feedbacks);
        await context.SaveChangesAsync();

        logger.LogInformation("Database seeding completed successfully!");
        logger.LogInformation("  - Teams: 4");
        logger.LogInformation("  - Users: 14 (1 Admin, 4 Managers, 9 Members)");
        logger.LogInformation("  - Feedbacks: {FeedbackCount}", feedbacks.Count);
        logger.LogInformation("Sample login credentials:");
        logger.LogInformation("  Admin: admin@teamfeedback.com / Admin1234");
        logger.LogInformation("  Manager: sarah.johnson@teamfeedback.com / Manager123");
        logger.LogInformation("  Member: bob.smith@teamfeedback.com / Member123");
    }
}