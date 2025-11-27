using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamFeedbackPro.Domain.Entities;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedBackPro.Infrastructure.Persistence.Configurations;

public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.ToTable("feedbacks");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(f => f.AuthorId)
            .IsRequired()
            .HasColumnName("author_id");

        builder.Property(f => f.RecipientId)
            .IsRequired()
            .HasColumnName("recipient_id");

        builder.Property(f => f.Type)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<FeedbackType>(v))
            .HasMaxLength(50)
            .HasColumnName("type");

        builder.Property(f => f.Category)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<FeedbackCategory>(v))
            .HasMaxLength(50)
            .HasColumnName("category");

        builder.Property(f => f.Content)
            .IsRequired()
            .HasMaxLength(2000)
            .HasColumnName("content");

        builder.Property(f => f.IsAnonymous)
            .IsRequired()
            .HasColumnName("is_anonymous");

        builder.Property(f => f.Status)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => Enum.Parse<FeedbackStatus>(v))
            .HasMaxLength(50)
            .HasColumnName("status");

        builder.Property(f => f.ReviewedBy)
            .HasColumnName("reviewed_by");

        builder.Property(f => f.ReviewedAt)
            .HasColumnName("reviewed_at");

        builder.Property(f => f.ReviewNotes)
            .HasMaxLength(500)
            .HasColumnName("review_notes");

        builder.Property(f => f.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(f => f.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.Property(f => f.TeamId)
            .IsRequired()
            .HasColumnName("team_id");

        // Relationships
        builder.HasOne(f => f.Author)
            .WithMany()
            .HasForeignKey(f => f.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Recipient)
            .WithMany()
            .HasForeignKey(f => f.RecipientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Reviewer)
            .WithMany()
            .HasForeignKey(f => f.ReviewedBy)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(f => f.Team)
            .WithMany()
            .HasForeignKey(f => f.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(f => f.AuthorId)
            .HasDatabaseName("ix_feedbacks_author_id");

        builder.HasIndex(f => f.RecipientId)
            .HasDatabaseName("ix_feedbacks_recipient_id");

        builder.HasIndex(f => f.Status)
            .HasDatabaseName("ix_feedbacks_status");

        builder.HasIndex(f => f.CreatedAt)
            .HasDatabaseName("ix_feedbacks_created_at");
    }
}