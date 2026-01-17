using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MPHMS.Domain.Entities.Habits;

namespace MPHMS.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Maps HabitSkipLog entity.
    /// </summary>
    public class HabitSkipLogConfiguration : IEntityTypeConfiguration<HabitSkipLog>
    {
        public void Configure(EntityTypeBuilder<HabitSkipLog> builder)
        {
            builder.ToTable("HabitSkipLogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Comment)
                   .HasMaxLength(2000);

            // Relationships
            builder.HasOne(x => x.HabitLog)
                   .WithOne(l => l.SkipLog)
                   .HasForeignKey<HabitSkipLog>(x => x.HabitLogId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.SkipReason)
                   .WithMany()
                   .HasForeignKey(x => x.ReasonId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Audit fields
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        }
    }
}
