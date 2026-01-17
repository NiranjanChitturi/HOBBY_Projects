using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MPHMS.Domain.Entities.Goals;

namespace MPHMS.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Entity configuration for Milestone table mapping.
    /// </summary>
    public class MilestoneConfiguration : IEntityTypeConfiguration<Milestone>
    {
        public void Configure(EntityTypeBuilder<Milestone> builder)
        {
            builder.ToTable("Milestones");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(x => x.TargetValue)
                   .IsRequired();

            builder.Property(x => x.CurrentValue)
                   .IsRequired();

            // Relationship: Many Milestones -> One Goal
            builder.HasOne(x => x.Goal)
                   .WithMany(g => g.Milestones)
                   .HasForeignKey(x => x.GoalId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Audit fields
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        }
    }
}
