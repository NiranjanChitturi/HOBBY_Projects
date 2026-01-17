using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MPHMS.Domain.Entities.Goals;

namespace MPHMS.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Maps Milestone entity.
    /// </summary>
    public class MilestoneConfiguration : IEntityTypeConfiguration<Milestone>
    {
        public void Configure(EntityTypeBuilder<Milestone> builder)
        {
            builder.ToTable("Milestones");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.IsCompleted)
                   .HasDefaultValue(false);

            builder.HasOne(x => x.Goal)
                   .WithMany(g => g.Milestones)
                   .HasForeignKey(x => x.GoalId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        }
    }
}
