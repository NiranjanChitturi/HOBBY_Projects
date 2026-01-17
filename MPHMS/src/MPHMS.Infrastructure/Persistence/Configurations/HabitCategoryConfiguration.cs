using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MPHMS.Domain.Entities.Habits;

namespace MPHMS.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Maps HabitCategory domain entity to database table.
    /// </summary>
    public class HabitCategoryConfiguration : IEntityTypeConfiguration<HabitCategory>
    {
        public void Configure(EntityTypeBuilder<HabitCategory> builder)
        {
            builder.ToTable("HabitCategories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.DisplayOrder)
                   .IsRequired();

            // Audit fields
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);

            // Index
            builder.HasIndex(x => x.Name).IsUnique();
        }
    }
}
