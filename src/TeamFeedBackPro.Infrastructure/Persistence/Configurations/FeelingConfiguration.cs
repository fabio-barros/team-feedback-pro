using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamFeedbackPro.Domain.Entities;
namespace TeamFeedBackPro.Infrastructure.Persistence.Configurations;

public class FeelingConfiguration : IEntityTypeConfiguration<Feeling>
{
    public void Configure(EntityTypeBuilder<Feeling> builder)
    {
        builder.ToTable("feelings");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.HasMany(t => t.Feedbacks)
            .WithOne(u => u.Feeling)
            .HasForeignKey(u => u.FeelingId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(t => t.CreatedAt);
        builder.Ignore(t => t.UpdatedAt);
    }
}
