using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamFeedbackPro.Domain.Entities;
namespace TeamFeedBackPro.Infrastructure.Persistence.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("teams");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.Property(t => t.ManagerId)
            .HasColumnName("manager_id");

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(t => t.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.HasMany(t => t.Members)
            .WithOne(u => u.Team)
            .HasForeignKey(u => u.TeamId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
