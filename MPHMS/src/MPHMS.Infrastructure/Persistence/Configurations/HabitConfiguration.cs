using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MPHMS.Domain.Entities.Habits;

namespace MPHMS.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// HabitConfiguration defines how the Habit entity
    /// is mapped to the database.
    ///
    /// WHY THIS EXISTS:
    /// ----------------
    /// Keeps DbContext clean
    /// Centralizes DB constraints
    /// Improves maintainability
    /// Avoids DataAnnotations clutter
    ///
    /// Clean Architecture Rule:
    /// ------------------------
    /// Domain = PURE business model
    /// Infrastructure = DB mapping logic
    /// </summary>
    public class HabitConfiguration : IEntityTypeConfiguration<Habit>
    {
        public void Configure(EntityTypeBuilder<Habit> builder)
        {
            // ----------------------------------------------------
            // Table Name
            // ----------------------------------------------------

            builder.ToTable("Habits");

            // ----------------------------------------------------
            // Primary Key
            // ----------------------------------------------------

            builder.HasKey(h => h.Id);

            // ----------------------------------------------------
            // Property Configuration
            // ----------------------------------------------------

            builder.Property(h => h.Name)
                   .IsRequired()                  // NOT NULL
                   .HasMaxLength(200);            // Matches schema

            builder.Property(h => h.Difficulty)
                   .IsRequired();

            builder.Property(h => h.Status)
                   .IsRequired();

            // ----------------------------------------------------
            // Relationships
            // ----------------------------------------------------

            // Habit → User (Many-to-One)
       //      builder.HasOne(h => h.User)
       //             .WithMany()
       //             .HasForeignKey(h => h.UserId)
       //             .OnDelete(DeleteBehavior.Restrict);
builder.Property(x => x.UserId)
       .IsRequired();

            // Habit → Category (Many-to-One)
            builder.HasOne(h => h.Category)
                   .WithMany()
                   .HasForeignKey(h => h.CategoryId)
                   .OnDelete(DeleteBehavior.SetNull);

            // ----------------------------------------------------
            // Indexes
            // ----------------------------------------------------

            builder.HasIndex(h => h.UserId);
            builder.HasIndex(h => h.CategoryId);

            // ----------------------------------------------------
            // Audit Columns (BaseAuditableEntity)
            // ----------------------------------------------------

            builder.Property(h => h.CreatedAt)
                   .IsRequired();

            builder.Property(h => h.ModifiedAt);

            builder.Property(h => h.IsDeleted)
                   .HasDefaultValue(false);
        }
    }
}
