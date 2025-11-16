using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedBackPro.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("email");

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("ix_users_email");

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("password_hash");

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<UserRole>(v))
            .HasMaxLength(50)
            .HasColumnName("role");

        builder.Property(u => u.TeamId)
            .HasColumnName("team_id");

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(u => u.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.HasOne(u => u.Team)
            .WithMany(t => t.Members)
            .HasForeignKey(u => u.TeamId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}