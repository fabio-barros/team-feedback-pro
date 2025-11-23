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

        // await context.Database.MigrateAsync();

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
                "Ótimo trabalho na revisão de código recente! Sua atenção aos detalhes nos ajudou a identificar vários possíveis bugs antes que chegassem à produção. Continue com o excelente trabalho.",
                false
            ),

            new(
                memberEng2.Id,
                memberEng1.Id,
                FeedbackType.Constructive,
                FeedbackCategory.Communication,
                "Agradeço suas contribuições nas nossas reuniões de sprint. No entanto, acho que poderíamos nos beneficiar de atualizações mais detalhadas sobre suas tarefas. Isso ajudaria a equipe a entender melhor as dependências.",
                false
            ),

            new(
                managerEng.Id,
                memberEng3.Id,
                FeedbackType.Positive,
                FeedbackCategory.ProblemSolving,
                "Excelente trabalho ao resolver aquele problema de desempenho nas consultas do banco de dados. Sua abordagem analítica e persistência realmente fizeram a diferença. O aplicativo agora está 40% mais rápido!",
                false
            ),

            new(
                memberEng3.Id,
                managerEng.Id,
                FeedbackType.Positive,
                FeedbackCategory.Leadership,
                "Obrigado pela excelente liderança durante o último sprint. Sua comunicação clara e apoio ajudaram a equipe a entregar no prazo, apesar dos desafios que enfrentamos.",
                false
            ),

            new(
                memberEng1.Id,
                memberEng3.Id,
                FeedbackType.Constructive,
                FeedbackCategory.Teamwork,
                "Você é um ótimo desenvolvedor, mas percebi que às vezes trabalha de forma isolada. Colaborar mais com a equipe durante a implementação pode levar a melhores soluções e compartilhamento de conhecimento.",
                false
            ),

            // Marketing Team Feedbacks
            new(
                memberMkt1.Id,
                memberMkt2.Id,
                FeedbackType.Positive,
                FeedbackCategory.Communication,
                "Sua apresentação para os stakeholders foi excelente! Você comunicou métricas complexas de um jeito que todos puderam entender. Isso realmente ajudou a conquistar apoio para nossa nova campanha.",
                false
            ),

            new(
                memberMkt2.Id,
                managerMkt.Id,
                FeedbackType.Positive,
                FeedbackCategory.Leadership,
                "Agradeço muito como você vem me mentorando em estratégia de campanha. Sua orientação melhorou significativamente minha abordagem ao planejamento e execução de conteúdo.",
                false
            ),

            new(
                managerMkt.Id,
                memberMkt1.Id,
                FeedbackType.Constructive,
                FeedbackCategory.Other,
                "Suas ideias criativas são fantásticas, mas notei que alguns prazos têm sido perdidos recentemente. Vamos trabalhar juntos para melhorar a gestão do tempo e a priorização de tarefas.",
                false
            ),

            // Product & Design Team Feedbacks
            new(
                memberPD1.Id,
                memberPD2.Id,
                FeedbackType.Positive,
                FeedbackCategory.CodeQuality,
                "Os componentes de UI que você projetou não são apenas lindos, mas também altamente reutilizáveis. O sistema de design que você está construindo vai economizar inúmeras horas da equipe. Excelente trabalho!",
                false
            ),

            new(
                managerPD.Id,
                memberPD1.Id,
                FeedbackType.Positive,
                FeedbackCategory.ProblemSolving,
                "Estou impressionado com a forma como você abordou os problemas de acessibilidade em nosso app. Sua pesquisa e implementação de labels ARIA e navegação por teclado demonstram verdadeira dedicação ao design inclusivo.",
                false
            ),

            // Data Science Team Feedbacks
            new(
                memberDS1.Id,
                memberDS2.Id,
                FeedbackType.Positive,
                FeedbackCategory.Teamwork,
                "Sua colaboração no modelo de ML foi excepcional. Você sempre esteve disposto a explicar conceitos complexos e ajudar a depurar problemas. Isso realmente acelerou o cronograma do projeto.",
                false
            ),

            new(
                memberDS2.Id,
                managerDS.Id,
                FeedbackType.Constructive,
                FeedbackCategory.Communication,
                "Embora sua expertise técnica seja excelente, acho que nossas apresentações para stakeholders poderiam ser melhoradas simplificando o jargão técnico. Nem todos têm formação em ciência de dados.",
                false
            ),

            new(
                managerDS.Id,
                memberDS1.Id,
                FeedbackType.Positive,
                FeedbackCategory.ProblemSolving,
                "Sua abordagem inovadora de feature engineering melhorou significativamente a precisão do nosso modelo. Sua capacidade de pensar fora da caixa e experimentar novas técnicas é um grande diferencial para a equipe.",
                false
            ),

            // Anonymous Feedback Examples
            new(
                memberEng2.Id,
                managerEng.Id,
                FeedbackType.Critical,
                FeedbackCategory.Leadership,
                "Sinto que algumas decisões são tomadas sem contribuição suficiente da equipe. Maior transparência no processo de tomada de decisão ajudaria a construir confiança e garantir que todos estejamos alinhados nas prioridades.",
                true 
            ),

            new(
                memberMkt2.Id,
                memberMkt1.Id,
                FeedbackType.Constructive,
                FeedbackCategory.Teamwork,
                "Às vezes, durante as discussões da equipe, você tende a dominar a conversa. Dar mais espaço para que os outros contribuam com suas ideias melhoraria nosso ambiente de colaboração.",
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