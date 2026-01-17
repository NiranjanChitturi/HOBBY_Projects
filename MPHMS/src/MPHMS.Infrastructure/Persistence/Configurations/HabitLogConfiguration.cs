using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MPHMS.Domain.Entities.Habits;

namespace MPHMS.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Maps HabitLog entity to HabitLogs table.
    /// </summary>
    public class HabitLogConfiguration : IEntityTypeConfiguration<HabitLog>
    {
        public void Configure(EntityTypeBuilder<HabitLog> builder)
        {
            builder.ToTable("HabitLogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.LogDate)
                   .IsRequired();

            builder.Property(x => x.Status)
                   .IsRequired();

            builder.Property(x => x.Notes)
                   .HasMaxLength(1000);

            // Relationship
            builder.HasOne(x => x.Habit)
                   .WithMany(h => h.Logs)
                   .HasForeignKey(x => x.HabitId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint (HabitId + LogDate)
            builder.HasIndex(x => new { x.HabitId, x.LogDate })
                   .IsUnique();

            // Audit fields
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        }
    }
}
