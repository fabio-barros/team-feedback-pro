using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedBackPro.Infrastructure.Persistence.Configurations;

public class SprintConfiguration : IEntityTypeConfiguration<Sprint>
{
    public void Configure(EntityTypeBuilder<Sprint> builder)
    {
        builder.ToTable("sprints");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(f => f.Name)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(f => f.Description)
            .HasColumnName("description");

        builder.Property(f => f.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(f => f.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(f => f.StartAt)
            .IsRequired()
            .HasColumnType("date")
            .HasColumnName("start_at");

        builder.Property(f => f.EndAt)
            .IsRequired()
            .HasColumnType("date")
            .HasColumnName("end_at");

        builder.Property(f => f.TeamId)
            .IsRequired()
            .HasColumnName("team_id");
    }
}